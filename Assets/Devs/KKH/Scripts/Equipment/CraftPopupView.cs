using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftPopupView : BaseUI
{
    [SerializeField] private CraftListItemView _itemPrefab;
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private Button _closeButton;

    private readonly List<CraftListItemView> _spawnedItems = new();
    private readonly List<string> _loadedSpriteKeys = new();

    public event Action<string> OnCrafted;
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
    }

    private void OnDestroy()
    {
        ClearItems();
        ReleaseAllSprites();
    }

    public void Bind(CraftPopupViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: CraftPopupViewModel 이 null 입니다.");
            return;
        }

        if (null == _itemPrefab || null == _itemContainer)
        {
            Debug.LogError("Bind: 아이템 프리팹 또는 컨테이너가 연결되지 않았습니다.");
            return;
        }

        ClearItems();
        ReleaseAllSprites();

        foreach (CraftListItemViewModel itemViewModel in viewModel.CraftItems)
        {
            CraftListItemView itemView = Instantiate(_itemPrefab, _itemContainer);
            itemView.Bind(itemViewModel);
            itemView.OnCrafted += HandleCrafted;
            itemView.OnSpriteLoaded += HandleSpriteLoaded;

            _spawnedItems.Add(itemView);
        }
    }

    private void ClearItems()
    {
        foreach (CraftListItemView item in _spawnedItems)
        {
            if (null == item)
            {
                continue;
            }

            item.OnCrafted -= HandleCrafted;
            item.OnSpriteLoaded -= HandleSpriteLoaded;
            Destroy(item.gameObject);
        }

        _spawnedItems.Clear();
    }

    private void HandleCrafted(string dataId)
    {
        OnCrafted?.Invoke(dataId);
    }

    private void HandleSpriteLoaded(string spritePath)
    {
        _loadedSpriteKeys.Add(spritePath);
    }

    private void HandleCloseClicked()
    {
        OnCloseButtonClicked?.Invoke();
    }

    private void ReleaseAllSprites()
    {
        foreach (string key in _loadedSpriteKeys)
        {
            GameManager.Instance.ResourceManager.ReleaseAsset(key);
        }

        _loadedSpriteKeys.Clear();
    }
}