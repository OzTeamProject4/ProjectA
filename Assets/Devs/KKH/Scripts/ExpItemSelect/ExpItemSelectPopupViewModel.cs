using System.Collections.Generic;
using UnityEngine;

public class ExpItemSelectPopupViewModel
{
    private readonly List<ExpItemSlotViewModel> _items = new();

    public IReadOnlyList<ExpItemSlotViewModel> Items
    {
        get { return _items; }
    }

    public ExpItemSelectPopupViewModel(CharacterModel model, Inventory inventory)
    {
        if (null == model || null == inventory)
        {
            Debug.LogError("model 또는 inventory 가 null 입니다.");
            return;
        }

        foreach (ItemData itemData in model.GetExpItems())
        {
            _items.Add(new ExpItemSlotViewModel(model, inventory, itemData));
        }
    }
}