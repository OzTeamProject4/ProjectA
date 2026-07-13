using UnityEngine;

public class EquipmentInstance
{
    public string InstanceId { get; }
    public EquipmentData Data { get; }
    public RolledStats RolledStat { get; }

    public string EquippedBy { get; private set; }

    public EquipType Type
    {
        get { return Data.Type; }
    }

    public bool IsEquipped
    {
        get { return !string.IsNullOrEmpty(EquippedBy); }
    }

    public float TotalHp 
    { 
        get { return Data.MaxHp + RolledStat.Hp; }
    }

    public float TotalAtk
    { 
        get { return Data.Atk + RolledStat.Atk; } 
    }

    public float TotalDef 
    { 
        get { return Data.Def + RolledStat.Def; } 
    }

    public float TotalAtkSpeed
    { 
        get { return Data.AtkSpeed + RolledStat.AtkSpeed; } 
    }

    public float TotalMoveSpeed 
    {
        get { return Data.MoveSpeed + RolledStat.MoveSpeed; }
    }

    public EquipmentInstance(string instanceId, EquipmentData data, RolledStats rolledStat)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            Debug.LogError("EquipmentInstance: instanceId 가 비어 있습니다.");
        }

        if (null == data)
        {
            Debug.LogError("EquipmentInstance: data 가 null 입니다.");
        }

        InstanceId = instanceId;
        Data = data;
        RolledStat = rolledStat;
    }

    public void SetEquippedBy(string characterId)
    {
        EquippedBy = characterId;
    }

    public void ClearEquipped()
    {
        EquippedBy = null;
    }
}