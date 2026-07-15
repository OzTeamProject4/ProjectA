using System.Collections.Generic;

public enum ElementType
{
    None,
    Fire,
    Water,
    Grass
}

public enum Role
{
    None,
    Attacker,
    Buffer
}

public class CharacterData : BaseData
{
    public string Name { get; set; }
    public int Star { get; set; }
    public int Level { get; set; }
    public List<string> SkillList { get; set; }
    public ElementType Type { get; set; }
    public Role Role { get; set; }
    public int SkillGauge { get; set; }

    public int Hp { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    public float AtkSpeed { get; set; }
    public float MoveSpeed { get; set; }

    public float HpGrow { get; set; }
    public float AtkGrow { get; set; }
    public float DefGrow { get; set; }
    public float AtkSpeedGrow { get; set; }
    public float MoveSpeedGrow { get; set; }

    public string CharacterIconPath { get; set; }
    public string PrefabPath { get; set; }
}
