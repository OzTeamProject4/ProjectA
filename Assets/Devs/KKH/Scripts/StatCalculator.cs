using UnityEngine;

// 최종 스텟 계산 공식 = 기본값 + (스탯 상승량 + 성급 상승량) × (레벨 - 1)   [+ 장비 스탯]
public static class StatCalculator
{
    public static FinalStats Calculate(CharacterStatData stat, CharacterGradeData grade, int level)
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

        if (null == grade)
        {
            Debug.LogWarning("CharacterGradeData 가 null 입니다. 성급 상승량을 0으로 처리합니다.");
        }

        int levelStep = level - 1;

        return new FinalStats
        {
            // TODO - 07.08: 장비 스텟 합산은 여기서 수행
            Hp = stat.Hp + (stat.HpGrow + (grade?.HpGrow ?? 0f)) * levelStep,
            Atk = stat.Atk + (stat.AtkGrow + (grade?.AtkGrow ?? 0f)) * levelStep,
            Def = stat.Def + (stat.DefGrow + (grade?.DefGrow ?? 0f)) * levelStep,
            AtkSpeed = stat.AtkSpeed + (stat.AtkSpeedGrow + (grade?.AtkSpeedGrow ?? 0f)) * levelStep,
            MoveSpeed = stat.MoveSpeed + (stat.MoveSpeedGrow + (grade?.MoveSpeedGrow ?? 0f)) * levelStep
        };
    }
}
