using System.Collections.Generic;
using UnityEngine;

public class CraftPopupViewModel
{
    private readonly List<CraftListItemViewModel> _items = new();

    public IReadOnlyList<CraftListItemViewModel> Items
    {
        get { return _items; }
    }

    public CraftPopupViewModel(CraftingModel crafting, Inventory inventory, EquipType type, string characterId)
    {
        if (null == crafting || null == inventory)
        {
            Debug.LogError("crafting 또는 inventory 가 null 입니다.");
            return;
        }

        foreach (EquipmentData data in crafting.GetCraftableEquipment(type, characterId))
        {
            _items.Add(new CraftListItemViewModel(crafting, inventory, data));
        }
    }
}