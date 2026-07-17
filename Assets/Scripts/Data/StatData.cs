namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public readonly struct StatData
{
    public float Hp { get; init; }
    public float Atk { get; init; }
    public float Def { get; init; }
    public float MoveSpeed { get; init; }
}

public readonly struct RolledStats
{
    public float Hp { get; init; }
    public float Atk { get; init; }
    public float Def { get; init; }
    public float MoveSpeed { get; init; }
}

public readonly struct StatDelta
{
    public StatType Type { get; init; }
    public float Value { get; init; }
    public float Delta { get; init; }
    public bool IsInteger { get; init; }
}

public readonly struct StatValue
{
    public StatType Type { get; init; }
    public float Value { get; init; }
    public bool IsInteger { get; init; }
}
