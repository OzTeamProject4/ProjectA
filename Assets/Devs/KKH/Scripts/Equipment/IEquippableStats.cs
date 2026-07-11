public interface IEquippableStats
{
    EquipType Type { get; }
    float MaxHp { get; }
    float Atk { get; }
    float Def { get; }
    float AtkSpeed { get; }
    float Movement { get; }
}