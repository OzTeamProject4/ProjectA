using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageController : MonoBehaviour
{
    private const float CameraBlendDuration = 0.6f;

    [SerializeField] private StageMonsterParty _monsterPartyPrefab;
    [SerializeField] private StagePlayerPartyRoot _playerPartyPrefab;

    private ScreenStateModel _screenStateModel;
    private StageProgressModel _progressModel;
    private StageSelectController _selectController;
    private IGameDataProvider _dataProvider;

    private StageSelectMap _selectMap;
    private BattleMap _battleMap;
    private string _battleMapKey;
    private Transform _mapRoot;
    private StagePlayerParty _playerParty;
    private Camera _camera;

    private bool _hasEntered;

    private void Start()
    {
        EnterAsync().Forget();
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
        if (null != _screenStateModel)
        {
            _screenStateModel.OnScreenChanged -= HandleScreenChanged;
        }

        if (null != _selectController)
        {
            _selectController.Dispose();
        }

        GameManager.Instance.ResourceManager.ReleaseAsset(AddressableKey.Prefab.StageSelectMap01);

        if (!string.IsNullOrEmpty(_battleMapKey))
        {
            GameManager.Instance.ResourceManager.ReleaseAsset(_battleMapKey);
        }
    }

    public async UniTask EnterAsync()
    {
        await GameManager.Instance.InitializeManagersAsync();

        if (_hasEntered)
        {
            Debug.LogWarning("[StageController] 이미 진입했습니다. 중복 실행을 건너뜁니다.");
            return;
        }

        _screenStateModel = new ScreenStateModel(ScreenType.StageSelect);
        _screenStateModel.OnScreenChanged += HandleScreenChanged;
        _progressModel = new StageProgressModel();

        _mapRoot = CreateMapRoot();

        _selectMap = await SpawnSelectMapAsync();

        if (null == _selectMap)
        {
            Debug.LogError("[StageController] 선택맵 스폰에 실패했습니다.");
            return;
        }

        _dataProvider = new GameDataProvider();
        _playerParty = SpawnPlayerParty();

        _selectController = new StageSelectController(_monsterPartyPrefab, _selectMap, _progressModel, _dataProvider, _screenStateModel, _playerParty);
        _selectController.SpawnParties();

        _hasEntered = true;
    }

    private void ReEnter()
    {
        if (null == _playerParty)
        {
            Debug.LogError("[StageController] ReEnter: _playerParty 가 null 입니다.");
            return;
        }

        _playerParty.WarpTo(_progressModel.PlayerPosition);
        _playerParty.ResumeMove();

        // TODO: 전투맵 ->선택맵 복귀 로직 구현 시 그쪽에서 StageSelect 로 전환하는 게 정석 위치.
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
            Debug.LogError("[StageController] _mapRoot 가 null 입니다.");
            return null;
        }

        GameObject mapPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(AddressableKey.Prefab.StageSelectMap01, destroyCancellationToken);

        if (null == mapPrefab)
        {
            Debug.LogError("[StageController] 선택맵 프리팹 로드에 실패했습니다.");
            return null;
        }

        GameObject mapInstance = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, _mapRoot);

        if (!mapInstance.TryGetComponent(out StageSelectMap selectMap))
        {
            Debug.LogError("[StageController] 스폰된 맵 오브젝트에 StageSelectMap 컴포넌트가 없습니다.");
            return null;
        }

        return selectMap;
    }

    private StagePlayerParty SpawnPlayerParty()
    {
        if (null == _playerPartyPrefab || null == _selectMap)
        {
            Debug.LogError("[StageController] _playerPartyPrefab 또는 _selectMap 이 null 입니다.");
            return null;
        }

        Transform spawnPoint = _selectMap.PlayerSpawnPoint;

        if (null == spawnPoint)
        {
            Debug.LogError("[StageController] 맵에 PlayerSpawnPoint 가 연결되지 않았습니다.");
            return null;
        }

        StagePlayerPartyRoot root = Instantiate(_playerPartyPrefab, spawnPoint.position, spawnPoint.rotation, transform);

        return root.PlayerParty;
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
        // TODO: UIType.Loading 추가 후 여기서 로딩 오버레이 ON

        StageData stageData = _dataProvider.GetStage(_progressModel.SelectedStageId);

        if (null == stageData)
        {
            Debug.LogError($"[StageController] StageData 를 찾을 수 없습니다. stageId={_progressModel.SelectedStageId}");
            return;
        }

        ClearSelectMap();

        _battleMap = await SpawnBattleMapAsync(stageData.MapPrefabKey);

        if (null == _battleMap)
        {
            Debug.LogError("[StageController] 전투맵 스폰에 실패했습니다.");
            return;
        }

        DeactivateSelectPlayer();
        // TODO: WASD 플레이어를 _battleMap.PlayerSpawnPoint 에 스폰하는 지점.

        // 임시: 카메라 Blend를 위한 수동 Lerp 로직
        await BlendCameraToAsync(_battleMap.CameraAnchor, destroyCancellationToken);

        // TODO: UIType.Loading 추가 후 여기서 로딩 오버레이 OFF
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

    private void ClearSelectMap()
    {
        if (null != _selectController)
        {
            _selectController.Dispose();
            _selectController = null;
        }

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
            Debug.LogError("[StageController] mapPrefabKey 가 비어 있습니다.");
            return null;
        }

        GameObject mapPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(mapPrefabKey, destroyCancellationToken);

        if (null == mapPrefab)
        {
            Debug.LogError($"[StageController] 전투맵 프리팹 로드에 실패했습니다. key={mapPrefabKey}");
            return null;
        }

        GameObject mapInstance = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, _mapRoot);

        if (!mapInstance.TryGetComponent(out BattleMap battleMap))
        {
            Debug.LogError("[StageController] 스폰된 맵 오브젝트에 BattleMap 컴포넌트가 없습니다.");
            return null;
        }

        _battleMapKey = mapPrefabKey;

        return battleMap;
    }

    private async UniTask BlendCameraToAsync(Transform targetAnchor, CancellationToken cancellationToken)
    {
        if (null == targetAnchor)
        {
            Debug.LogWarning("[StageController] targetAnchor 가 null 입니다. 카메라 전환을 건너뜁니다.");
            return;
        }

        if (null == _camera)
        {
            _camera = Camera.main;
        }

        if (null == _camera)
        {
            Debug.LogWarning("[StageController] 메인 카메라를 찾을 수 없습니다. 카메라 전환을 건너뜁니다.");
            return;
        }

        Vector3 startPosition = _camera.transform.position;
        Quaternion startRotation = _camera.transform.rotation;

        Vector3 targetPosition = targetAnchor.position;
        Quaternion targetRotation = targetAnchor.rotation;

        float elapsed = 0f;

        try
        {
            while (elapsed < CameraBlendDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / CameraBlendDuration);

                _camera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                _camera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // 오브젝트 파괴로 취소됨, 무시
        }

        _camera.transform.position = targetPosition;
        _camera.transform.rotation = targetRotation;
    }
}