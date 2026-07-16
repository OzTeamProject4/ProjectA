using System.Collections.Generic;
using UnityEngine;

// 최종 스텟 계산 공식 = 기본값 + 성급 상승량 + (스탯 상승량 * 레벨 - 1) + 장비 스탯
public static class StatCalculator
{
    public static StatData Calculate(CharacterData stat, CharacterGradeData grade, int level, StatData equipmentBonus)
    {
        if (null == stat)
        {
            Debug.LogWarning("CharacterStatData 가 null 입니다. 기본값(0)을 반환합니다.");
            return default;
        }

        if (level < 1)
        {
            Debug.LogWarning($"잘못된 레벨({level}). 1로 보정합니다.");
            level = 1;
        }

        float gradeHpGrow = 0f;
        float gradeAtkGrow = 0f;
        float gradeDefGrow = 0f;
        float gradeMoveSpeedGrow = 0f;

        if (null == grade)
        {
            Debug.LogWarning("CharacterGradeData 가 null 입니다. 성급 상승량을 0으로 처리합니다.");
        }
        else
        {
            gradeHpGrow = grade.HpGrow;
            gradeAtkGrow = grade.AtkGrow;
            gradeDefGrow = grade.DefGrow;
            gradeMoveSpeedGrow = grade.MoveSpeedGrow;
        }


        int levelStep = level - 1;

        return new StatData
        {
            Hp = stat.Hp + gradeHpGrow + (stat.HpGrow * levelStep ) + equipmentBonus.Hp,
            Atk = stat.Atk + gradeAtkGrow + (stat.AtkGrow * levelStep) + equipmentBonus.Atk,
            Def = stat.Def + gradeDefGrow + (stat.DefGrow * levelStep) + equipmentBonus.Def,
            MoveSpeed = stat.MoveSpeed + gradeMoveSpeedGrow + (stat.MoveSpeedGrow * levelStep) + equipmentBonus.MoveSpeed
        };
    }

    public static StatData SumEquipmentStats(IReadOnlyList<EquipmentInstance> equippedItems)
    {
        if (null == equippedItems)
        {
            Debug.LogWarning("equippedItems 가 null 입니다. 장비 보너스를 0으로 처리합니다.");
            return default;
        }

        float hp = 0f;
        float atk = 0f;
        float def = 0f;
        float moveSpeed = 0f;

        foreach (EquipmentInstance instance in equippedItems)
        {
            if (null == instance)
            {
                continue;
            }

            hp += instance.TotalHp;
            atk += instance.TotalAtk;
            def += instance.TotalDef;
            moveSpeed += instance.TotalMoveSpeed;
        }

        return new StatData
        {
            Hp = hp,
            Atk = atk,
            Def = def,
            MoveSpeed = moveSpeed
        };
    }
}
