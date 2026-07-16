
public enum CharacterSkillCategory
{
    Basic,
    Normal,
    Ultimate
}

public enum CharacterSkillType
{
    SingleAttack,
    AreaAttack,
    HealBuff
}
public class CharacterSkillData : BaseData
{
    public string Name { get; set; }
    public CharacterSkillCategory Category { get; set; }
    public CharacterSkillType Type { get; set; }
    public float Cooldown { get; set; }
    public int GaugeRecovery { get; set; }
    public string PrefabPath { get; set; }
    public float DamageCoefficient { get; set; }
    public float ProjectileSpeed { get; set; }
    public float AreaRadius { get; set; }
    public int HealAmount { get; set; }
    public float MoveSpeedBuff { get; set; }
    public float BuffDuration { get; set; }

}
