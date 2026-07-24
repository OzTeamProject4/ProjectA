//using UnityEngine;

//public static class EquipmentCalculator
//{
//    public static RolledStats Roll(EquipmentData data)
//    {
//        if (null == data)
//        {
//            Debug.LogWarning("[EquipmentRoller:Roll] data 가 null 입니다.");
//            return default;
//        }

//        if (data.BonusRate <= 0f)
//        {
//            return default;
//        }

//        return new RolledStats
//        {
//            Hp = RollStat(data.MaxHp, data.BonusRate),
//            Atk = RollStat(data.Atk, data.BonusRate),
//            Def = RollStat(data.Def, data.BonusRate),
//            MoveSpeed = RollStat(data.MoveSpeed, data.BonusRate)
//        };
//    }

//    private static float RollStat(float baseStat, float bonusRate)
//    {
//        if (baseStat <= 0f)
//        {
//            return 0f;
//        }

//        float variance = baseStat * bonusRate;
//        return Random.Range(-variance, variance);
//    }
//}
