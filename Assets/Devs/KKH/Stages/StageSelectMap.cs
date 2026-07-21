using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageSelectMap : MonoBehaviour
{
    [SerializeField] private MonsterPartySpawner[] _spawners;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private StageMonsterParty _partyPrefab;

    private readonly List<StageMonsterParty> _spawnedParties = new List<StageMonsterParty>();

    private StageProgressModel _progressModel;
    private ScreenStateModel _screenStateModel;
    private IGameDataProvider _dataProvider;
    private StagePlayerParty _playerParty;

    private StageInfoPopupView _stageInfoPopup;
    private StageInfoPopupViewModel _stageInfoViewModel;
    private PartySetupPopupView _partySetupPopup;
    private PartySetupPopupViewModel _partySetupViewModel;

    public IReadOnlyList<MonsterPartySpawner> Spawners
    {
        get { return _spawners; }
    }

    public Transform PlayerSpawnPoint
    {
        get { return _playerSpawnPoint; }
    }

    private void OnDestroy()
    {
        UnsubscribeParties();
        UnsubscribeStageInfoPopup();
        UnsubscribePartySetupPopup();
    }

    public void Initialize(StageProgressModel progressModel, ScreenStateModel screenStateModel,
        IGameDataProvider dataProvider, StagePlayerParty playerParty)
    {
        if (null == progressModel || null == screenStateModel || null == dataProvider || null == playerParty)
        {
            Debug.LogError("[StageSelectMap] Initialize 인자 중 null 이 있습니다.");
            return;
        }

        _progressModel = progressModel;
        _screenStateModel = screenStateModel;
        _dataProvider = dataProvider;
        _playerParty = playerParty;

        SpawnParties();
    }

    public void CloseAllPopups()
    {
        CloseStageInfoPopup();
        ClosePartySetupPopup();
    }

    // ===== 몬스터 파티 스폰 =====

    private void SpawnParties()
    {
        if (null == _partyPrefab)
        {
            Debug.LogError("[StageSelectMap] _partyPrefab 이 연결되지 않았습니다.");
            return;
        }

        foreach (MonsterPartySpawner spawner in _spawners)
        {
            if (null == spawner)
            {
                continue;
            }

            SpawnPartyAt(spawner);
        }
    }

    private void SpawnPartyAt(MonsterPartySpawner spawner)
    {
        StageMonsterParty party = Instantiate(_partyPrefab, spawner.SpawnPosition, Quaternion.identity, transform);

        party.SetStageId(spawner.StageId);

        party.OnPlayerReached -= HandlePlayerReached;
        party.OnPlayerLeft -= HandlePlayerLeft;
        party.OnPlayerReached += HandlePlayerReached;
        party.OnPlayerLeft += HandlePlayerLeft;

        _spawnedParties.Add(party);
    }

    private void UnsubscribeParties()
    {
        foreach (StageMonsterParty party in _spawnedParties)
        {
            if (null == party)
            {
                continue;
            }

            party.OnPlayerReached -= HandlePlayerReached;
            party.OnPlayerLeft -= HandlePlayerLeft;
        }

        _spawnedParties.Clear();
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
            Debug.LogWarning($"[StageSelectMap] StageData 를 찾을 수 없습니다. stageId={stageId}");
            return;
        }

        _stageInfoPopup = await GameManager.Instance.UIManager.OpenStageInfoPopupAsync();

        if (null == _stageInfoPopup)
        {
            Debug.LogError("[StageSelectMap] StageInfoPopupView 를 열지 못했습니다.");
            return;
        }

        _stageInfoViewModel = new StageInfoPopupViewModel(stageData, _dataProvider.GetStageWaves(stageId));
        _stageInfoViewModel.OnPartySetupRequested += HandlePartySetupRequested;
        _stageInfoViewModel.OnCloseRequested += HandleStageInfoCloseRequested;

        _stageInfoPopup.Bind(_stageInfoViewModel);
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
        if (null != _stageInfoViewModel)
        {
            _stageInfoViewModel.OnPartySetupRequested -= HandlePartySetupRequested;
            _stageInfoViewModel.OnCloseRequested -= HandleStageInfoCloseRequested;
            _stageInfoViewModel = null;
        }

        _stageInfoPopup = null;
    }

    private void HandleStageInfoCloseRequested()
    {
        CloseStageInfoPopup();

        if (null != _playerParty)
        {
            _playerParty.ResumeMove();
        }
    }

    // ===== 파티 편성 팝업 =====

    private void HandlePartySetupRequested()
    {
        CloseStageInfoPopup();

        OpenPartySetupPopupAsync().Forget();
    }

    private async UniTaskVoid OpenPartySetupPopupAsync()
    {
        _partySetupPopup = await GameManager.Instance.UIManager.OpenPartySetupPopupAsync();

        if (null == _partySetupPopup)
        {
            Debug.LogError("[StageSelectMap] PartySetupPopupView 를 열지 못했습니다.");
            return;
        }

        _partySetupViewModel = new PartySetupPopupViewModel(_screenStateModel);
        _partySetupViewModel.OnCloseRequested += HandlePartySetupCloseRequested;

        _partySetupPopup.Bind(_partySetupViewModel);
    }

    private void HandlePartySetupCloseRequested()
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
        if (null != _partySetupViewModel)
        {
            _partySetupViewModel.OnCloseRequested -= HandlePartySetupCloseRequested;
            _partySetupViewModel = null;
        }

        _partySetupPopup = null;
    }
}