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
    private bool _isHudRequested;

    public Transform PlayerSpawnPoint
    {
        get { return _playerSpawnPoint; }
    }

    private void Awake()
    {
        UnityUtil.ValidateReference(_partyPrefab, nameof(StageSelectMap), nameof(_partyPrefab));

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

        _viewModel = null;
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

        RefreshClearedParties();

        _viewModel.Refresh();
    }

    private void SubscribeViewModel()
    {
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnStageInfoPopupOpenRequested += HandleStageInfoPopupOpenRequested;
        _viewModel.OnStageInfoPopupCloseRequested += HandleStageInfoPopupCloseRequested;
        _viewModel.OnVisibleChanged += HandleVisibleChanged;
        _viewModel.OnHudOpenRequested += HandleHudOpenRequested;
        _viewModel.OnHudCloseRequested += HandleHudCloseRequested;
        _viewModel.OnStageCleared += HandleStageCleared;

        _isSubscribed = true;
    }

    private void UnsubscribeViewModel()
    {
        if (!_isSubscribed)
        {
            return;
        }

        if (null != _viewModel)
        {
            _viewModel.OnStageInfoPopupOpenRequested -= HandleStageInfoPopupOpenRequested;
            _viewModel.OnStageInfoPopupCloseRequested -= HandleStageInfoPopupCloseRequested;
            _viewModel.OnVisibleChanged -= HandleVisibleChanged;
            _viewModel.OnHudOpenRequested -= HandleHudOpenRequested;
            _viewModel.OnHudCloseRequested -= HandleHudCloseRequested;
            _viewModel.OnStageCleared -= HandleStageCleared;
        }

        _isSubscribed = false;
    }

    // ===== 표시/숨김 =====

    private void HandleVisibleChanged(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    // ===== 선택맵 HUD =====

    private void HandleHudOpenRequested(StageSelectHudViewModel hudViewModel)
    {
        _isHudRequested = true;

        ShowHudAsync(hudViewModel).Forget();
    }

    private async UniTaskVoid ShowHudAsync(StageSelectHudViewModel hudViewModel)
    {
        if (null == GameManager.Instance)
        {
            return;
        }

        StageSelectHudView hud = await GameManager.Instance.UIManager.OpenStageSelectHudAsync(destroyCancellationToken);

        if (null == GameManager.Instance)
        {
            return;
        }

        if (null == hud)
        {
            Debug.LogError("[StageSelectMap] 스테이지 선택 HUD 를 열지 못했습니다.");
            return;
        }

        if (!_isHudRequested)
        {
            GameManager.Instance.UIManager.CloseStageSelectHud();
            return;
        }

        hud.Bind(hudViewModel);
    }

    private void HandleHudCloseRequested()
    {
        _isHudRequested = false;

        if (null == GameManager.Instance)
        {
            return;
        }

        GameManager.Instance.UIManager.CloseStageSelectHud();
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

    // ===== 스테이지 클리어 =====

    private void HandleStageCleared(string stageId)
    {
        SetPartyActive(stageId, false);
    }

    private void RefreshClearedParties()
    {
        if (null == _viewModel)
        {
            return;
        }

        foreach (StageMonsterParty party in _spawnedParties)
        {
            if (null == party)
            {
                continue;
            }

            party.gameObject.SetActive(!_viewModel.IsStageCleared(party.StageId));
        }
    }

    private void SetPartyActive(string stageId, bool isActive)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            return;
        }

        foreach (StageMonsterParty party in _spawnedParties)
        {
            if (null == party || party.StageId != stageId)
            {
                continue;
            }

            party.gameObject.SetActive(isActive);
        }
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
        if (null == GameManager.Instance)
        {
            return;
        }

        StageInfoPopupView popup = await GameManager.Instance.UIManager.OpenStageInfoPopupAsync();

        if (null == GameManager.Instance)
        {
            return;
        }

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

        if (null != GameManager.Instance)
        {
            GameManager.Instance.UIManager.CloseStageInfoPopup();
        }

        _stageInfoPopup = null;
    }
}
