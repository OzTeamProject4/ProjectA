using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftPopupViewModel
{
    private readonly Inventory _inventory;
    private readonly CharacterModel _characterModel;
    private readonly EquipType _type;

    private readonly List<CraftListItemViewModel> _craftItems = new();
    private readonly List<EquipmentListItemViewModel> _equipmentItems = new();

    public IReadOnlyList<CraftListItemViewModel> CraftItems
    {
        get { return _craftItems; }
    }

    public IReadOnlyList<EquipmentListItemViewModel> EquipmentItems
    {
        get { return _equipmentItems; }
    }

    public event Action<EquipmentListItemViewModel> OnEquipmentItemAdded;

    public CraftPopupViewModel(CraftingModel crafting, Inventory inventory, EquipType type, CharacterModel characterModel)
    {
        if (null == crafting || null == inventory || null == characterModel)
        {
            Debug.LogError("crafting, inventory 또는 characterModel 이 null 입니다.");
            return;
        }

        _inventory = inventory;
        _characterModel = characterModel;
        _type = type;

        foreach (EquipmentData data in crafting.GetCraftableEquipment(type, characterModel.CharacterId))
        {
            _craftItems.Add(new CraftListItemViewModel(crafting, inventory, data));
        }

        foreach (EquipmentInstance instance in inventory.GetEquipmentByType(type))
        {
            _equipmentItems.Add(new EquipmentListItemViewModel(characterModel, instance));
        }
    }

    public void Initialize()
    {
        _inventory.OnEquipmentCreated += HandleEquipmentCreated;
    }

    public void Dispose()
    {
        _inventory.OnEquipmentCreated -= HandleEquipmentCreated;
    }

    private void HandleEquipmentCreated(EquipmentInstance instance)
    {
        if (null == instance || instance.Type != _type)
        {
            return;
        }

        EquipmentListItemViewModel itemViewModel = new EquipmentListItemViewModel(_characterModel, instance);
        _equipmentItems.Add(itemViewModel);

        OnEquipmentItemAdded?.Invoke(itemViewModel);
    }
}