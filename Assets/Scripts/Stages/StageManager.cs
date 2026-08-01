using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class StageManager : BaseManager <StageManager>
{
    private const float FadeDuration = 0.35f;

    [SerializeField] private StagePlayerPartyRoot _playerPartyPrefab;

    private StageSession _session;
    private StageMapBuilder _mapBuilder;
    private StageSelectPlayer _player;

    // 아래 두 참조는 생성/파괴에만 쓰임
    private StageSelectMapViewModel _selectMapViewModel;
    private StagePlayerPartyViewModel _playerPartyViewModel;

    private bool _hasEntered;

    public override UniTask InitializeAsync()
    {
        return UniTask.CompletedTask;
    }

    private void OnEnable()
    {
        if (!_hasEntered)
        {
            return;
        }

        ReEnter();
    }

    private void OnDisable()
    {
        SavePlayerPosition();
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;

        if (null != _session)
        {
            _session.ScreenState.OnScreenChanged -= HandleScreenChanged;
        }

        if (null != GameManager.Instance && null != GameManager.Instance.BattleManager)
        {
            GameManager.Instance.BattleManager.OnReturnToSelectRequested -= HandleReturnToSelectRequested;
            GameManager.Instance.BattleManager.OnBattleEnded -= HandleBattleEnded;
            GameManager.Instance.BattleManager.OnRetryRequested -= HandleRetryRequested;
        }

        if (null != _mapBuilder)
        {
            _mapBuilder.Dispose();
        }

        DisposeViewModels();

        StageSession.Clear();
    }

    public async UniTask EnterAsync()
    {
        if (_hasEntered)
        {
            await ReEnterFromLobbyAsync();
            return;
        }

        await GameManager.Instance.UIManager.OpenOverlayAsync();

        _session = StageSession.Create();
        _session.ScreenState.OnScreenChanged += HandleScreenChanged;

        GameManager.Instance.BattleManager.OnReturnToSelectRequested += HandleReturnToSelectRequested;
        GameManager.Instance.BattleManager.OnBattleEnded += HandleBattleEnded;
        GameManager.Instance.BattleManager.OnRetryRequested += HandleRetryRequested;

        _mapBuilder = new StageMapBuilder(transform);
        _mapBuilder.CreateMapRoot();

        StageSelectMap selectMap = await _mapBuilder.SpawnSelectMapAsync(destroyCancellationToken);

        if (null == selectMap)
        {
            Debug.LogError("[StageManager] 선택맵 스폰에 실패했습니다.");
            return;
        }

        _player = new StageSelectPlayer(_playerPartyPrefab, transform);

        if (!_player.Spawn(selectMap.PlayerSpawnPoint))
        {
            Debug.LogError("[StageManager] 플레이어 파티 스폰에 실패했습니다.");
            return;
        }

        CreateViewModels();

        _player.Bind(_playerPartyViewModel);
        selectMap.Bind(_selectMapViewModel);

        _hasEntered = true;
        GameManager.Instance.UIManager.CloseOverlay();
    }

    // ===== ViewModel 조립 =====

    private void CreateViewModels()
    {
        StudentListModel characterListModel = GetCharacterListModel();

        _selectMapViewModel = new StageSelectMapViewModel(_session.Progress, _session.ScreenState, characterListModel, _session.MoveLock, _session.Stages);
        _playerPartyViewModel = new StagePlayerPartyViewModel(_session.MoveLock, _session.ScreenState);
    }

    private StudentListModel GetCharacterListModel()
    {
        if (null == NetworkManager.Instance)
        {
            Debug.LogError("[StageManager] NetworkManagerTemp.Instance 가 null 입니다.");
            return null;
        }

        return NetworkManager.Instance.StudentListModel;
    }

    private void DisposeViewModels()
    {
        if (null != _selectMapViewModel)
        {
            _selectMapViewModel.Dispose();
            _selectMapViewModel = null;
        }

        if (null != _playerPartyViewModel)
        {
            _playerPartyViewModel.Dispose();
            _playerPartyViewModel = null;
        }
    }

    // ===== 로비 복귀/재진입 =====

    private async UniTask ReEnterFromLobbyAsync()
    {
        if (null == _player || !_player.IsSpawned)
        {
            Debug.LogError("[StageManager] ReEnterFromLobbyAsync: 플레이어가 null 입니다.");
            return;
        }

        await GameManager.Instance.UIManager.OpenOverlayAsync();

        try
        {
            _session.ScreenState.ChangeScreen(ScreenType.StageSelect);

            _player.WarpTo(_session.Progress.PlayerPosition);
            _session.MoveLock.ClearLocks();
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlay();
        }
    }

    private async UniTask ExitToLobbyAsync()
    {
        SavePlayerPosition();

        await GameManager.Instance.UIManager.OpenOverlayAsync();

        try
        {
            await GameManager.Instance.UIManager.OpenLobbyAsync();
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlay();
        }
    }

    private void ReEnter()
    {
        if (null == _session || null == _player || !_player.IsSpawned)
        {
            Debug.LogError("[StageManager] ReEnter: 세션 또는 플레이어가 없습니다.");
            return;
        }

        _session.ScreenState.ChangeScreen(ScreenType.StageSelect);

        _player.WarpTo(_session.Progress.PlayerPosition);
        _session.MoveLock.ClearLocks();
    }

    private void SavePlayerPosition()
    {
        if (null == _session || null == _player || !_player.IsSpawned)
        {
            return;
        }

        _session.Progress.SetPlayerPosition(_player.Position);
    }

    // ===== 화면 전환 =====

    private void HandleScreenChanged(ScreenType screen)
    {
        if (screen == ScreenType.Battle)
        {
            TransitionToBattleAsync().Forget();
            return;
        }

        if (screen == ScreenType.StageSelect)
        {
            TransitionToSelectAsync().Forget();
            return;
        }

        if (screen == ScreenType.Lobby)
        {
            ExitToLobbyAsync().Forget();
            return;
        }
    }

    private void HandleReturnToSelectRequested()
    {
        _session.ScreenState.ChangeScreen(ScreenType.StageSelect);
    }

    private void HandleRetryRequested()
    {
        RetryBattleAsync().Forget();
    }

    private async UniTask RetryBattleAsync()
    {
        await GameManager.Instance.UIManager.OpenOverlayAsync();

        try
        {
            _mapBuilder.ClearBattleMap();

            await TransitionToBattleInternalAsync();
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlay();
        }
    }

    private void HandleBattleEnded(bool isVictory)
    {
        if (!isVictory || null == _session)
        {
            return;
        }

        string clearedStageId = _session.Progress.SelectedStageId;

        _session.Progress.AddCleared(clearedStageId);

        if (null == NetworkManager.Instance)
        {
            Debug.LogWarning($"[{nameof(StageManager)}:{nameof(HandleBattleEnded)}] NetworkManagerTemp가 없어 클리어 기록을 남기지 못했습니다.");
            return;
        }

        NetworkManager.Instance.StageClearModel.AddClearedStage(clearedStageId);
    }

    private async UniTask TransitionToBattleAsync()
    {
        await GameManager.Instance.UIManager.OpenOverlayAsync();

        try
        {
            await TransitionToBattleInternalAsync();
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlay();
        }
    }

    private async UniTask TransitionToBattleInternalAsync()
    {
        StageData stageData = _session.Stages.GetStage(_session.Progress.SelectedStageId);

        if (null == stageData)
        {
            Debug.LogError($"[StageManager] StageData 를 찾을 수 없습니다. stageId={_session.Progress.SelectedStageId}");
            return;
        }

        BattleMap battleMap = await _mapBuilder.SpawnBattleMapAsync(stageData.MapPrefabKey, destroyCancellationToken);

        if (null == battleMap)
        {
            Debug.LogError("[StageManager] 전투맵 스폰에 실패했습니다.");
            return;
        }

        CinemachineCamera battleCamera = battleMap.BattleCamera;

        if (null == battleCamera)
        {
            Debug.LogError("[StageManager] 전투맵에 BattleCamera 가 연결되지 않았습니다.");
            return;
        }

        _mapBuilder.ActivateBattleCamera(battleCamera);

        await EnterBattleAsync(stageData, battleMap, battleCamera);

        await _mapBuilder.CutToActiveCameraAsync(destroyCancellationToken);
        await UniTask.Delay((int)(FadeDuration * 1000f), cancellationToken: destroyCancellationToken);
    }

    private async UniTask EnterBattleAsync(StageData stageData, BattleMap battleMap, CinemachineCamera battleCamera)
    {
        BattleManager battleManager = GameManager.Instance.BattleManager;

        if (null == battleManager)
        {
            Debug.LogError("[StageManager] BattleManager 싱글톤이 없습니다.");
            return;
        }

        Transform spawnPoint = battleMap.PlayerSpawnPoint;

        if (null == spawnPoint)
        {
            Debug.LogError("[StageManager] 전투맵에 PlayerSpawnPoint 가 연결되지 않았습니다.");
            return;
        }

        IReadOnlyList<string> partyIds = _session.Progress.SelectedPartyIds;

        if (null == partyIds || partyIds.Count == 0)
        {
            Debug.LogError("[StageManager] 편성된 캐릭터가 없어 전투에 진입할 수 없습니다.");
            return;
        }

        await battleManager.EnterBattle(spawnPoint.position, stageData.DataId, battleCamera, partyIds);

        await GameManager.Instance.UIManager.OpenBattleHUDAsync(destroyCancellationToken);
    }

    // ===== 선택맵 복귀 전환 =====

    private async UniTask TransitionToSelectAsync()
    {
        if (!_mapBuilder.HasBattleMap)
        {
            return;
        }

        await GameManager.Instance.UIManager.OpenOverlayAsync();

        try
        {
            _mapBuilder.ClearBattleMap();

            await _mapBuilder.CutToActiveCameraAsync(destroyCancellationToken);
            await UniTask.Delay((int)(FadeDuration * 1000f), cancellationToken: destroyCancellationToken);
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlay();
        }
    }
}
