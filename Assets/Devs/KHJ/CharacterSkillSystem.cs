using UnityEngine;

public class CharacterSkillSystem : MonoBehaviour
{
    private RuntimeSkill _basicSkill;
    private RuntimeSkill _normalSkill;
    private RuntimeSkill _ultimateSkill;
    

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

    public bool CanUseSkill()
    {
        if (_normalSkill == null)
        {
            return false;
        }
        return _normalSkill.IsReady();
    }

    public void UseSkill(Transform target)
    {
        if (_normalSkill == null)
        {
            return;
        }
        
        if (_normalSkill.IsReady() == false)
        {
            return;
        }

        // TODO희준 : 실제 스킬 실행 필요
        Debug.Log($"일반 스킬 사용: {_normalSkill.Data.Name}");
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
}
