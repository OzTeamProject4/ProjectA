using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class StageSceneLoader : MonoBehaviour
{
    private const float FadeDuration = 0.35f;
    private const int ActiveCameraPriority = 20;

    [SerializeField] private StagePlayerPartyRoot _playerPartyPrefab;

    private ScreenStateModel _screenStateModel;
    private StageProgressModel _progressModel;

    private StageSelectMap _selectMap;
    private StageSelectMapViewModel _selectMapViewModel;
    private BattleMap _battleMap;
    private string _battleMapKey;
    private Transform _mapRoot;
    private StagePlayerParty _playerParty;

    private bool _hasEntered;

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
        if (null != _screenStateModel)
        {
            _screenStateModel.OnScreenChanged -= HandleScreenChanged;
        }

        if (null != _selectMapViewModel)
        {
            _selectMapViewModel.Dispose();
            _selectMapViewModel = null;
        }

        GameManager.Instance.ResourceManager.ReleaseAsset(AddressableKey.Prefab.StageSelectMap01);

        if (!string.IsNullOrEmpty(_battleMapKey))
        {
            GameManager.Instance.ResourceManager.ReleaseAsset(_battleMapKey);
        }
    }

    public async UniTask EnterAsync()
    {
        await GameManager.Instance.UIManager.OpenOverlayUIAsync();
        await GameManager.Instance.InitializeManagersAsync();
        await GameManager.Instance.DataManager.LoadRuntimeDataAsync();
        
        if (_hasEntered)
        {
            Debug.LogWarning("[StageSceneLoader] 이미 진입했습니다. 중복 실행을 건너뜁니다.");
            return;
        }

        _screenStateModel = new ScreenStateModel(ScreenType.StageSelect);
        _screenStateModel.OnScreenChanged += HandleScreenChanged;
        _progressModel = new StageProgressModel();

        _mapRoot = CreateMapRoot();

        _selectMap = await SpawnSelectMapAsync();

        if (null == _selectMap)
        {
            Debug.LogError("[StageSceneLoader] 선택맵 스폰에 실패했습니다.");
            return;
        }

        _playerParty = SpawnPlayerParty();

        _selectMapViewModel = new StageSelectMapViewModel(_progressModel, _screenStateModel, _playerParty);
        _selectMap.Bind(_selectMapViewModel);

        _hasEntered = true;
        GameManager.Instance.UIManager.CloseOverlayUI();
    }

    private void ReEnter()
    {
        if (null == _playerParty)
        {
            Debug.LogError("[StageSceneLoader] ReEnter: _playerParty 가 null 입니다.");
            return;
        }

        _playerParty.WarpTo(_progressModel.PlayerPosition);
        _playerParty.ResumeMove();

        // TODO: 전투맵 -> 선택맵 복귀 로직 구현 시 그쪽에서 StageSelect 로 전환
        // 지금은 그 복귀 로직이 없어서 로비 재진입 시점에 안전장치로 여기서 리셋함.
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
            Debug.LogError("[StageSceneLoader] _mapRoot 가 null 입니다.");
            return null;
        }

        GameObject mapPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(AddressableKey.Prefab.StageSelectMap01, destroyCancellationToken);

        if (null == mapPrefab)
        {
            Debug.LogError("[StageSceneLoader] 선택맵 프리팹 로드에 실패했습니다.");
            return null;
        }

        GameObject mapInstance = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, _mapRoot);

        if (!mapInstance.TryGetComponent(out StageSelectMap selectMap))
        {
            Debug.LogError("[StageSceneLoader] 스폰된 맵 오브젝트에 StageSelectMap 컴포넌트가 없습니다.");
            return null;
        }

        return selectMap;
    }

    private StagePlayerParty SpawnPlayerParty()
    {
        if (null == _playerPartyPrefab || null == _selectMap)
        {
            Debug.LogError("[StageSceneLoader] _playerPartyPrefab 또는 _selectMap 이 null 입니다.");
            return null;
        }

        Transform spawnPoint = _selectMap.PlayerSpawnPoint;

        if (null == spawnPoint)
        {
            Debug.LogError("[StageSceneLoader] 맵에 PlayerSpawnPoint 가 연결되지 않았습니다.");
            return null;
        }

        StagePlayerPartyRoot root = Instantiate(_playerPartyPrefab, spawnPoint.position, spawnPoint.rotation, transform);

        return root.PlayerParty;
    }

    private StageData GetStage(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning("[StageSceneLoader] GetStage: stageId 가 비어 있습니다.");
            return null;
        }

        GameManager.Instance.DataManager.TryGetData(stageId, out StageData data);
        return data;
    }

    // ===== 전투맵 전환 =====

    private void HandleScreenChanged(ScreenType screen)
    {
        if (screen != ScreenType.Battle)
        {
            return;
        }

        TransitionToBattleAsync().Forget();
    }

    private async UniTask TransitionToBattleAsync()
    {
        await GameManager.Instance.UIManager.OpenOverlayUIAsync();
        StageData stageData = GetStage(_progressModel.SelectedStageId);

        if (null == stageData)
        {
            Debug.LogError($"[StageSceneLoader] StageData 를 찾을 수 없습니다. stageId={_progressModel.SelectedStageId}");
            return;
        }

        if (null != _selectMapViewModel)
        {
            _selectMapViewModel.CloseAllPopups();
        }

        ClearSelectMap();

        _battleMap = await SpawnBattleMapAsync(stageData.MapPrefabKey);

        if (null == _battleMap)
        {
            Debug.LogError("[StageSceneLoader] 전투맵 스폰에 실패했습니다.");
            return;
        }

        DeactivateSelectPlayer();

        ActivateBattleCamera();

        await EnterBattleAsync(stageData);

        await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);

        GameManager.Instance.UIManager.CloseOverlayUI();
    }

    private async UniTask EnterBattleAsync(StageData stageData)
    {
        BattleManager battleManager = _battleMap.BattleManager;

        if (null == battleManager)
        {
            Debug.LogError("[StageSceneLoader] 전투맵에 BattleManager 가 연결되지 않았습니다.");
            return;
        }

        Transform spawnPoint = _battleMap.PlayerSpawnPoint;

        if (null == spawnPoint)
        {
            Debug.LogError("[StageSceneLoader] 전투맵에 PlayerSpawnPoint 가 연결되지 않았습니다.");
            return;
        }

        await battleManager.EnterBattle(spawnPoint.position);

        // TODO: BattleTimer 배선 - 전투맵에 BattleTimer 붙으면 여기 또는 EnterBattle 내부에서 StartTimer(stageData.TimeLimit) 호출
        // TODO: 웨이브(StageWaveData) 기반 EnemySpawn 순차 호출 배선
    }

    private void ClearSelectMap()
    {
        if (null != _selectMap)
        {
            Destroy(_selectMap.gameObject);
            _selectMap = null;
        }

        GameManager.Instance.ResourceManager.ReleaseAsset(AddressableKey.Prefab.StageSelectMap01);
    }

    private async UniTask<BattleMap> SpawnBattleMapAsync(string mapPrefabKey)
    {
        if (string.IsNullOrEmpty(mapPrefabKey))
        {
            Debug.LogError("[StageSceneLoader] mapPrefabKey 가 비어 있습니다.");
            return null;
        }

        GameObject mapPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(mapPrefabKey, destroyCancellationToken);

        if (null == mapPrefab)
        {
            Debug.LogError($"[StageSceneLoader] 전투맵 프리팹 로드에 실패했습니다. key={mapPrefabKey}");
            return null;
        }

        GameObject mapInstance = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, _mapRoot);

        if (!mapInstance.TryGetComponent(out BattleMap battleMap))
        {
            Debug.LogError("[StageSceneLoader] 스폰된 맵 오브젝트에 BattleMap 컴포넌트가 없습니다.");
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

    private void ActivateBattleCamera()
    {
        if (null == _battleMap)
        {
            return;
        }

        CinemachineCamera battleCamera = _battleMap.BattleCamera;

        if (null == battleCamera)
        {
            Debug.LogWarning("[StageSceneLoader] 전투맵에 BattleCamera 가 연결되지 않았습니다. 카메라 전환을 건너뜁니다.");
            return;
        }

        battleCamera.Priority = ActiveCameraPriority;
    }
}