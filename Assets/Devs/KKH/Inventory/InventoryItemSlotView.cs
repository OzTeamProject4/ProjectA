using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlotView : MonoBehaviour
{
    private const string ItemCountFormat = "x {0}";

    [SerializeField] private Image _itemIconImage;
    [SerializeField] private TMP_Text _itemCountText;

    private InventoryItemSlotViewModel _inventoryItemSlotViewModel;

    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_itemIconImage, nameof(InventoryItemSlotView), nameof(_itemIconImage));
        UnityUtil.ValidateReference(_itemCountText, nameof(InventoryItemSlotView), nameof(_itemCountText));

        _inventoryItemSlotViewModel = new InventoryItemSlotViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _inventoryItemSlotViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnDisable()
    {
        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        _inventoryItemSlotViewModel.PropertyChanged -= OnPropertyChanged;
    }

    private void OnDestroy()
    {
        _inventoryItemSlotViewModel.Dispose();
        _inventoryItemSlotViewModel = null;
    }

    public void SetModel(ItemModel itemModel)
    {
        _inventoryItemSlotViewModel.SetModel(itemModel);

        RefreshView();
    }

    private void RefreshView()
    {
        UpdateIconImageAsync().Forget();
        RefreshCountText();
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_inventoryItemSlotViewModel.IconKey):
                UpdateIconImageAsync().Forget();
                break;
            case nameof(_inventoryItemSlotViewModel.Count):
            case nameof(_inventoryItemSlotViewModel.HasCount):
                RefreshCountText();
                break;
        }
    }

    private void RefreshCountText()
    {
        bool hasCount = _inventoryItemSlotViewModel.HasCount;

        _itemCountText.gameObject.SetActive(hasCount);

        if (!hasCount)
        {
            return;
        }

        _itemCountText.text = string.Format(ItemCountFormat, _inventoryItemSlotViewModel.Count);
    }

    private UniTask UpdateIconImageAsync()
    {
        return SpriteLoader.LoadIntoAsync(_itemIconImage, _inventoryItemSlotViewModel.IconKey, _disableCts.Token);
    }
}
