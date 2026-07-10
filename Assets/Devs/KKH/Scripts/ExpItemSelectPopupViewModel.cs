using System.Collections.Generic;
using UnityEngine;

public class ExpItemSelectPopupViewModel
{
    private readonly List<ExpItemSlotViewModel> _items = new();

    public IReadOnlyList<ExpItemSlotViewModel> Items
    {
        get { return _items; }
    }

    public ExpItemSelectPopupViewModel(CharacterModel model, IGrowthDataProvider dataProvider)
    {
        if (null == model || null == dataProvider)
        {
            Debug.LogError("model 또는 dataProvider 가 null 입니다.");
            return;
        }

        foreach (string dataId in dataProvider.GetAllExpItemIds())
        {
            ItemData itemData = dataProvider.GetItem(dataId);
            if (null == itemData)
            {
                continue;
            }

            _items.Add(new ExpItemSlotViewModel(model, itemData));
        }
    }
}
