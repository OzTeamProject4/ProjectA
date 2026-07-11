using UnityEngine;

public class EquipmentInstance
{
    public string InstanceId { get; }
    public IEquippableStats Data { get; }

    public float BonusHp { get; }
    public float BonusAtk { get; }
    public float BonusDef { get; }
    public float BonusAtkSpeed { get; }
    public float BonusMovement { get; }

    public string EquippedBy { get; private set; }

    public EquipType Type
    {
        get { return Data.Type; }
    }

    public bool IsEquipped
    {
        get { return !string.IsNullOrEmpty(EquippedBy); }
    }

    public float TotalHp { get { return Data.MaxHp + BonusHp; } }
    public float TotalAtk { get { return Data.Atk + BonusAtk; } }
    public float TotalDef { get { return Data.Def + BonusDef; } }
    public float TotalAtkSpeed { get { return Data.AtkSpeed + BonusAtkSpeed; } }
    public float TotalMovement { get { return Data.Movement + BonusMovement; } }

    public EquipmentInstance(string instanceId, IEquippableStats data,
        float bonusHp, float bonusAtk, float bonusDef, float bonusAtkSpeed, float bonusMovement)
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
        BonusHp = bonusHp;
        BonusAtk = bonusAtk;
        BonusDef = bonusDef;
        BonusAtkSpeed = bonusAtkSpeed;
        BonusMovement = bonusMovement;
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