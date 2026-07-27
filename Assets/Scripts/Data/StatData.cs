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

public readonly struct StatDelta
{
    public StatType Type { get; init; }
    public float Value { get; init; }
    public float Delta { get; init; }
    public bool HasComparison { get; init; }

    public StatDelta(StatType statType, float value, float delta, bool hasComparison)
    {
        Type = statType;
        Value = value;
        Delta = delta;
        HasComparison = hasComparison;
    }
}
