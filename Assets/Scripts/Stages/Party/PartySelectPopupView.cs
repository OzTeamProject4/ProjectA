using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySelectPopupView : BaseUI
{
    [SerializeField] private PartySelectSlotView _itemPrefab;
    [SerializeField] private Transform _contentParent;
    [SerializeField] private Button _closeButton;

    private readonly List<PartySelectSlotView> _spawnedItems = new List<PartySelectSlotView>();

    private PartySelectPopupViewModel _viewModel;
    private bool _isSubscribed;

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
        ClearItems();
    }

    public void Bind(PartySelectPopupViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("[PartySelectPopupView] Bind: viewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();

        _viewModel = viewModel;

        Subscribe();
    }

    private void Subscribe()
    {
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        if (null != _closeButton)
        {
            _closeButton.onClick.AddListener(HandleClickClose);
        }

        _isSubscribed = true;

        RefreshItems();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        if (null != _closeButton)
        {
            _closeButton.onClick.RemoveListener(HandleClickClose);
        }

        UnsubscribeItems();

        _isSubscribed = false;
    }

    private void RefreshItems()
    {
        ClearItems();

        if (null == _itemPrefab || null == _contentParent)
        {
            Debug.LogError("[PartySelectPopupView] 아이템 프리팹 또는 컨테이너가 연결되지 않았습니다.");
            return;
        }

        foreach (PartySelectSlotViewModel itemViewModel in _viewModel.Items)
        {
            PartySelectSlotView itemView = Instantiate(_itemPrefab, _contentParent);
            itemView.Bind(itemViewModel);

            itemView.OnClicked -= HandleItemClicked;
            itemView.OnClicked += HandleItemClicked;

            _spawnedItems.Add(itemView);
        }
    }

    private void UnsubscribeItems()
    {
        foreach (PartySelectSlotView item in _spawnedItems)
        {
            if (null == item)
            {
                continue;
            }

            item.OnClicked -= HandleItemClicked;
        }
    }

    private void ClearItems()
    {
        foreach (PartySelectSlotView item in _spawnedItems)
        {
            if (null == item)
            {
                continue;
            }

            item.OnClicked -= HandleItemClicked;
            Destroy(item.gameObject);
        }

        _spawnedItems.Clear();
    }

    private void HandleItemClicked(string characterId)
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.SelectCommand(characterId);
    }

    private void HandleClickClose()
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.CloseCommand();
    }
}
