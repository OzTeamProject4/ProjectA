using UnityEngine;

public class CharacterSkillSystem : MonoBehaviour
{
    private const float SkillDamageMultiplier = 0.01f;

    private CharacterAttack _characterAttack;
    private BattleCharacter _battleCharacter;
    private RuntimeSkill _basicSkill;
    private RuntimeSkill _normalSkill;
    private RuntimeSkill _ultimateSkill;

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

    public void Initialize(CharacterData data)
    {
        foreach (string skillId in data.SkillList)
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
        
        // TODO 희준: 궁극기는 쿨타임이 아닌 게이지 확인으로 판정
        return _ultimateSkill.IsReady();
    }

    public void UseUltSkill(Transform target)
    {
        if (_ultimateSkill == null)
        {
            return;
        }

        if (_ultimateSkill.IsReady() == false)
        {
            return;
        }

        // TODO희준 : 실제 스킬 실행 필요
        Debug.Log($"궁극 스킬 사용: {_ultimateSkill.Data.Name}");
        _ultimateSkill.MarkUsed();
    }

    private void ExecuteSkill(RuntimeSkill skill, Transform target)
    {
        switch (skill.Data.Type)
        {
            case CharacterSkillType.SingleAttack:
                int damage = (int)(_battleCharacter.CurAtk * SkillDamageMultiplier * skill.Data.DamageCoefficient);
                _characterAttack.FireProjectile(target, damage);
                break;
            case CharacterSkillType.AreaAttack:
                //TODO
                break;
            case CharacterSkillType.HealBuff:
                //TODO
                break;
        }
    }
}
