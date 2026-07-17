using System;
using UnityEngine;

public class CharacterSkillSystem : MonoBehaviour
{
    private const float SkillDamageMultiplier = 0.01f;
    private const float GaugePerSecond = 5.0f;
    private const string EnemyTag = "Enemy";

    private CharacterAttack _characterAttack;
    private BattleCharacter _battleCharacter;
    private RuntimeSkill _basicSkill;
    private RuntimeSkill _normalSkill;
    private RuntimeSkill _ultimateSkill;
    private int _currentGauge;
    private int _maxGauge;
    private float _gaugeAccumulator;
    

    public event Action<int, int> OnGaugeChanged;

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
    public void Initialize(CharacterData data)
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
    }
    public bool CanUseBasicSkill()
    {
        if (_basicSkill == null)
        {
            return false;
        }
        return _basicSkill.IsReady();
    }

    public void UseBasicSkill(Transform target)
    {
        if (_basicSkill == null || _basicSkill.IsReady() == false)
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

    public void UseNormalSkill(Transform target)
    {
        if (_normalSkill == null || _normalSkill.IsReady() == false)
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

        Transform target = FindNearestEnemy(_ultimateSkill.Data.SkillRange);
        if (target == null)
        {
            Debug.Log("사거리 내 적 없음");
            return;
        }

        ExecuteSkill(_ultimateSkill, target);
        Debug.Log($"궁극 스킬 사용: {_ultimateSkill.Data.Name}");
        ChangeGauge(0);
    }

    private void ExecuteSkill(RuntimeSkill skill, Transform target)
    {
        int damage = (int)(_battleCharacter.CurAtk * SkillDamageMultiplier * skill.Data.DamageCoefficient);

        switch (skill.Data.Type)
        {
            case CharacterSkillType.SingleAttack:
                _characterAttack.FireProjectile(target, damage, this, skill.Data.GaugeRecovery);
                break;
            case CharacterSkillType.AreaAttack:
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
                AddGauge(skill.Data.GaugeRecovery);
                break;
            case CharacterSkillType.HealBuff:
                //TODO
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
}
