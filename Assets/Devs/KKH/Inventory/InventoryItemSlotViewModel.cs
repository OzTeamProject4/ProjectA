using System;
using System.ComponentModel;

public class InventoryItemSlotViewModel
{
    private ItemModel _itemModel;

    public ItemModel ItemModel
    {
        get
        {
            return _itemModel;
        }
    }

    public string Name
    {
        get
        {
            return _itemModel.Name;
        }
    }

    public string IconKey
    {
        get
        {
            return _itemModel.IconKey;
        }
    }

    // 재료는 스택형이라 개수를 표시하고, 장비는 인스턴스형이라 개수가 없다
    public bool HasCount
    {
        get
        {
            return _itemModel is MaterialModel;
        }
    }

    public int Count
    {
        get
        {
            if (_itemModel is not MaterialModel materialModel)
            {
                return 0;
            }

            return materialModel.Count;
        }
    }

    public event Action<string> PropertyChanged;

    public void SetModel(ItemModel itemModel)
    {
        if (_itemModel != null)
        {
            _itemModel.PropertyChanged -= OnPropertyChanged;
        }

        _itemModel = itemModel;

        if (_itemModel != null)
        {
            _itemModel.PropertyChanged += OnPropertyChanged;
        }
    }

    public void Dispose()
    {
        if (_itemModel != null)
        {
            _itemModel.PropertyChanged -= OnPropertyChanged;
        }

        _itemModel = null;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(e.PropertyName);
    }
}
