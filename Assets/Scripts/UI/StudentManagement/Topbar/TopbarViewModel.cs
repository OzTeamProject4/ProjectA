using System;
using System.ComponentModel;

public class TopbarViewModel
{
    private readonly InventoryModel _inventoryModel;

    private MaterialModel _goldMaterial;
    private MaterialModel _crystalMaterial;

    public int GoldCount
    {
        get
        {
            return _inventoryModel.GetItemCount(CurrencyItemId.Gold);
        }
    }

    public int CrystalCount
    {
        get
        {
            return _inventoryModel.GetItemCount(CurrencyItemId.Crystal);
        }
    }

    public string GoldIconKey
    {
        get
        {
            return GetItemIconKey(CurrencyItemId.Gold);
        }
    }

    public string CrystalIconKey
    {
        get
        {
            return GetItemIconKey(CurrencyItemId.Crystal);
        }
    }

    public event Action<string> PropertyChanged;

    public TopbarViewModel()
    {
        _inventoryModel = NetworkManagerTemp.Instance.InventoryModel;

        _inventoryModel.PropertyChanged += OnInventoryChanged;

        SubscribeMaterials();
    }

    private void SubscribeMaterials()
    {
        if (_goldMaterial == null && _inventoryModel.TryGetMaterial(CurrencyItemId.Gold, out MaterialModel goldMaterial))
        {
            _goldMaterial = goldMaterial;
            _goldMaterial.PropertyChanged += OnGoldMaterialChanged;
        }

        if (_crystalMaterial == null && _inventoryModel.TryGetMaterial(CurrencyItemId.Crystal, out MaterialModel crystalMaterial))
        {
            _crystalMaterial = crystalMaterial;
            _crystalMaterial.PropertyChanged += OnCrystalMaterialChanged;
        }
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(GoldCount));
        OnPropertyChanged(nameof(CrystalCount));
        OnPropertyChanged(nameof(GoldIconKey));
        OnPropertyChanged(nameof(CrystalIconKey));
    }

    private static string GetItemIconKey(string itemId)
    {
        if (!GameManager.Instance.DataManager.TryGetData(itemId, out ItemData itemData))
        {
            return string.Empty;
        }

        return itemData.IconKey;
    }

    public void Dispose()
    {
        if (_inventoryModel != null)
        {
            _inventoryModel.PropertyChanged -= OnInventoryChanged;
        }

        if (_goldMaterial != null)
        {
            _goldMaterial.PropertyChanged -= OnGoldMaterialChanged;
            _goldMaterial = null;
        }

        if (_crystalMaterial != null)
        {
            _crystalMaterial.PropertyChanged -= OnCrystalMaterialChanged;
            _crystalMaterial = null;
        }
    }

    private void OnInventoryChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(InventoryModel.Inventory))
        {
            return;
        }

        SubscribeMaterials();

        OnPropertyChanged(nameof(GoldCount));
        OnPropertyChanged(nameof(CrystalCount));
        OnPropertyChanged(nameof(GoldIconKey));
        OnPropertyChanged(nameof(CrystalIconKey));
    }

    private void OnGoldMaterialChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(GoldCount));
    }

    private void OnCrystalMaterialChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CrystalCount));
    }

    private void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(propertyName);
    }
}
