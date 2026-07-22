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

    public Transform PlayerSpawnPoint
    {
        get { return _playerSpawnPoint; }
    }

    private void OnDestroy()
    {
        UnsubscribeParties();
        UnsubscribeViewModel();

        // 종료 중 GameManager 파괴 상황을 피하기 위해 UIManager 호출 없이 참조만 정리
        _stageInfoPopup = null;
    }

    public void Bind(StageSelectMapViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("[StageSelectMap] Bind: viewModel 이 null 입니다.");
            return;
        }

        UnsubscribeViewModel();

        _viewModel = viewModel;

        SubscribeViewModel();

        SpawnParties();
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
        OpenStageInfoPopupAsync(viewModel).Forget();
    }

    private async UniTaskVoid OpenStageInfoPopupAsync(StageInfoPopupViewModel viewModel)
    {
        _stageInfoPopup = await GameManager.Instance.UIManager.OpenStageInfoPopupAsync();

        if (null == _stageInfoPopup)
        {
            Debug.LogError("[StageSelectMap] StageInfoPopupView 를 열지 못했습니다.");
            return;
        }

        _stageInfoPopup.Bind(viewModel);
    }

    private void HandleStageInfoPopupCloseRequested()
    {
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
