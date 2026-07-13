using UnityEngine;

public class EquipmentInstance
{
    public string InstanceId { get; }
    public EquipmentData Data { get; }

    public float RolledHp { get; }
    public float RolledAtk { get; }
    public float RolledDef { get; }
    public float RolledAtkSpeed { get; }
    public float RolledMoveSpeed { get; }

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
        get { return Data.MaxHp + RolledHp; }
    }

    public float TotalAtk
    { 
        get { return Data.Atk + RolledAtk; } 
    }

    public float TotalDef 
    { 
        get { return Data.Def + RolledDef; } 
    }

    public float TotalAtkSpeed
    { 
        get { return Data.AtkSpeed + RolledAtkSpeed; } 
    }

    public float TotalMoveSpeed 
    {
        get { return Data.MoveSpeed + RolledMoveSpeed; }
    }

    public EquipmentInstance(string instanceId, EquipmentData data, float rolledHp, float rolledAtk, float rolledDef, float rolledAtkSpeed, float rolledMoveSpeed)
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
        RolledHp = rolledHp;
        RolledAtk = rolledAtk;
        RolledDef = rolledDef;
        RolledAtkSpeed = rolledAtkSpeed;
        RolledMoveSpeed = rolledMoveSpeed;
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