using UnityEngine;

public class EnemyData : BaseData
{
    public string Name { get; set; }

    public int TotalExp { get; set; }

    public ElementType ElementalType { get; set; }

    public int BaseHp { get; set; }

    public int BaseDamage { get; set; }
    public string PrefabAddress { get; set; }
    public string SkillDataId { get; set; }

}
