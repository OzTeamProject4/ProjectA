using System;

[Serializable]
public sealed class EnemyData
{
    // JSON 적 식별 정보
    public string DataId { get; set; }
    public string Name { get; set; }

    // 적 보상과 속성 정보
    public int TotalExp { get; set; }
    public ElementType ElementalType { get; set; }

    // 적 기본 전투 능력치
    public int BaseHp { get; set; }
    public int BaseDamage { get; set; }

    // Addressables 프리팹 주소
    public string PrefabAddress { get; set; }
}