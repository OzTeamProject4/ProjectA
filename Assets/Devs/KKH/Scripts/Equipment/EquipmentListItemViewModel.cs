using System;
using UnityEngine;

public class EquipmentListItemViewModel
{
    private readonly CharacterModel _characterModel;
    private readonly EquipmentInstance _instance;

    private bool _isSelected;

    public string InstanceId
    {
        get { return _instance.InstanceId; }
    }

    public EquipmentInstance Instance
    {
        get { return _instance; }
    }

    public string SpritePath
    {
        get { return _instance.Data.SpritePath; }
    }

    public bool IsEquippedByThisCharacter
    {
        get { return _instance.EquippedBy == _characterModel.CharacterId; }
    }

    public bool IsEquippedByOther
    {
        get { return _instance.IsEquipped && !IsEquippedByThisCharacter; }
    }

    public bool IsSelected
    {
        get { return _isSelected; }
    }

    public event Action OnChanged;

    public EquipmentListItemViewModel(CharacterModel characterModel, EquipmentInstance instance)
    {
        if (null == characterModel || null == instance)
        {
            Debug.LogError("EquipmentListItemViewModel: characterModel 또는 instance 가 null 입니다.");
        }

        _characterModel = characterModel;
        _instance = instance;
    }

    public void Initialize()
    {
        _characterModel.OnEquipmentChanged += HandleChanged;
    }

    public void Dispose()
    {
        _characterModel.OnEquipmentChanged -= HandleChanged;
    }

    public void SetSelected(bool isSelected)
    {
        if (_isSelected == isSelected)
        {
            return;
        }

        _isSelected = isSelected;
        OnChanged?.Invoke();
    }

    private void HandleChanged()
    {
        OnChanged?.Invoke();
    }
}