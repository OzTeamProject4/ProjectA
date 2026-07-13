using UnityEngine;

// 최종 스텟 계산 공식 = 기본값 + (스탯 상승량 + 성급 상승량) × (레벨 - 1)   [+ 장비 스탯]
public static class StatCalculator
{
    public static StatData Calculate(CharacterStatData stat, CharacterGradeData grade, int level)
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
        float gradeAtkSpeedGrow = 0f;
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
            gradeAtkSpeedGrow = grade.AtkSpeedGrow;
            gradeMoveSpeedGrow = grade.MoveSpeedGrow;
        }


        int levelStep = level - 1;

        return new StatData
        {
            Hp = stat.Hp + (stat.HpGrow + gradeHpGrow) * levelStep,
            Atk = stat.Atk + (stat.AtkGrow + gradeAtkGrow) * levelStep,
            Def = stat.Def + (stat.DefGrow + gradeDefGrow) * levelStep,
            AtkSpeed = stat.AtkSpeed + (stat.AtkSpeedGrow + gradeAtkSpeedGrow) * levelStep,
            MoveSpeed = stat.MoveSpeed + (stat.MoveSpeedGrow + gradeMoveSpeedGrow) * levelStep
        };
    }
}
