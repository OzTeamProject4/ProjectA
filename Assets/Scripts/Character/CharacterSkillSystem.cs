using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkillSystem : MonoBehaviour
{
    private const float SkillDamageMultiplier = 0.01f;
    private const float GaugePerSecond = 5.0f;
    private const string EnemyTag = "Enemy";
    private const float EffectLifeTime = 3.0f;
    private const int SkillPrewarmCount = 5;
    private const float MinAttackRangeRatio = 0.4f;

    private CharacterAttack _characterAttack;
    private BattleCharacter _battleCharacter;
    private RuntimeSkill _basicSkill;
    private RuntimeSkill _normalSkill;
    private RuntimeSkill _ultimateSkill;
    private int _currentGauge;
    private int _maxGauge;
    private float _gaugeAccumulator;
    private List<string> _loadedPrefabKeys = new List<string>();

    public event Action<int, int> OnGaugeChanged;
    public event Action<int, float, float, GameObject> OnHealBuffRequested;
    public event Action<CharacterSkillCategory> OnSkillUsed;

    public float MaxSkillRange
    {
        get
        {
            float maxRange = 0f;

            if (_basicSkill != null)
            {
                maxRange = Mathf.Max(maxRange, _basicSkill.Data.SkillRange);
            }

            if (_normalSkill != null)
            {
                maxRange = Mathf.Max(maxRange, _normalSkill.Data.SkillRange);
            }

            if (_ultimateSkill != null)
            {
                maxRange = Mathf.Max(maxRange, _ultimateSkill.Data.SkillRange);
            }
            return maxRange;
        }
    }

    public float AttackRange
    {
        get
        {
            float basicRange = GetAttackSkillRange(_basicSkill);
            float normalRange = GetAttackSkillRange(_normalSkill);

            if (basicRange <= 0f && normalRange <= 0f)
            {
                return 0f;
            }

            if (basicRange <= 0f)
            {
                return normalRange;
            }

            if (normalRange <= 0f)
            {
                return basicRange;
            }

            return Mathf.Min(basicRange, normalRange);
        }
    }

    public float MinAttackRange
    {
        get
        {
            return AttackRange * MinAttackRangeRatio;
        }
    }

    
    
    private void Awake()
    {
        _battleCharacter = GetComponent<BattleCharacter>();
        _characterAttack = GetComponent<CharacterAttack>();

        if (_battleCharacter == null )
        {
            Debug.LogError("BattleCharacter 컴포넌트가 null");
        }

        if (_characterAttack == null)
        {
            Debug.LogError("CharacterAttack 컴포넌트가 null");

        }
    }

    private void Update()
    {
        UpdateGauge();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        foreach (string key in _loadedPrefabKeys)
        {
            GameManager.Instance.ResourceManager.ReleaseAsset(key);
        }
        _loadedPrefabKeys.Clear();
    }
    public async UniTask InitializeAsync(CharacterData data)
    {
        _maxGauge = data.SkillGauge;
        ChangeGauge(0);

        foreach (string skillId in data.ParsedSkillList)
        {
            if (GameManager.Instance.DataManager.TryGetData<CharacterSkillData>(skillId, out CharacterSkillData skillData))
            {
                switch (skillData.Category)
                {
                    case CharacterSkillCategory.Basic:
                        _basicSkill = new RuntimeSkill(skillData);
                        break;

                    case CharacterSkillCategory.Normal:
                        _normalSkill = new RuntimeSkill(skillData);
                        break;

                    case CharacterSkillCategory.Ultimate:
                        _ultimateSkill = new RuntimeSkill(skillData);
                        break;
                }
            }

            else
            {
                Debug.LogError($"스킬 데이터 없음 {skillId}");
            }
        }
        await LoadSkillPrefabs();
    }
    public bool CanUseBasicSkill()
    {
        if (_basicSkill == null)
        {
            return false;
        }
        return _basicSkill.IsReady();
    }

    public void UseBasicSkill()
    {
        if (_basicSkill == null || _basicSkill.IsReady() == false)
        {
            return;
        }

        Transform target = FindNearestEnemy(_basicSkill.Data.SkillRange);
        ExecuteSkill(_basicSkill, target);
        _basicSkill.MarkUsed();
    }

    public void UseBasicSkill(Transform target)
    {
        if (_basicSkill == null || _basicSkill.IsReady() == false)
        {
            return;
        }

        if (_basicSkill.Data.Type != CharacterSkillType.HealBuff && target == null)
        {
            return;
        }

        ExecuteSkill(_basicSkill, target);
        _basicSkill.MarkUsed();
    }

    public bool CanUseNormalSkill()
    {
        if (_normalSkill == null)
        {
            return false;
        }

        return _normalSkill.IsReady();
    }

    public void UseNormalSkill()
    {
        if (_normalSkill == null || _normalSkill.IsReady() == false)
        {
            return;
        }
        
        if (_normalSkill.Data.Type == CharacterSkillType.HealBuff)
        {
            ExecuteSkill(_normalSkill, null);
            _normalSkill.MarkUsed();
            return;
        }

        Transform target = FindNearestEnemy(_normalSkill.Data.SkillRange);
        ExecuteSkill(_normalSkill, target);
        _normalSkill.MarkUsed();
    }
    public void UseNormalSkill(Transform target)
    {
        if (_normalSkill == null || _normalSkill.IsReady() == false)
        {
            return;
        }

        if (_normalSkill.Data.Type != CharacterSkillType.HealBuff && target == null)
        {
            return;
        }

        ExecuteSkill(_normalSkill, target);
        _normalSkill.MarkUsed();
    }

    public bool CanUseUltSkill()
    {
        if (_ultimateSkill == null)
        {
            return false;
        }

        return _currentGauge >= _maxGauge;
    }

    public void UseUltSkill()
    {
        if (_ultimateSkill == null /*|| _currentGauge < _maxGauge */)
        {
            return;
        }

        if (_ultimateSkill.Data.Type == CharacterSkillType.HealBuff)
        {
            ExecuteSkill(_ultimateSkill, null);
            ChangeGauge(0);
            return;
        }

        Transform target = FindNearestEnemy(_ultimateSkill.Data.SkillRange);
        if (target == null)
        {
            Debug.Log("사거리 내 적 없음");
            return;
        }

        ExecuteSkill(_ultimateSkill, target);
        ChangeGauge(0);
    }

    private void ExecuteSkill(RuntimeSkill skill, Transform target)
    {
        if (skill.Data.Type != CharacterSkillType.HealBuff && target == null && skill.Data.ProjectileSpeed <= 0)
        {
            return;
        }

        if (target != null)
        {
            _battleCharacter.LookAtInstant(target.position); 
        }

        OnSkillUsed?.Invoke(skill.Data.Category);

        int damage = (int)(_battleCharacter.CurAtk * SkillDamageMultiplier * skill.Data.DamageCoefficient);

        switch (skill.Data.Type)
        {
            case CharacterSkillType.SingleAttack:
                if (skill.Data.ProjectileSpeed > 0)
                {
                    _characterAttack.FireProjectile(skill.Data.PrefabPath, target, damage, this, skill.Data.GaugeRecovery, skill.Data.ProjectileSpeed);
                }

                else
                {
                    if (skill.ProjectilePrefab == null)
                    {
                        Debug.LogError($"[{skill.Data.Name}] SingleAttack인데 프리팹 null (경로: {skill.Data.PrefabPath})");
                    }

                    IDamageable damageable = target.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(damage, gameObject);
                    }

                    if (skill.ProjectilePrefab != null)
                    {
                        GameObject effect = Instantiate(skill.ProjectilePrefab, target.position, Quaternion.identity);
                        Destroy(effect, EffectLifeTime);
                    }

                    AddGauge(skill.Data.GaugeRecovery);
                }
                break;

            case CharacterSkillType.AreaAttack:
                if (skill.Data.ProjectileSpeed > 0)
                {
                    _characterAttack.FireProjectile(skill.Data.PrefabPath, target, damage, this, skill.Data.GaugeRecovery, skill.Data.ProjectileSpeed, skill.Data.AreaRadius);
                }

                else
                {
                    Collider[] hits = Physics.OverlapSphere(target.position, skill.Data.AreaRadius);
                    foreach (Collider hit in hits)
                    {
                        if (hit.CompareTag(EnemyTag))
                        {
                            IDamageable damageable = hit.GetComponent<IDamageable>();
                            if (damageable != null)
                            {
                                damageable.TakeDamage(damage, gameObject);
                            }
                        }
                    }
                    if (skill.ProjectilePrefab != null)
                    {
                        GameObject effect = Instantiate(skill.ProjectilePrefab, target.position, Quaternion.identity);
                        Destroy(effect, EffectLifeTime);
                    }
                    AddGauge(skill.Data.GaugeRecovery);
                }
                break;
            case CharacterSkillType.HealBuff:
                int healAmount = (int)(_battleCharacter.CurAtk * SkillDamageMultiplier * skill.Data.HealAmount);
                OnHealBuffRequested?.Invoke(healAmount, skill.Data.BuffDuration, skill.Data.MoveSpeedBuff, skill.ProjectilePrefab);
                break;
        }
    }

    public void AddGauge(int amount)
    {
        ChangeGauge(_currentGauge + amount);
    }

    public void UpdateGauge()
    {
        if (_currentGauge >= _maxGauge)
        {
            return;
        }

        _gaugeAccumulator += GaugePerSecond * Time.deltaTime;
        if (_gaugeAccumulator >= 1.0f)
        {
            int gain = (int)_gaugeAccumulator;
            _gaugeAccumulator -= gain;
            ChangeGauge(_currentGauge + gain);
        }
    }

    private void ChangeGauge(int newValue)
    {
        _currentGauge = Mathf.Clamp(newValue, 0, _maxGauge);
        OnGaugeChanged?.Invoke(_currentGauge, _maxGauge);
    }
    private Transform FindNearestEnemy(float range)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        Transform nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit.isTrigger == true)
            {
                continue;
            }

            if (hit.CompareTag(EnemyTag) == false)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    private async UniTask LoadSkillPrefabs()
    {
        await LoadPrefabForSkill(_basicSkill);
        await LoadPrefabForSkill(_normalSkill);
        await LoadPrefabForSkill(_ultimateSkill);
    }

    private async UniTask LoadPrefabForSkill(RuntimeSkill skill)
    {
        if (skill == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(skill.Data.PrefabPath) == true)
        {
            return;
        }

        GameObject prefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(skill.Data.PrefabPath);

        if (prefab == null)
        {
            Debug.LogError($"프리팹 로드 실패 {skill.Data.PrefabPath}");
            return;
        }

        skill.SetProjectilePrefab(prefab);
        _loadedPrefabKeys.Add(skill.Data.PrefabPath);

        
        if (skill.Data.ProjectileSpeed > 0)
        {
            await GameManager.Instance.ObjectManager.PrewarmAsync(skill.Data.PrefabPath, SkillPrewarmCount, destroyCancellationToken);
        }
    }

    private float GetAttackSkillRange(RuntimeSkill skill)
    {
        if (skill == null)
        {
            return 0f;
        }

        if (skill.Data.Type == CharacterSkillType.HealBuff)
        {
            return 0f;
        }

        return skill.Data.SkillRange;
    }
}
