namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public enum ItemType
{
    ExpBook,
    Ticket
}

public class ItemData : BaseData
{
    public string Name { get; init; }
    public ItemType Type { get; init; }
    public int Gold { get; init; }
    public int Crystal { get; init; }
    public int Value { get; init; }
}

public class CharacterStatData : BaseData
{
    public int Hp { get; init; }
    public int Atk { get; init; }
    public int Def { get; init; }
    public float AtkSpeed { get; init; }
    public float MoveSpeed { get; init; }

    public float HpGrow { get; init; }
    public float AtkGrow { get; init; }
    public float DefGrow { get; init; }
    public float AtkSpeedGrow { get; init; }
    public float MoveSpeedGrow { get; init; }
}

public class CharacterGradeData : BaseData
{
    public int Star { get; init; }
    public int MaxLevel { get; init; }
    public int RequiredToNext { get; init; }

    public float HpGrow { get; init; }
    public float AtkGrow { get; init; }
    public float DefGrow { get; init; }
    public float AtkSpeedGrow { get; init; }
    public float MoveSpeedGrow { get; init; }
}

public class LevelExpData : BaseData
{
    public int Level { get; init; }
    public int RequiredExp { get; init; }
}