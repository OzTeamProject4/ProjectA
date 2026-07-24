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

    private StageSelectMap _selectMap;

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
        }

        if (null != _mapBuilder)
        {
            _mapBuilder.Dispose();
        }

        StageSession.Clear();
    }

    public async UniTask EnterAsync()
    {
        if (_hasEntered)
        {
            await ReEnterFromLobbyAsync();
            return;
        }

        await GameManager.Instance.UIManager.OpenOverlayUIAsync();

        _session = StageSession.Create();
        _session.ScreenState.OnScreenChanged += HandleScreenChanged;

        GameManager.Instance.BattleManager.OnReturnToSelectRequested += HandleReturnToSelectRequested;

        _mapBuilder = new StageMapBuilder(transform);
        _mapBuilder.CreateMapRoot();

        _selectMap = await _mapBuilder.SpawnSelectMapAsync(destroyCancellationToken);

        if (null == _selectMap)
        {
            Debug.LogError("[StageManager] 선택맵 스폰에 실패했습니다.");
            return;
        }

        _player = new StageSelectPlayer(_playerPartyPrefab, transform);

        if (!_player.Spawn(_selectMap.PlayerSpawnPoint))
        {
            Debug.LogError("[StageManager] 플레이어 파티 스폰에 실패했습니다.");
            return;
        }

        _session.Player = _player;

        await ShowStageSelectHudAsync();

        _hasEntered = true;
        GameManager.Instance.UIManager.CloseOverlayUI();
    }

    // ===== 선택맵 HUD =====

    private async UniTask ShowStageSelectHudAsync()
    {
        StageSelectHudView hud = await GameManager.Instance.UIManager.OpenStageSelectHudAsync(destroyCancellationToken);

        if (null == hud)
        {
            Debug.LogError("[StageManager] 스테이지 선택 HUD 를 열지 못했습니다.");
        }
    }

    private void HideStageSelectHud()
    {
        if (null == GameManager.Instance)
        {
            return;
        }

        GameManager.Instance.UIManager.CloseStageSelectHud();
    }

    // ===== 로비 복귀/재진입 =====

    private async UniTask ReEnterFromLobbyAsync()
    {
        if (null == _selectMap || null == _player || !_player.IsSpawned)
        {
            Debug.LogError("[StageManager] ReEnterFromLobbyAsync: 선택맵 또는 플레이어가 null 입니다.");
            return;
        }

        await GameManager.Instance.UIManager.OpenOverlayUIAsync();

        try
        {
            SetSelectMapActive(true);
            _player.Activate();

            _player.WarpTo(_session.Progress.PlayerPosition);
            _player.ResumeMove();

            await ShowStageSelectHudAsync();

            _session.ScreenState.ChangeScreen(ScreenType.StageSelect);
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlayUI();
        }
    }

    private async UniTask ExitToLobbyAsync()
    {
        SavePlayerPosition();

        await GameManager.Instance.UIManager.OpenOverlayUIAsync();

        try
        {
            HideStageSelectHud();
            _player.Deactivate();
            SetSelectMapActive(false);

            await GameManager.Instance.UIManager.OpenLobbyAsync();
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlayUI();
        }
    }

    private void ReEnter()
    {
        if (null == _session || null == _player || !_player.IsSpawned)
        {
            Debug.LogError("[StageManager] ReEnter: 세션 또는 플레이어가 없습니다.");
            return;
        }

        _player.WarpTo(_session.Progress.PlayerPosition);
        _player.ResumeMove();

        _session.ScreenState.ChangeScreen(ScreenType.StageSelect);
    }

    private void SavePlayerPosition()
    {
        if (null == _session || null == _player || !_player.IsSpawned)
        {
            return;
        }

        _session.Progress.SetPlayerPosition(_player.Position);
    }

    private void SetSelectMapActive(bool active)
    {
        if (null == _selectMap)
        {
            return;
        }

        _selectMap.gameObject.SetActive(active);
    }

    private StageData GetStage(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning("[StageManager] GetStage: stageId 가 비어 있습니다.");
            return null;
        }

        GameManager.Instance.DataManager.TryGetData(stageId, out StageData data);
        return data;
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

    private async UniTask TransitionToBattleAsync()
    {
        await GameManager.Instance.UIManager.OpenOverlayUIAsync();

        try
        {
            await TransitionToBattleInternalAsync();
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlayUI();
        }
    }

    private async UniTask TransitionToBattleInternalAsync()
    {
        StageData stageData = GetStage(_session.Progress.SelectedStageId);

        if (null == stageData)
        {
            Debug.LogError($"[StageManager] StageData 를 찾을 수 없습니다. stageId={_session.Progress.SelectedStageId}");
            return;
        }

        HideStageSelectHud();
        SetSelectMapActive(false);

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

        _player.Deactivate();

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
    }

    // ===== 선택맵 복귀 전환 =====

    private async UniTask TransitionToSelectAsync()
    {
        if (!_mapBuilder.HasBattleMap)
        {
            return;
        }

        await GameManager.Instance.UIManager.OpenOverlayUIAsync();

        try
        {
            SetSelectMapActive(true);
            _player.Activate();

            await ShowStageSelectHudAsync();

            _mapBuilder.ClearBattleMap();

            await _mapBuilder.CutToActiveCameraAsync(destroyCancellationToken);
            await UniTask.Delay((int)(FadeDuration * 1000f), cancellationToken: destroyCancellationToken);
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlayUI();
        }
    }
}
