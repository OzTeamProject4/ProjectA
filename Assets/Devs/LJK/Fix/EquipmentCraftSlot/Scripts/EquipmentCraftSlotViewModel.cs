using System;
using System.Collections.Generic;
using System.ComponentModel;

public class EquipmentCraftSlotViewModel
{
    private EquipmentCraftModel _equipmentCraftModel;

    private readonly List<MaterialModel> _requiredItems = new List<MaterialModel>();

    public string DataId
    {
        get
        { 
            return _equipmentCraftModel.DataId;
        }
    }

    public string Name
    {
        get
        { 
            return _equipmentCraftModel.Name;
        }
    }

    public string IconKey
    {
        get 
        {
            return _equipmentCraftModel.IconKey;
        }
    }

    public int RequiredGold
    {
        get 
        {
            return _equipmentCraftModel.RequiredGold; 
        }
    }

    public IReadOnlyList<string> RequiredItemIds
    {
        get
        {
            return _equipmentCraftModel.RequiredItemIds;
        }
    }

    public IReadOnlyList<int> RequiredItemCounts
    {
        get { return _equipmentCraftModel.RequiredItemCounts; }
    }

    public IReadOnlyList<MaterialModel> RequiredItems
    {
        get 
        { 
            return _requiredItems; 
        }
    }

    public event Action<object, string> OnPropertyChange;

    public void SetModel(EquipmentCraftModel equipmentCraftModel)
    {
        _equipmentCraftModel = equipmentCraftModel;

        CacheRequiredMaterials();
    }

    public void Dispose()
    {
        foreach (MaterialModel material in _requiredItems)
        {
            material.PropertyChanged -= OnPropertyChanged;
        }

        if (_equipmentCraftModel == null)
        {
            return;
        }

        _equipmentCraftModel = null;
    }

    //기능 구현
    public void RequestCraftItem()
    {
        throw new NotImplementedException();
    }

    private void CacheRequiredMaterials()
    {
        foreach (MaterialModel material in _requiredItems)
        {
            material.PropertyChanged -= OnPropertyChanged;
        }

        _requiredItems.Clear();
        
        InventoryModel inventoryModel = NetworkManagerTemp.Instance.InventoryModel;
        
        foreach (string requiredItemId in _equipmentCraftModel.RequiredItemIds)
        {
            if (!inventoryModel.TryGetItem(requiredItemId, out ItemModel item))
            {
                continue;
            }
            
            if(item is not MaterialModel materialModel)
            {
                continue;
            }

            _requiredItems.Add(materialModel);
            materialModel.PropertyChanged += OnPropertyChanged;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (OnPropertyChange == null)
        {
            return;
        }

        OnPropertyChange.Invoke(sender, nameof(RequiredItems));
    }
}