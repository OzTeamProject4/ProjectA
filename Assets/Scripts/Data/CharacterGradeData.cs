public class CharacterGradeData : BaseData
{
    public int Star { get; init; }
    public int MaxLevel { get; init; }
    public int RequiredToNext { get; init; }

    public float HpGrow { get; init; }
    public float AtkGrow { get; init; }
    public float DefGrow { get; init; }
    public float MoveSpeedGrow { get; init; }
}
