public readonly struct StatInfo
{
    public StatType Type { get; init; }
    public float Value { get; init; }

    public StatInfo(StatType statType, float value)
    {
        Type = statType;
        Value = value;
    }
}

// 장착 시 스탯 증감 표시 기능에서 사용 예정
//public readonly struct StatDelta
//{
//    public StatType Type { get; init; }
//    public float Value { get; init; }
//    public float Delta { get; init; }
//    public bool IsInteger { get; init; }
//}

//public readonly struct StatValue
//{
//    public StatType Type { get; init; }
//    public float Value { get; init; }
//    public bool IsInteger { get; init; }
//}
