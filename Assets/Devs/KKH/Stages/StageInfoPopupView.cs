using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPopupView : BaseUI
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Transform _monsterListContainer;
    [SerializeField] private MonsterListItemView _monsterItemPrefab;
    [SerializeField] private Button _partySettingButton;
    [SerializeField] private Button _blockerButton;

    private readonly List<MonsterListItemView> _spawnedItems = new List<MonsterListItemView>();

    private StageInfoPopupViewModel _viewModel;
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

        if (null != _partySettingButton)
        {
            _partySettingButton.onClick.AddListener(HandleClickPartySetting);
        }

        if (null != _blockerButton)
        {
            _blockerButton.onClick.AddListener(HandleClickClose);
        }

        _isSubscribed = true;

        RefreshDisplay();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        if (null != _partySettingButton)
        {
            _partySettingButton.onClick.RemoveListener(HandleClickPartySetting);
        }

        if (null != _blockerButton)
        {
            _blockerButton.onClick.RemoveListener(HandleClickClose);
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
            MonsterListItemView item = Instantiate(_monsterItemPrefab, _monsterListContainer);
            item.Bind(monsterId);

            _spawnedItems.Add(item);
        }
    }

    private void ClearItems()
    {
        foreach (MonsterListItemView item in _spawnedItems)
        {
            if (null == item)
            {
                continue;
            }

            Destroy(item.gameObject);
        }

        _spawnedItems.Clear();
    }

    private void HandleClickPartySetting()
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.PartySetupCommand();
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