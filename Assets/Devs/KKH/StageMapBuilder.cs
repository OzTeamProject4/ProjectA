using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class StageMapBuilder
{
    private const int ActiveCameraPriority = 20;

    private readonly Transform _parent;

    private Transform _mapRoot;
    private BattleMap _battleMap;
    private string _battleMapKey;

    public BattleMap BattleMap
    {
        get { return _battleMap; }
    }

    public bool HasBattleMap
    {
        get { return null != _battleMap; }
    }

    public StageMapBuilder(Transform parent)
    {
        _parent = parent;
    }

    public void CreateMapRoot()
    {
        GameObject mapRootObject = new GameObject("MapRoot");
        mapRootObject.transform.SetParent(_parent);
        mapRootObject.transform.localPosition = Vector3.zero;
        mapRootObject.transform.localRotation = Quaternion.identity;

        _mapRoot = mapRootObject.transform;
    }

    public async UniTask<StageSelectMap> SpawnSelectMapAsync(CancellationToken cancellationToken)
    {
        if (null == _mapRoot)
        {
            Debug.LogError("[StageMapBuilder] _mapRoot 가 null 입니다.");
            return null;
        }

        GameObject mapPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(AddressableKey.Prefab.StageSelectMap01, cancellationToken);

        if (null == mapPrefab)
        {
            Debug.LogError("[StageMapBuilder] 선택맵 프리팹 로드에 실패했습니다.");
            return null;
        }

        GameObject mapInstance = Object.Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, _mapRoot);

        if (!mapInstance.TryGetComponent(out StageSelectMap selectMap))
        {
            Debug.LogError("[StageMapBuilder] 스폰된 맵 오브젝트에 StageSelectMap 컴포넌트가 없습니다.");
            return null;
        }

        return selectMap;
    }

    public async UniTask<BattleMap> SpawnBattleMapAsync(string mapPrefabKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(mapPrefabKey))
        {
            Debug.LogError("[StageMapBuilder] mapPrefabKey 가 비어 있습니다.");
            return null;
        }

        GameObject mapPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(mapPrefabKey, cancellationToken);

        if (null == mapPrefab)
        {
            Debug.LogError($"[StageMapBuilder] 전투맵 프리팹 로드에 실패했습니다. key={mapPrefabKey}");
            return null;
        }

        GameObject mapInstance = Object.Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, _mapRoot);

        if (!mapInstance.TryGetComponent(out BattleMap battleMap))
        {
            Debug.LogError("[StageMapBuilder] 스폰된 맵 오브젝트에 BattleMap 컴포넌트가 없습니다.");
            return null;
        }

        _battleMap = battleMap;
        _battleMapKey = mapPrefabKey;

        return battleMap;
    }

    public void ClearBattleMap()
    {
        if (null != _battleMap)
        {
            Object.Destroy(_battleMap.gameObject);
            _battleMap = null;
        }

        ReleaseBattleMapAsset();
    }

    public void ActivateBattleCamera(CinemachineCamera battleCamera)
    {
        if (null == battleCamera)
        {
            return;
        }

        battleCamera.Priority = ActiveCameraPriority;
    }

    public async UniTask CutToActiveCameraAsync(CancellationToken cancellationToken)
    {
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

        if (CinemachineBrain.ActiveBrainCount == 0)
        {
            Debug.LogWarning("[StageMapBuilder] 활성화된 CinemachineBrain 이 없습니다. 카메라 컷을 건너뜁니다.");
            return;
        }

        CinemachineBrain brain = CinemachineBrain.GetActiveBrain(0);

        if (null == brain)
        {
            return;
        }

        brain.ResetState();
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
    }

    public void Dispose()
    {
        if (null == GameManager.Instance)
        {
            return;
        }

        GameManager.Instance.ResourceManager.ReleaseAsset(AddressableKey.Prefab.StageSelectMap01);
        ReleaseBattleMapAsset();
    }

    private void ReleaseBattleMapAsset()
    {
        if (string.IsNullOrEmpty(_battleMapKey))
        {
            return;
        }

        GameManager.Instance.ResourceManager.ReleaseAsset(_battleMapKey);
        _battleMapKey = null;
    }
}
