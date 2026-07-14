using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentListPopupView : BaseUI
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private EquipmentListItemView _itemPrefab;
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private Button _closeButton;

    private readonly List<EquipmentListItemView> _spawnedItems = new();

    private EquipmentListPopupViewModel _viewModel;

    public event Action<EquipmentListItemViewModel, RectTransform> OnItemSelected;
    public event Action OnCloseButtonClicked;

    private void OnEnable()
    {
        if (null != _closeButton)
        {
            _closeButton.onClick.AddListener(HandleCloseClicked);
        }
    }

    private void OnDisable()
    {
        if (null != _closeButton)
        {
            _closeButton.onClick.RemoveListener(HandleCloseClicked);
        }

        Unbind();
    }

    private void OnDestroy()
    {
        Unbind();
        ClearItems();
    }

    public void Bind(EquipmentListPopupViewModel viewModel, string slotName)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: EquipmentListPopupViewModel 이 null 입니다.");
            return;
        }

        if (null == _itemPrefab || null == _itemContainer)
        {
            Debug.LogError("Bind: 아이템 프리팹 또는 컨테이너가 연결되지 않았습니다.");
            return;
        }

        Unbind();
        ClearItems();

        _viewModel = viewModel;
        _viewModel.OnItemAdded += HandleItemAdded;
        _viewModel.Initialize();

        if (null != _nameText)
        {
            _nameText.text = slotName;
        }

        foreach (EquipmentListItemViewModel itemViewModel in _viewModel.Items)
        {
            SpawnItem(itemViewModel);
        }
    }

    public void ClearSelection()
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.ClearSelection();
    }

    private void Unbind()
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.OnItemAdded -= HandleItemAdded;
        _viewModel.Dispose();
        _viewModel = null;
    }

    private void HandleItemAdded(EquipmentListItemViewModel itemViewModel)
    {
        if (null == itemViewModel)
        {
            return;
        }

        SpawnItem(itemViewModel);
    }

    private void SpawnItem(EquipmentListItemViewModel itemViewModel)
    {
        EquipmentListItemView itemView = Instantiate(_itemPrefab, _itemContainer);
        itemView.Bind(itemViewModel);
        itemView.OnItemClicked += HandleItemClicked;

        _spawnedItems.Add(itemView);
    }

    private void ClearItems()
    {
        foreach (EquipmentListItemView item in _spawnedItems)
        {
            if (null == item)
            {
                continue;
            }

            item.OnItemClicked -= HandleItemClicked;
            Destroy(item.gameObject);
        }

        _spawnedItems.Clear();
    }

    private void HandleItemClicked(EquipmentListItemViewModel itemViewModel, RectTransform itemRect)
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.SelectItem(itemViewModel);

        OnItemSelected?.Invoke(itemViewModel, itemRect);
    }

    private void HandleCloseClicked()
    {
        OnCloseButtonClicked?.Invoke();
    }
}