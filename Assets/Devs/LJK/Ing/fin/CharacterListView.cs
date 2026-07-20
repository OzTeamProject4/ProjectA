using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class CharacterListView : BaseUI
{
    [SerializeField] private CharacterSlotView _slotPrefab;
    [SerializeField] private Transform _contentParent;

    private CharacterListViewModel _characterListViewModel;

    private readonly List<CharacterSlotView> _spawnedItems = new List<CharacterSlotView>();

    private void Awake()
    {
        UnityUtil.ValidateReference(_slotPrefab, nameof(CharacterListView), nameof(_slotPrefab));
        UnityUtil.ValidateReference(_contentParent, nameof(CharacterListView), nameof(_contentParent));

        _characterListViewModel = new CharacterListViewModel();
    }

    private void OnEnable()
    {
        _characterListViewModel.ModelPropertyChanged += OnModelPropertyChanged;

        UpdateList();
    }

    private void UpdateList()
    {
        ClearItems();

        foreach (string characterId in _characterListViewModel.CharacterIdList)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                Debug.LogError($"[{nameof(CharacterListView)}:{nameof(UpdateList)}] CharacterId가 null 또는 빈 문자열입니다.");
                return;
            }

            CharacterSlotView slotView = Instantiate(_slotPrefab, _contentParent);
            slotView.Bind(characterId);
            slotView.OnClicked += HandleItemClicked;

            _spawnedItems.Add(slotView);
        }
    }

    private void OnDisable()
    {
        _characterListViewModel.ModelPropertyChanged -= OnModelPropertyChanged;
    }

    private void OnDestroy()
    {
        ClearItems();
    }

    private void OnModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_characterListViewModel.CharacterIdList):
                UpdateList();
                break;
        }
    }

    private void ClearItems()
    {
        foreach (CharacterSlotView item in _spawnedItems)
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
        OpenDetailUI(characterId).Forget();
    }

    private async UniTask OpenDetailUI(string characterId)
    {
        await GameManager.Instance.UIManager.OpenOverlayUIAsync();
        GameManager.Instance.UIManager.OpenCharacterDetailAsync(characterId).Forget();
    }
}