//using System;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using UnityEngine;

//public class CharacterListView : BaseUI
//{
//    [SerializeField] private CharacterListItemView _itemPrefab;
//    [SerializeField] private Transform _contentParent;
//    [SerializeField] private Button _btnClose;

//    private readonly List<CharacterListItemView> _spawnedItems = new();

//    public event Action<string> OnItemSelected;
//    public event Action OnCloseButtonClicked;

//    private void OnEnable()
//    {
//        if (null != _btnClose)
//        {
//            _btnClose.onClick.AddListener(HandleCloseClicked);
//        }
//    }

//    private void OnDisable()
//    {
//        if (null != _btnClose)
//        {
//            _btnClose.onClick.RemoveListener(HandleCloseClicked);
//        }
//    }

//    private void OnDestroy()
//    {
//        ClearItems();
//    }

//    public void Bind(CharacterListViewModel viewModel, IReadOnlyList<CharacterDisplayInfo> displayInfos)
//    {
//        if (null == viewModel)
//        {
//            Debug.LogError("Bind: CharacterListViewModel 이 null 입니다.");
//            return;
//        }

//        if (null == _itemPrefab || null == _contentParent)
//        {
//            Debug.LogError("Bind: 아이템 프리팹 또는 ContentParent 가 연결되지 않았습니다.");
//            return;
//        }

//        ClearItems();

//        Dictionary<string, CharacterDisplayInfo> infoLookup = BuildDisplayInfoLookup(displayInfos);

//        foreach (CharacterListItemViewModel itemViewModel in viewModel.Items)
//        {
//            if (!infoLookup.TryGetValue(itemViewModel.DataId, out CharacterDisplayInfo displayInfo))
//            {
//                Debug.LogWarning($"CharacterDisplayInfo 를 찾을 수 없습니다. DataId={itemViewModel.DataId}");
//                continue;
//            }

//            CharacterListItemView itemView = Instantiate(_itemPrefab, _contentParent);
//            itemView.Bind(itemViewModel, displayInfo);
//            itemView.OnClicked += HandleItemClicked;

//            _spawnedItems.Add(itemView);
//        }
//    }

//    private void ClearItems()
//    {
//        foreach (CharacterListItemView item in _spawnedItems)
//        {
//            if (null == item)
//            {
//                continue;
//            }

//            item.OnClicked -= HandleItemClicked;
//            Destroy(item.gameObject);
//        }

//        _spawnedItems.Clear();
//    }

//    private Dictionary<string, CharacterDisplayInfo> BuildDisplayInfoLookup(IReadOnlyList<CharacterDisplayInfo> displayInfos)
//    {
//        Dictionary<string, CharacterDisplayInfo> lookup = new();

//        if (null == displayInfos)
//        {
//            return lookup;
//        }

//        foreach (CharacterDisplayInfo info in displayInfos)
//        {
//            lookup[info.DataId] = info;
//        }

//        return lookup;
//    }

//    private void HandleItemClicked(string characterId)
//    {
//        OnItemSelected?.Invoke(characterId);
//    }

//    private void HandleCloseClicked()
//    {
//        OnCloseButtonClicked?.Invoke();
//    }
//}