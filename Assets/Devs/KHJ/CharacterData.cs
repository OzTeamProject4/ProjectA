using System.Collections.Generic;

public enum Role
{
    None,
    Attacker,
    Buffer
}

public class CharacterData : BaseData
{
    public string Name { get; set; }
    public int InitialStar { get; set; }
    public int Level { get; set; }
    public int BaseHp { get; set; }
    public int SkillGauge { get; set; }
    public List<string> SkillList { get; set; }
    public int BaseAtk { get; set; }
    public int BaseDef { get; set; }
    public float BaseAtkSpeed { get; set; }
    public float BaseMoveSpeed { get; set; }
    public float HpGrow { get; set; }
    public float AtkGrow { get; set; }
    public float DefGrow { get; set; }
    public float AtkSpeedGrow { get; set; }
    public float MoveSpeedGrow { get; set; }
    public string CharacterIconPath { get; set; }
    public string PrefabPath { get; set; }
    public string Description { get; set; }
    public ElementType ElementType { get; set; }
    public Role Role { get; set; }
}
