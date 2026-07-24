using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageSelectMap : MonoBehaviour
{
    [SerializeField] private MonsterPartySpawner[] _spawners;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private StageMonsterParty _partyPrefab;

    private readonly List<StageMonsterParty> _spawnedParties = new List<StageMonsterParty>();

    private StageSelectMapViewModel _viewModel;
    private bool _isSubscribed;

    private StageInfoPopupView _stageInfoPopup;

    private bool _isStageInfoPopupRequested;

    public Transform PlayerSpawnPoint
    {
        get { return _playerSpawnPoint; }
    }

    private void Awake()
    {
        UnityUtil.ValidateReference(_partyPrefab, nameof(StageSelectMap), nameof(_partyPrefab));

        _viewModel = new StageSelectMapViewModel();

        SubscribeViewModel();
        SpawnParties();
    }

    private void OnDisable()
    {
        CloseAllPopups();
    }

    private void OnDestroy()
    {
        UnsubscribeParties();
        UnsubscribeViewModel();

        if (null != _viewModel)
        {
            _viewModel.Dispose();
            _viewModel = null;
        }

        _stageInfoPopup = null;
    }

    private void SubscribeViewModel()
    {
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnStageInfoPopupOpenRequested += HandleStageInfoPopupOpenRequested;
        _viewModel.OnStageInfoPopupCloseRequested += HandleStageInfoPopupCloseRequested;

        _isSubscribed = true;
    }

    private void UnsubscribeViewModel()
    {
        if (!_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnStageInfoPopupOpenRequested -= HandleStageInfoPopupOpenRequested;
        _viewModel.OnStageInfoPopupCloseRequested -= HandleStageInfoPopupCloseRequested;

        _isSubscribed = false;
    }

    private void CloseAllPopups()
    {
        if (null == _viewModel || null == GameManager.Instance)
        {
            return;
        }

        _viewModel.CloseAllPopups();
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

    private void HandlePlayerReached(string stageId)
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.HandlePartyReached(stageId);
    }

    private void HandlePlayerLeft(string stageId)
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.HandlePartyLeft(stageId);
    }

    // ===== 스테이지 정보 팝업 =====

    private void HandleStageInfoPopupOpenRequested(StageInfoPopupViewModel viewModel)
    {
        _isStageInfoPopupRequested = true;

        ShowStageInfoPopupAsync(viewModel).Forget();
    }

    private async UniTaskVoid ShowStageInfoPopupAsync(StageInfoPopupViewModel viewModel)
    {
        StageInfoPopupView popup = await GameManager.Instance.UIManager.OpenStageInfoPopupAsync();

        if (null == popup)
        {
            Debug.LogError("[StageSelectMap] StageInfoPopupView 를 열지 못했습니다.");
            return;
        }

        if (!_isStageInfoPopupRequested)
        {
            GameManager.Instance.UIManager.CloseStageInfoPopup();
            return;
        }

        _stageInfoPopup = popup;
        _stageInfoPopup.Bind(viewModel);
    }

    private void HandleStageInfoPopupCloseRequested()
    {
        _isStageInfoPopupRequested = false;

        CloseStageInfoPopupView();
    }

    private void CloseStageInfoPopupView()
    {
        if (null == _stageInfoPopup)
        {
            return;
        }

        GameManager.Instance.UIManager.CloseStageInfoPopup();

        _stageInfoPopup = null;
    }
}
