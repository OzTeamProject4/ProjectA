using System;
using System.Collections.Generic;
using System.ComponentModel;

public class InventoryDetailViewModel
{
    private InventoryModel _inventoryModel;
    private InventoryFilterType _currentFilter = InventoryFilterType.All;

    private readonly List<ItemModel> _displayedItems = new List<ItemModel>();

    public IReadOnlyList<ItemModel> DisplayedItems
    {
        get { return _displayedItems; }
    }

    public InventoryFilterType CurrentFilter
    {
        get { return _currentFilter; }
    }

    public event Action<string> PropertyChanged;

    public InventoryDetailViewModel()
    {
        _inventoryModel = NetworkManager.Instance.InventoryModel;
        _inventoryModel.PropertyChanged += OnInventoryPropertyChanged;
    }

    public void Refresh()
    {
        UpdateDisplayedItems();
    }

    public void Dispose()
    {
        if (_inventoryModel != null)
        {
            _inventoryModel.PropertyChanged -= OnInventoryPropertyChanged;
            _inventoryModel = null;
        }

        _displayedItems.Clear();
    }

    public void SetFilter(InventoryFilterType filterType)
    {
        if (_currentFilter == filterType)
        {
            return;
        }

        _currentFilter = filterType;

        UpdateDisplayedItems();
    }

    private void UpdateDisplayedItems()
    {
        _displayedItems.Clear();

        if (_inventoryModel == null)
        {
            return;
        }

        switch (_currentFilter)
        {
            case InventoryFilterType.All:
                AddItems(_inventoryModel.Inventory.Values);
                AddItems(_inventoryModel.Equipments.Values);
                break;
            case InventoryFilterType.Equipment:
                AddItems(_inventoryModel.Equipments.Values);
                break;
            case InventoryFilterType.Currency:
                AddItems(_inventoryModel.GetItemsByItemType(ItemType.Currency).Values);
                break;
            case InventoryFilterType.Material:
                AddItems(_inventoryModel.GetItemsByItemType(ItemType.Material).Values);
                break;
        }

        NotifyPropertyChanged(nameof(DisplayedItems));
    }

    private void AddItems(IEnumerable<ItemModel> items)
    {
        foreach (ItemModel itemModel in items)
        {
            _displayedItems.Add(itemModel);
        }
    }

    private void OnInventoryPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(InventoryModel.Inventory) && e.PropertyName != nameof(InventoryModel.Equipments))
        {
            return;
        }

        UpdateDisplayedItems();
    }

    private void NotifyPropertyChanged(string propertyName)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(propertyName);
    }
}
