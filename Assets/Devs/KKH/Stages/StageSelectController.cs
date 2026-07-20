using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageSelectController
{
    private readonly StageMonsterParty _partyPrefab;
    private readonly StagePlayerParty _playerParty;
    private readonly StageSelectMap _map;
    private readonly StageProgressModel _progressModel;
    private readonly IGameDataProvider _dataProvider;
    private readonly ScreenStateModel _screenStateModel;

    private readonly List<StageMonsterParty> _spawnedParties = new List<StageMonsterParty>();

    private StageInfoPopupView _stageInfoPopup;
    private PartySetupPopupView _partySetupPopup;

    public StageSelectController(StageMonsterParty partyPrefab, StageSelectMap map, StageProgressModel progressModel,
         IGameDataProvider dataProvider, ScreenStateModel screenStateModel, StagePlayerParty playerParty)
    {
        if (null == partyPrefab || null == map || null == progressModel || null == dataProvider || null == screenStateModel || null == playerParty)
        {
            Debug.LogError("[StageSelectController] 생성자 인자 중 null 이 있습니다.");
        }

        _partyPrefab = partyPrefab;
        _map = map;
        _progressModel = progressModel;
        _dataProvider = dataProvider;
        _screenStateModel = screenStateModel;
        _playerParty = playerParty;
    }

    public void SpawnParties()
    {
        if (null == _map || null == _partyPrefab)
        {
            return;
        }

        foreach (MonsterPartySpawner spawner in _map.Spawners)
        {
            if (null == spawner)
            {
                continue;
            }

            SpawnPartyAt(spawner);
        }
    }

    public void Dispose()
    {
        foreach (StageMonsterParty party in _spawnedParties)
        {
            if (null == party)
            {
                continue;
            }

            party.OnPlayerReached -= HandlePlayerReached;
            party.OnPlayerLeft -= HandlePlayerLeft;

            Object.Destroy(party.gameObject);   
        }

        _spawnedParties.Clear();

        UnsubscribeStageInfoPopup();
        UnsubscribePartySetupPopup();
    }

    private void SpawnPartyAt(MonsterPartySpawner spawner)
    {
        StageMonsterParty party = Object.Instantiate(_partyPrefab, spawner.SpawnPosition, Quaternion.identity, _map.transform);

        party.SetStageId(spawner.StageId);

        party.OnPlayerReached -= HandlePlayerReached;
        party.OnPlayerLeft -= HandlePlayerLeft;
        party.OnPlayerReached += HandlePlayerReached;
        party.OnPlayerLeft += HandlePlayerLeft;

        _spawnedParties.Add(party);
    }

    // ===== 스테이지 정보 팝업 =====

    private void HandlePlayerReached(string stageId)
    {
        _progressModel.SelectStage(stageId);

        if (null != _playerParty)
        {
            _playerParty.StopMove();
        }

        OpenStageInfoPopupAsync(stageId).Forget();
    }

    private void HandlePlayerLeft(string stageId)
    {
        if (_progressModel.SelectedStageId != stageId)
        {
            return;
        }

        CloseStageInfoPopup();

        if (null != _playerParty)
        {
            _playerParty.ResumeMove();
        }
    }

    private async UniTaskVoid OpenStageInfoPopupAsync(string stageId)
    {
        StageData stageData = _dataProvider.GetStage(stageId);

        if (null == stageData)
        {
            Debug.LogWarning($"[StageSelectController] StageData 를 찾을 수 없습니다. stageId={stageId}");
            return;
        }

        _stageInfoPopup = await GameManager.Instance.UIManager.OpenStageInfoPopupAsync();

        if (null == _stageInfoPopup)
        {
            Debug.LogError("[StageSelectController] StageInfoPopupView 를 열지 못했습니다.");
            return;
        }

        _stageInfoPopup.OnPartySettingButtonClicked -= HandlePartySettingClicked;
        _stageInfoPopup.OnCloseButtonClicked -= HandleStageInfoClosed;
        _stageInfoPopup.OnPartySettingButtonClicked += HandlePartySettingClicked;
        _stageInfoPopup.OnCloseButtonClicked += HandleStageInfoClosed;

        StageInfoPopupViewModel viewModel = new StageInfoPopupViewModel(stageData, _dataProvider.GetStageWaves(stageId));
        _stageInfoPopup.Bind(viewModel);
    }

    private void CloseStageInfoPopup()
    {
        if (null == _stageInfoPopup)
        {
            return;
        }

        GameManager.Instance.UIManager.CloseStageInfoPopup();

        UnsubscribeStageInfoPopup();
    }

    private void UnsubscribeStageInfoPopup()
    {
        if (null == _stageInfoPopup)
        {
            return;
        }

        _stageInfoPopup.OnPartySettingButtonClicked -= HandlePartySettingClicked;
        _stageInfoPopup.OnCloseButtonClicked -= HandleStageInfoClosed;
        _stageInfoPopup = null;
    }

    // ===== 파티 편성 팝업 =====

    private void HandlePartySettingClicked()
    {
        CloseStageInfoPopup();

        OpenPartySetupPopupAsync().Forget();
    }

    private async UniTaskVoid OpenPartySetupPopupAsync()
    {
        _partySetupPopup = await GameManager.Instance.UIManager.OpenPartySetupPopupAsync();

        if (null == _partySetupPopup)
        {
            Debug.LogError("[StageSelectController] PartySetupPopupView 를 열지 못했습니다.");
            return;
        }

        _partySetupPopup.OnStartButtonClicked -= HandleGameStartClicked;
        _partySetupPopup.OnCloseButtonClicked -= HandlePartySetupClosed;
        _partySetupPopup.OnStartButtonClicked += HandleGameStartClicked;
        _partySetupPopup.OnCloseButtonClicked += HandlePartySetupClosed;
    }

    private void HandlePartySetupClosed()
    {
        ClosePartySetupPopup();
        OpenStageInfoPopupAsync(_progressModel.SelectedStageId).Forget();
    }

    private void ClosePartySetupPopup()
    {
        if (null == _partySetupPopup)
        {
            return;
        }

        GameManager.Instance.UIManager.ClosePartySetupPopup();

        UnsubscribePartySetupPopup();
    }

    private void UnsubscribePartySetupPopup()
    {
        if (null == _partySetupPopup)
        {
            return;
        }

        _partySetupPopup.OnStartButtonClicked -= HandleGameStartClicked;
        _partySetupPopup.OnCloseButtonClicked -= HandlePartySetupClosed;
        _partySetupPopup = null;
    }

    private void HandleStageInfoClosed()
    {
        CloseStageInfoPopup();

        if (null != _playerParty)
        {
            _playerParty.ResumeMove();
        }
    }

    // TODO: 전투맵 로드/전환 구현
    private void HandleGameStartClicked()
    {
        ClosePartySetupPopup();

        _screenStateModel.ChangeScreen(ScreenType.Battle);

        Debug.Log($"[StageSelectController] 전투맵 이동 - stageId={_progressModel.SelectedStageId}");
    }
}