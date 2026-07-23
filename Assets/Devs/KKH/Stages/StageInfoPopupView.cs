using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPopupView : BaseUI
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Transform _monsterListContainer;
    [SerializeField] private MonsterListSlotView _monsterItemPrefab;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _blockerButton;
    [SerializeField] private PartySlotButton[] _partySlots;

    private readonly List<MonsterListSlotView> _spawnedItems = new List<MonsterListSlotView>();

    private StageInfoPopupViewModel _viewModel;
    private bool _isSubscribed;

    private PartySelectPopupView _partySelectPopup;

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
        ClearItems();

        _partySelectPopup = null;
    }

    public void Bind(StageInfoPopupViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("[StageInfoPopupView] Bind: viewModel 이 null 입니다.");
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

        if (null != _blockerButton)
        {
            _blockerButton.onClick.AddListener(HandleClickClose);
        }

        if (null != _startButton)
        {
            _startButton.onClick.AddListener(HandleClickStart);
        }

        SubscribeSlots();

        _viewModel.OnPartySelectOpenRequested += HandlePartySelectOpenRequested;
        _viewModel.OnPartySelectCloseRequested += HandlePartySelectCloseRequested;
        _viewModel.OnPartySlotChanged += HandlePartySlotChanged;

        _isSubscribed = true;

        RefreshDisplay();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        if (null != _blockerButton)
        {
            _blockerButton.onClick.RemoveListener(HandleClickClose);
        }

        if (null != _startButton)
        {
            _startButton.onClick.RemoveListener(HandleClickStart);
        }

        UnsubscribeSlots();

        if (null != _viewModel)
        {
            _viewModel.OnPartySelectOpenRequested -= HandlePartySelectOpenRequested;
            _viewModel.OnPartySelectCloseRequested -= HandlePartySelectCloseRequested;
            _viewModel.OnPartySlotChanged -= HandlePartySlotChanged;
        }

        _isSubscribed = false;
    }

    private void RefreshDisplay()
    {
        if (null == _viewModel)
        {
            return;
        }

        if (null != _nameText)
        {
            _nameText.text = _viewModel.StageName;
        }

        RefreshMonsterList();
    }

    private void RefreshMonsterList()
    {
        ClearItems();

        if (null == _monsterListContainer || null == _monsterItemPrefab)
        {
            return;
        }

        foreach (string monsterId in _viewModel.GetMonsterIds())
        {
            MonsterListSlotView item = Instantiate(_monsterItemPrefab, _monsterListContainer);
            item.Bind(monsterId);

            _spawnedItems.Add(item);
        }
    }

    private void ClearItems()
    {
        foreach (MonsterListSlotView item in _spawnedItems)
        {
            if (null == item)
            {
                continue;
            }

            Destroy(item.gameObject);
        }

        _spawnedItems.Clear();
    }

    // ===== 캐릭터 슬롯 =====

    private void SubscribeSlots()
    {
        if (null == _partySlots)
        {
            return;
        }

        foreach (PartySlotButton slot in _partySlots)
        {
            if (null == slot)
            {
                continue;
            }

            slot.OnClicked -= HandleSlotClicked;
            slot.OnClicked += HandleSlotClicked;
        }
    }

    private void UnsubscribeSlots()
    {
        if (null == _partySlots)
        {
            return;
        }

        foreach (PartySlotButton slot in _partySlots)
        {
            if (null == slot)
            {
                continue;
            }

            slot.OnClicked -= HandleSlotClicked;
        }
    }

    private void HandleSlotClicked(int slotIndex)
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.SelectSlotCommand(slotIndex);
    }

    private void HandlePartySlotChanged(int slotIndex)
    {
        RefreshSlotIconAsync(slotIndex).Forget();
    }

    private async UniTaskVoid RefreshSlotIconAsync(int slotIndex)
    {
        PartySlotButton slot = FindSlot(slotIndex);

        if (null == slot)
        {
            return;
        }

        string iconPath = _viewModel.GetSlotIconPath(slotIndex);

        if (string.IsNullOrEmpty(iconPath))
        {
            slot.SetIcon(null);
            return;
        }

        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconPath, destroyCancellationToken);

        slot.SetIcon(sprite);
    }

    private PartySlotButton FindSlot(int slotIndex)
    {
        if (null == _partySlots)
        {
            return null;
        }

        foreach (PartySlotButton slot in _partySlots)
        {
            if (null == slot)
            {
                continue;
            }

            if (slot.Index == slotIndex)
            {
                return slot;
            }
        }

        return null;
    }

    // ===== 캐릭터 리스트 팝업 =====

    private void HandlePartySelectOpenRequested(PartySelectPopupViewModel selectViewModel)
    {
        OpenPartySelectPopupAsync(selectViewModel).Forget();
    }

    private async UniTaskVoid OpenPartySelectPopupAsync(PartySelectPopupViewModel selectViewModel)
    {
        _partySelectPopup = await GameManager.Instance.UIManager.OpenPartySelectPopupAsync();

        if (null == _partySelectPopup)
        {
            Debug.LogError("[StageInfoPopupView] PartySelectPopupView 를 열지 못했습니다.");
            return;
        }

        _partySelectPopup.Bind(selectViewModel);
    }

    private void HandlePartySelectCloseRequested()
    {
        if (null == _partySelectPopup)
        {
            return;
        }

        GameManager.Instance.UIManager.ClosePartySelectPopup();

        _partySelectPopup = null;
    }

    private void HandleClickStart()
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.StartBattleCommand();
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
