using System.Collections.Generic;



namespace KHJ
{
    public enum ElementType
    {
        None,
        Fire,
        Water,
        Thunder,
        Light,
        Dark
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
        public int Level { get; set; }
        public int BaseHp { get; set; }
        public int SkillGauge { get; set; }
        public List<string> SkillList { get; set; }
        public int BaseAtk { get; set; }
        public int BaseDef { get; set; }
        public float BaseMoveSpeed { get; set; }
        public ElementType ElementType { get; set; }
        public Role Role { get; set; }
    }
}

