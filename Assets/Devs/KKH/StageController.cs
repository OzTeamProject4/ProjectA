using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] private StageMonsterParty _monsterPartyPrefab;
    [SerializeField] private StagePlayerPartyRoot _playerPartyPrefab;

    private ScreenStateModel _screenStateModel;
    private StageProgressModel _progressModel;
    private StageSelectController _selectController;

    private StageSelectMap _selectMap;
    private Transform _mapRoot;
    private StagePlayerParty _playerParty;

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
        if (null != _selectController)
        {
            _selectController.Dispose();
        }

        GameManager.Instance.ResourceManager.ReleaseAsset(AddressableKey.Prefab.StageSelectMap01);
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
        _progressModel = new StageProgressModel();

        _mapRoot = CreateMapRoot();

        _selectMap = await SpawnSelectMapAsync();

        if (null == _selectMap)
        {
            Debug.LogError("[StageController] 선택맵 스폰에 실패했습니다.");
            return;
        }

        IGameDataProvider dataProvider = new GameDataProvider();
        _playerParty = SpawnPlayerParty();

        _selectController = new StageSelectController(_monsterPartyPrefab, _selectMap, _progressModel, dataProvider, _screenStateModel, _playerParty);
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
}