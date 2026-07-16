using UnityEngine;

public static class ElementDamageCalculator
{
    // 플레이어 → 적
    public static int ApplyElementModifier(int damageBeforeElement, BattleCharacter attacker, EnemyData target)
    {
        return ApplyElementModifier(damageBeforeElement, attacker.ElementType, target.ElementalType);
    }

    // 적 → 플레이어
    public static int ApplyElementModifier(int damageBeforeElement, EnemyData attacker, BattleCharacter target)
    {
        return ApplyElementModifier(damageBeforeElement, attacker.ElementalType, target.ElementType);
    }

    // 속성에 따른 최종 데미지 보정
    public static int ApplyElementModifier(int damageBeforeElement, ElementType attackElement, ElementType targetElement)
    {
        if (damageBeforeElement <= 0)
        {
            return 0;
        }

        float multiplier = GetElementMultiplier(attackElement, targetElement);

        return Mathf.RoundToInt(damageBeforeElement * multiplier);
    }

    private static float GetElementMultiplier(ElementType attackElement, ElementType targetElement)
    {
        // 한쪽이 무상성이거나 타입이 같다면?
        if (attackElement == ElementType.None || targetElement == ElementType.None || attackElement == targetElement)
        {
            return 1f;
        }

        // 상성
        bool isAdvantage =
            (attackElement == ElementType.Fire && targetElement == ElementType.Grass) ||

            (attackElement == ElementType.Water && targetElement == ElementType.Fire) ||

            (attackElement == ElementType.Grass && targetElement == ElementType.Water);

        // 역상성
        bool isDisadvantage =
            (attackElement == ElementType.Fire && targetElement == ElementType.Water) ||

            (attackElement == ElementType.Water && targetElement == ElementType.Grass) ||

            (attackElement == ElementType.Grass && targetElement == ElementType.Fire);

        if (isAdvantage)
        {
            return 1.2f;
        }

        if (isDisadvantage)
        {
            return 0.8f;
        }

        return 1f;
    }
}