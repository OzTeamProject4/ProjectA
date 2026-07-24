using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class StageManager : BaseManager <StageManager>
{
    private const float FadeDuration = 0.35f;
    private const int ActiveCameraPriority = 20;

    [SerializeField] private StagePlayerPartyRoot _playerPartyPrefab;

    private ScreenStateModel _screenStateModel;
    private StageProgressModel _progressModel;

    private StageSelectMap _selectMap;
    private StageSelectMapViewModel _selectMapViewModel;
    private StageSelectHudView _stageSelectHud; // 선택맵 화면 HUD
    private BattleMap _battleMap;
    private string _battleMapKey;
    private Transform _mapRoot;
    private StagePlayerParty _playerParty;

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

        if (null != _screenStateModel)
        {
            _screenStateModel.OnScreenChanged -= HandleScreenChanged;
        }

        if (null != GameManager.Instance && null != GameManager.Instance.BattleManager)
        {
            GameManager.Instance.BattleManager.OnReturnToSelectRequested -= HandleReturnToSelectRequested;
        }

        if (null != _selectMapViewModel)
        {
            _selectMapViewModel.OnReturnToLobbyRequested -= HandleReturnToLobbyRequested;
            _selectMapViewModel.Dispose();
            _selectMapViewModel = null;
        }

        _stageSelectHud = null;

        if (null == GameManager.Instance)
        {
            return;
        }

        GameManager.Instance.ResourceManager.ReleaseAsset(AddressableKey.Prefab.StageSelectMap01);

        if (!string.IsNullOrEmpty(_battleMapKey))
        {
            GameManager.Instance.ResourceManager.ReleaseAsset(_battleMapKey);
            _battleMapKey = null;
        }
    }

    public async UniTask EnterAsync()
    {
        if (_hasEntered)
        {
            await ReEnterFromLobbyAsync();
            return;
        }

        await GameManager.Instance.UIManager.OpenOverlayUIAsync();

        _screenStateModel = new ScreenStateModel(ScreenType.StageSelect);
        _screenStateModel.OnScreenChanged += HandleScreenChanged;
        _progressModel = new StageProgressModel();

        GameManager.Instance.BattleManager.OnReturnToSelectRequested += HandleReturnToSelectRequested;

        _mapRoot = CreateMapRoot();

        _selectMap = await SpawnSelectMapAsync();

        if (null == _selectMap)
        {
            Debug.LogError("[StageManager] 선택맵 스폰에 실패했습니다.");
            return;
        }

        _playerParty = SpawnPlayerParty();

        CharacterListModel characterListModel = GetCharacterListModel();

        if (null == characterListModel)
        {
            Debug.LogError("[StageManager] 보유 캐릭터 목록을 가져오지 못했습니다.");
            return;
        }

        _selectMapViewModel = new StageSelectMapViewModel(_progressModel, _screenStateModel, _playerParty, characterListModel);
        _selectMapViewModel.OnReturnToLobbyRequested += HandleReturnToLobbyRequested;
        _selectMap.Bind(_selectMapViewModel);

        await ShowStageSelectHudAsync();

        _hasEntered = true;
        GameManager.Instance.UIManager.CloseOverlayUI();
    }

    // ===== 선택맵 HUD =====

    private async UniTask ShowStageSelectHudAsync()
    {
        _stageSelectHud = await GameManager.Instance.UIManager.OpenStageSelectHudAsync(destroyCancellationToken);

        if (null == _stageSelectHud)
        {
            Debug.LogError("[StageManager] 스테이지 선택 HUD 를 열지 못했습니다.");
            return;
        }

        _stageSelectHud.Bind(_selectMapViewModel);
    }

    private void HideStageSelectHud()
    {
        if (null == GameManager.Instance)
        {
            return;
        }

        GameManager.Instance.UIManager.CloseStageSelectHud();
    }

    // ===== 로비 복귀 =====

    private async UniTask ReEnterFromLobbyAsync()
    {
        if (null == _selectMap || null == _playerParty)
        {
            Debug.LogError("[StageManager] ReEnterFromLobbyAsync: 선택맵 또는 플레이어가 null 입니다.");
            return;
        }

        await GameManager.Instance.UIManager.OpenOverlayUIAsync();

        try
        {
            ReactivateSelectMap();
            ReactivateSelectPlayer();

            _playerParty.WarpTo(_progressModel.PlayerPosition);
            _playerParty.ResumeMove();

            await ShowStageSelectHudAsync();

            _screenStateModel.ChangeScreen(ScreenType.StageSelect);
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlayUI();
        }
    }

    private void HandleReturnToLobbyRequested()
    {
        ShowReturnToLobbyPopupAsync().Forget();
    }

    private async UniTaskVoid ShowReturnToLobbyPopupAsync()
    {
        if (null != _playerParty)
        {
            _playerParty.StopMove();
        }

        ReturnToLobbyChoice choice = await WaitForReturnToLobbyChoiceAsync();

        if (choice == ReturnToLobbyChoice.Confirm)
        {
            await ExitToLobbyAsync();
            return;
        }

        if (null != _playerParty)
        {
            _playerParty.ResumeMove();
        }
    }

    private async UniTask<ReturnToLobbyChoice> WaitForReturnToLobbyChoiceAsync()
    {
        ReturnToLobbyPopupView view = await GameManager.Instance.UIManager.OpenReturnToLobbyPopupAsync(destroyCancellationToken);

        if (null == view)
        {
            Debug.LogError("[StageManager] 로비 복귀 팝업을 열지 못했습니다. 복귀를 취소합니다.");
            return ReturnToLobbyChoice.Cancel;
        }

        ReturnToLobbyChoice choice = await view.WaitForChoiceAsync();

        GameManager.Instance.UIManager.CloseReturnToLobbyPopup();

        return choice;
    }

    private async UniTask ExitToLobbyAsync()
    {
        SavePlayerPosition();

        if (null != _selectMapViewModel)
        {
            _selectMapViewModel.CloseAllPopups();
        }

        await GameManager.Instance.UIManager.OpenOverlayUIAsync();

        try
        {
            HideStageSelectHud();
            DeactivateSelectPlayer();
            DeactivateSelectMap();

            _screenStateModel.ChangeScreen(ScreenType.Lobby);

            await GameManager.Instance.UIManager.OpenLobbyAsync();
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlayUI();
        }
    }

    private void ReEnter()
    {
        if (null == _playerParty)
        {
            Debug.LogError("[StageManager] ReEnter: _playerParty 가 null 입니다.");
            return;
        }

        _playerParty.WarpTo(_progressModel.PlayerPosition);
        _playerParty.ResumeMove();

        _screenStateModel.ChangeScreen(ScreenType.StageSelect);
    }

    private void SavePlayerPosition()
    {
        if (null == _progressModel || null == _playerParty)
        {
            return;
        }

        _progressModel.SetPlayerPosition(_playerParty.transform.position);
    }

    private Transform CreateMapRoot()
    {
        GameObject mapRootObject = new GameObject("MapRoot");
        mapRootObject.transform.SetParent(transform);
        mapRootObject.transform.localPosition = Vector3.zero;
        mapRootObject.transform.localRotation = Quaternion.identity;

        return mapRootObject.transform;
    }

    private async UniTask<StageSelectMap> SpawnSelectMapAsync()
    {
        if (null == _mapRoot)
        {
            Debug.LogError("[StageManager] _mapRoot 가 null 입니다.");
            return null;
        }

        GameObject mapPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(AddressableKey.Prefab.StageSelectMap01, destroyCancellationToken);

        if (null == mapPrefab)
        {
            Debug.LogError("[StageManager] 선택맵 프리팹 로드에 실패했습니다.");
            return null;
        }

        GameObject mapInstance = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, _mapRoot);

        if (!mapInstance.TryGetComponent(out StageSelectMap selectMap))
        {
            Debug.LogError("[StageManager] 스폰된 맵 오브젝트에 StageSelectMap 컴포넌트가 없습니다.");
            return null;
        }

        return selectMap;
    }

    private StagePlayerParty SpawnPlayerParty()
    {
        if (null == _playerPartyPrefab || null == _selectMap)
        {
            Debug.LogError("[StageManager] _playerPartyPrefab 또는 _selectMap 이 null 입니다.");
            return null;
        }

        Transform spawnPoint = _selectMap.PlayerSpawnPoint;

        if (null == spawnPoint)
        {
            Debug.LogError("[StageManager] 맵에 PlayerSpawnPoint 가 연결되지 않았습니다.");
            return null;
        }

        StagePlayerPartyRoot root = Instantiate(_playerPartyPrefab, spawnPoint.position, spawnPoint.rotation, transform);

        return root.PlayerParty;
    }

    private CharacterListModel GetCharacterListModel()
    {
        if (null == NetworkManagerTemp.Instance)
        {
            Debug.LogError("[StageManager] NetworkManagerTemp.Instance 가 null 입니다.");
            return null;
        }

        return NetworkManagerTemp.Instance.GetcharacterListModel();
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

    // ===== 전투맵 전환 =====

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
    }

    private void HandleReturnToSelectRequested()
    {
        _screenStateModel.ChangeScreen(ScreenType.StageSelect);
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
        StageData stageData = GetStage(_progressModel.SelectedStageId);

        if (null == stageData)
        {
            Debug.LogError($"[StageManager] StageData 를 찾을 수 없습니다. stageId={_progressModel.SelectedStageId}");
            return;
        }

        if (null != _selectMapViewModel)
        {
            _selectMapViewModel.CloseAllPopups();
        }

        HideStageSelectHud();
        DeactivateSelectMap();

        _battleMap = await SpawnBattleMapAsync(stageData.MapPrefabKey);

        if (null == _battleMap)
        {
            Debug.LogError("[StageManager] 전투맵 스폰에 실패했습니다.");
            return;
        }

        CinemachineCamera battleCamera = _battleMap.BattleCamera;

        if (null == battleCamera)
        {
            Debug.LogError("[StageManager] 전투맵에 BattleCamera 가 연결되지 않았습니다.");
            return;
        }

        DeactivateSelectPlayer();

        ActivateBattleCamera(battleCamera);

        await EnterBattleAsync(stageData, battleCamera);

        await CutToActiveCameraAsync();
        await UniTask.Delay((int)(FadeDuration * 1000f), cancellationToken: destroyCancellationToken);
    }

    private async UniTask CutToActiveCameraAsync()
    {
        await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);

        if (CinemachineBrain.ActiveBrainCount == 0)
        {
            Debug.LogWarning("[StageManager] 활성화된 CinemachineBrain 이 없습니다. 카메라 컷을 건너뜁니다.");
            return;
        }

        CinemachineBrain brain = CinemachineBrain.GetActiveBrain(0);

        if (null == brain)
        {
            return;
        }

        brain.ResetState();
        await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);
    }

    private async UniTask EnterBattleAsync(StageData stageData, CinemachineCamera battleCamera)
    {
        BattleManager battleManager = GameManager.Instance.BattleManager;

        if (null == battleManager)
        {
            Debug.LogError("[StageManager] BattleManager 싱글톤이 없습니다.");
            return;
        }

        Transform spawnPoint = _battleMap.PlayerSpawnPoint;

        if (null == spawnPoint)
        {
            Debug.LogError("[StageManager] 전투맵에 PlayerSpawnPoint 가 연결되지 않았습니다.");
            return;
        }


        IReadOnlyList<string> partyIds = _progressModel.SelectedPartyIds;

        if (null == partyIds || partyIds.Count == 0)
        {
            Debug.LogError("[StageManager] 편성된 캐릭터가 없어 전투에 진입할 수 없습니다.");
            return;
        }

        await battleManager.EnterBattle(spawnPoint.position, stageData.DataId, battleCamera, partyIds);
    }

    private void DeactivateSelectMap()
    {
        if (null == _selectMap)
        {
            return;
        }

        _selectMap.gameObject.SetActive(false);
    }

    private void ReactivateSelectMap()
    {
        if (null == _selectMap)
        {
            return;
        }

        _selectMap.gameObject.SetActive(true);
    }

    private void ClearBattleMap()
    {
        if (null != _battleMap)
        {
            Destroy(_battleMap.gameObject);
            _battleMap = null;
        }

        if (!string.IsNullOrEmpty(_battleMapKey))
        {
            GameManager.Instance.ResourceManager.ReleaseAsset(_battleMapKey);
            _battleMapKey = null;
        }
    }

    private async UniTask<BattleMap> SpawnBattleMapAsync(string mapPrefabKey)
    {
        if (string.IsNullOrEmpty(mapPrefabKey))
        {
            Debug.LogError("[StageManager] mapPrefabKey 가 비어 있습니다.");
            return null;
        }

        GameObject mapPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(mapPrefabKey, destroyCancellationToken);

        if (null == mapPrefab)
        {
            Debug.LogError($"[StageManager] 전투맵 프리팹 로드에 실패했습니다. key={mapPrefabKey}");
            return null;
        }

        GameObject mapInstance = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, _mapRoot);

        if (!mapInstance.TryGetComponent(out BattleMap battleMap))
        {
            Debug.LogError("[StageManager] 스폰된 맵 오브젝트에 BattleMap 컴포넌트가 없습니다.");
            return null;
        }

        _battleMapKey = mapPrefabKey;

        return battleMap;
    }

    private void DeactivateSelectPlayer()
    {
        if (null == _playerParty)
        {
            return;
        }

        _playerParty.StopMove();
        _playerParty.gameObject.SetActive(false);
    }

    private void ReactivateSelectPlayer()
    {
        if (null == _playerParty)
        {
            return;
        }

        _playerParty.gameObject.SetActive(true);
        _playerParty.ResumeMove();
    }

    private void ActivateBattleCamera(CinemachineCamera battleCamera)
    {
        battleCamera.Priority = ActiveCameraPriority;
    }

    // ===== 선택맵 복귀 전환 =====

    private async UniTask TransitionToSelectAsync()
    {
        if (null == _battleMap)
        {
            return;
        }

        await GameManager.Instance.UIManager.OpenOverlayUIAsync();

        try
        {
            ReactivateSelectMap();
            ReactivateSelectPlayer();

            await ShowStageSelectHudAsync();

            ClearBattleMap();

            await CutToActiveCameraAsync();
            await UniTask.Delay((int)(FadeDuration * 1000f), cancellationToken: destroyCancellationToken);
        }
        finally
        {
            GameManager.Instance.UIManager.CloseOverlayUI();
        }
    }
}