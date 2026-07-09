using System.Collections.Generic;
using UnityEngine;

public class ItemSelectPopupViewModel
{
    private readonly List<ItemSlotViewModel> _items = new();

    public IReadOnlyList<ItemSlotViewModel> Items
    {
        get { return _items; }
    }

    public ItemSelectPopupViewModel(CharacterModel model, IGrowthDataProvider dataProvider)
    {
        if (null == model || null == dataProvider)
        {
            Debug.LogError("model 또는 dataProvider 가 null 입니다.");
            return;
        }

        foreach (string itemId in dataProvider.GetAllExpItemIds())
        {
            ItemData itemData = dataProvider.GetItem(itemId);
            if (null == itemData)
            {
                continue;
            }

            _items.Add(new ItemSlotViewModel(model, itemData));
        }
    }
}
