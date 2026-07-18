using System;

[Serializable]
public sealed class EnemyData
{
    // JSON 적 식별 정보
    public string DataId;
    public string Name;

    // 적 보상과 속성 정보
    public int TotalExp;
    public string ElementalType;

    // 적 기본 전투 능력치
    public int BaseHp;
    public int BaseDamage;

    // Addressables 프리팹 주소
    public string PrefabAddress;
}
