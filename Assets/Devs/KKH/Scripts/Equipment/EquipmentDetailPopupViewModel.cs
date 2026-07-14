using System.Collections.Generic;
using UnityEngine;

public class EquipmentDetailPopupViewModel
{
    private readonly CharacterModel _characterModel;
    private readonly EquipmentInstance _instance;

    public string Name
    {
        get { return _instance.Data.Name; }
    }

    public string Description
    {
        get { return _instance.Data.Description; }
    }

    public string SpritePath
    {
        get { return _instance.Data.SpritePath; }
    }

    public bool IsEquippedByThisCharacter
    {
        get { return _instance.EquippedBy == _characterModel.CharacterId; }
    }

    public bool CanEquip
    {
        get { return _characterModel.CanEquip(_instance); }
    }

    public EquipmentDetailPopupViewModel(CharacterModel characterModel, EquipmentInstance instance)
    {
        if (null == characterModel || null == instance)
        {
            Debug.LogError("EquipmentDetailPopupViewModel: characterModel 또는 instance 가 null 입니다.");
        }

        _characterModel = characterModel;
        _instance = instance;
    }

    public IReadOnlyList<StatDelta> GetStatDelta()
    {
        List<StatDelta> deltas = new();

        EquipmentInstance current = _characterModel.GetEquippedItem(_instance.Type);

        float currentHp = null == current ? 0f : current.TotalHp;
        float currentAtk = null == current ? 0f : current.TotalAtk;
        float currentDef = null == current ? 0f : current.TotalDef;
        float currentAtkSpeed = null == current ? 0f : current.TotalAtkSpeed;
        float currentMoveSpeed = null == current ? 0f : current.TotalMoveSpeed;

        AddBonusStat(deltas, StatType.MaxHp, _instance.TotalHp, currentHp, true);
        AddBonusStat(deltas, StatType.Atk, _instance.TotalAtk, currentAtk, true);
        AddBonusStat(deltas, StatType.Def, _instance.TotalDef, currentDef, true);
        AddBonusStat(deltas, StatType.AtkSpeed, _instance.TotalAtkSpeed, currentAtkSpeed, false);
        AddBonusStat(deltas, StatType.MoveSpeed, _instance.TotalMoveSpeed, currentMoveSpeed, false);

        return deltas;
    }

    public void EquipCommand()
    {
        if (IsEquippedByThisCharacter)
        {
            return;
        }

        _characterModel.Equip(_instance);
    }

    public void UnequipCommand()
    {
        if (!IsEquippedByThisCharacter)
        {
            return;
        }

        _characterModel.Unequip(_instance.Type);
    }

    private void AddBonusStat(List<StatDelta> deltas, StatType type, float selectedValue, float currentValue, bool isInteger)
    {
        if (Mathf.Approximately(selectedValue, 0f) && Mathf.Approximately(currentValue, 0f))
        {
            return;
        }

        deltas.Add(new StatDelta
        {
            Type = type,
            Value = selectedValue,
            Delta = selectedValue - currentValue,
            IsInteger = isInteger
        });
    }
}
