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
