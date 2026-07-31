using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDetailView : BaseUI
{
    private readonly List<GameObject> _spawnedSlotList = new List<GameObject>();

    [SerializeField] private TopbarView _topbarView;

    [Header("필터 종류")]
    [SerializeField] private Button _allFilterButton;
    [SerializeField] private Button _equipmentFilterButton;
    [SerializeField] private Button _currencyFilterButton;
    [SerializeField] private Button _materialFilterButton;

    [Header("아이템 리스트")]
    [SerializeField] private Transform _content;
    [SerializeField] private int _slotPrewarmCount = 20;

    private InventoryDetailViewModel _inventoryDetailViewModel;

    private CancellationTokenSource _disableCts;
    private CancellationTokenSource _refreshCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_topbarView, nameof(InventoryDetailView), nameof(_topbarView));
        UnityUtil.ValidateReference(_allFilterButton, nameof(InventoryDetailView), nameof(_allFilterButton));
        UnityUtil.ValidateReference(_equipmentFilterButton, nameof(InventoryDetailView), nameof(_equipmentFilterButton));
        UnityUtil.ValidateReference(_currencyFilterButton, nameof(InventoryDetailView), nameof(_currencyFilterButton));
        UnityUtil.ValidateReference(_materialFilterButton, nameof(InventoryDetailView), nameof(_materialFilterButton));
        UnityUtil.ValidateReference(_content, nameof(InventoryDetailView), nameof(_content));

        _inventoryDetailViewModel = new InventoryDetailViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _topbarView.OnBackClicked += HandleBackClicked;
        _allFilterButton.onClick.AddListener(HandleAllFilterClicked);
        _equipmentFilterButton.onClick.AddListener(HandleEquipmentFilterClicked);
        _currencyFilterButton.onClick.AddListener(HandleCurrencyFilterClicked);
        _materialFilterButton.onClick.AddListener(HandleMaterialFilterClicked);

        _inventoryDetailViewModel.PropertyChanged += OnPropertyChanged;

        InitializeAsync().Forget();
    }

    private void OnDisable()
    {
        _topbarView.OnBackClicked -= HandleBackClicked;
        _allFilterButton.onClick.RemoveListener(HandleAllFilterClicked);
        _equipmentFilterButton.onClick.RemoveListener(HandleEquipmentFilterClicked);
        _currencyFilterButton.onClick.RemoveListener(HandleCurrencyFilterClicked);
        _materialFilterButton.onClick.RemoveListener(HandleMaterialFilterClicked);

        _inventoryDetailViewModel.PropertyChanged -= OnPropertyChanged;

        CancelRefresh();

        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        ReleaseSlots();
    }
    
    private void OnDestroy()
    {
        _inventoryDetailViewModel.Dispose();
        _inventoryDetailViewModel = null;
    }

    private async UniTaskVoid InitializeAsync()
    {
        await GameManager.Instance.ObjectManager.PrewarmAsync(AddressableKey.Prefab.InventoryItemSlot, _slotPrewarmCount, _disableCts.Token);
        _inventoryDetailViewModel.Refresh();
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_inventoryDetailViewModel.DisplayedItems):
                RefreshSlotsAsync().Forget();
                break;
        }
    }

    private async UniTaskVoid RefreshSlotsAsync()
    {
        CancelRefresh();

        _refreshCts = new CancellationTokenSource();
        CancellationToken token = CancellationTokenSource.CreateLinkedTokenSource(_disableCts.Token, _refreshCts.Token).Token;

        ReleaseSlots();

        try
        {
            foreach (ItemModel itemModel in _inventoryDetailViewModel.DisplayedItems)
            {
                GameObject slotObj = await GameManager.Instance.ObjectManager.SpawnAsync(AddressableKey.Prefab.InventoryItemSlot, _content, Vector3.zero, Quaternion.identity, token);

                if (slotObj == null)
                {
                    continue;
                }

                _spawnedSlotList.Add(slotObj);

                if (slotObj.TryGetComponent(out InventoryItemSlotView inventoryItemSlotView))
                {
                    inventoryItemSlotView.SetModel(itemModel);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 스폰 작업 중 취소 발생 시 무시
        }
    }

    private void ReleaseSlots()
    {
        if (GameManager.Instance == null || GameManager.Instance.ObjectManager == null)
        {
            return;
        }

        foreach (GameObject slotObj in _spawnedSlotList)
        {
            if (slotObj == null)
            {
                continue;
            }

            GameManager.Instance.ObjectManager.Despawn(slotObj);
        }

        _spawnedSlotList.Clear();
    }

    private void CancelRefresh()
    {
        if (_refreshCts != null)
        {
            _refreshCts.Cancel();
            _refreshCts.Dispose();
            _refreshCts = null;
        }
    }

    private void HandleAllFilterClicked()
    {
        _inventoryDetailViewModel.SetFilter(InventoryFilterType.All);
    }

    private void HandleEquipmentFilterClicked()
    {
        _inventoryDetailViewModel.SetFilter(InventoryFilterType.Equipment);
    }

    private void HandleCurrencyFilterClicked()
    {
        _inventoryDetailViewModel.SetFilter(InventoryFilterType.Currency);
    }

    private void HandleMaterialFilterClicked()  
    {
        _inventoryDetailViewModel.SetFilter(InventoryFilterType.Material);
    }

    private void HandleBackClicked()
    {
        GameManager.Instance.UIManager.CloseInventoryDetail();
    }
}