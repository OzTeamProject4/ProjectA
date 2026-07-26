using System;
using System.ComponentModel;

// 상단바 재화 표시용
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

    public event Action<string> PropertyChanged;

    public TopbarViewModel()
    {
        _inventoryModel = NetworkManagerTemp.Instance.InventoryModel;

        if (_inventoryModel.TryGetMaterial(CurrencyItemId.Gold, out MaterialModel goldMaterial))
        {
            _goldMaterial = goldMaterial;
            _goldMaterial.PropertyChanged += OnGoldMaterialChanged;
        }

        if (_inventoryModel.TryGetMaterial(CurrencyItemId.Crystal, out MaterialModel crystalMaterial))
        {
            _crystalMaterial = crystalMaterial;
            _crystalMaterial.PropertyChanged += OnCrystalMaterialChanged;
        }
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(GoldCount));
        OnPropertyChanged(nameof(CrystalCount));
    }

    public void Dispose()
    {
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
