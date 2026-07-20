using System;
using System.Collections.Generic;

[Serializable]
public class CharacterSaveData
{
    public string CharacterId;
    public int CurrentStar;
    public int CurrentLevel;
    public int CurrentExp;
    public int OwnedDuplicates;
}

[Serializable]
public class EquipmentSaveData
{
    public string InstanceId;
    public string DataId;
    public string EquippedBy;
    public float RolledHp;
    public float RolledAtk;
    public float RolledDef;
    public float RolledMoveSpeed;
}

[Serializable]
public class GrowthSaveData
{
    public int Gold;
    public List<CharacterSaveData> Characters = new();
    public List<EquipmentSaveData> Equipments = new();
    public Dictionary<string, int> ItemCounts = new();
}