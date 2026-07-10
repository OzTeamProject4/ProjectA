using System.Collections.Generic;
using UnityEngine;

public class ExpItemSelectPopupViewModel
{
    private readonly List<ExpItemSlotViewModel> _items = new();

    public IReadOnlyList<ExpItemSlotViewModel> Items
    {
        get { return _items; }
    }

    public ExpItemSelectPopupViewModel(CharacterModel model)
    {
        if (null == model)
        {
            Debug.LogError("model 이 null 입니다.");
            return;
        }

        foreach (ItemData itemData in model.GetExpItems())
        {
            _items.Add(new ExpItemSlotViewModel(model, itemData));
        }
    }
}
