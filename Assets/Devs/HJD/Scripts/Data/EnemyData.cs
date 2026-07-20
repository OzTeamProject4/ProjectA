using UnityEngine;

public class EnemyData : BaseData
{
    public string Name { get; set; }

    public int TotalExp { get; set; }

    public string ElementalType { get; set; }

    public int BaseHp { get; set; }

    public int BaseDamage { get; set; }
    public string PrefabAddress { get; set; }
    public string SkillPrefabAddress { get; set; }

}
