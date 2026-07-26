using System;
using System.Collections.Generic;
using System.ComponentModel;

public class EquipmentCraftSlotViewModel
{
    private EquipmentCraftModel _equipmentCraftModel;

    private readonly List<MaterialModel> _requiredItems = new List<MaterialModel>();

    public EquipmentCraftModel EquipmentCraftModel
    {
        get
        {
            return _equipmentCraftModel;
        }
    }

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

    public bool CanCraft
    {
        get
        {
            return NetworkManagerTemp.Instance.InventoryModel.CanCraftEquipment(_equipmentCraftModel);
        }
    }

    public int GetOwnedItemCount(string itemId)
    {
        return NetworkManagerTemp.Instance.InventoryModel.GetItemCount(itemId);
    }

    public string GetItemTier(string itemId)
    {
        if (!GameManager.Instance.DataManager.TryGetData(itemId, out ItemData itemData))
        {
            return string.Empty;
        }

        if (!GameManager.Instance.DataManager.TryGetData(itemData.ForeignKey, out CurrencyData currencyData))
        {
            return string.Empty;
        }

        return currencyData.Tier;
    }

    public string GetItemIconKey(string itemId)
    {
        if (!GameManager.Instance.DataManager.TryGetData(itemId, out ItemData itemData))
        {
            return string.Empty;
        }

        return itemData.IconKey;
    }


    public void RequestCraftItem()
    {
        if (_equipmentCraftModel == null)
        {
            return;
        }

        NetworkManagerTemp.Instance.InventoryModel.TryCraftEquipment(_equipmentCraftModel);
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