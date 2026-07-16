using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObjectManager : BaseManager<ObjectManager>
{
    public Transform ObjectPoolRoot { get; private set; }

    private readonly Dictionary<string, ObjectPool> _poolCacheDictionary = new Dictionary<string, ObjectPool>();

    private readonly Dictionary<GameObject, ObjectPool> _spawnedObjectPoolCacheDictionary = new Dictionary<GameObject, ObjectPool>();

    public override async UniTask InitializeAsync()
    {
        if (ObjectPoolRoot == null)
        {
            await CreateRoot();
        }
    }

    public async UniTask PrewarmAsync(string key, int count, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"[{nameof(ObjectManager)}:{nameof(PrewarmAsync)}] 전달된 Key가 Null 또는 빈 문자열입니다.");
            return;
        }

        if (count <= 0)
        {
            Debug.LogWarning($"[{nameof(ObjectManager)}:{nameof(PrewarmAsync)}] 유효하지 않은 사전 생성 개수 '{count}'입니다.");
            return;
        }

        ObjectPool objectPool = await GetOrCreatePoolAsync(key, cancellationToken);

        if (objectPool == null)
        {
            return;
        }

        objectPool.Prewarm(count);
    }

    public async UniTask<GameObject> SpawnAsync(string key, Transform parent, Vector3 position, Quaternion rotation, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"[{nameof(ObjectManager)}:{nameof(SpawnAsync)}] 전달된 Key가 Null 또는 빈 문자열입니다.");
            return null;
        }

        ObjectPool objectPool = await GetOrCreatePoolAsync(key, cancellationToken);

        if (objectPool == null)
        {
            return null;
        }

        GameObject instance = objectPool.Get();

        _spawnedObjectPoolCacheDictionary[instance] = objectPool;

        instance.transform.SetParent(parent, false);
        instance.transform.SetPositionAndRotation(position, rotation);

        if (instance.TryGetComponent(out IInitializable initializable))
        {
            initializable.Initialize(key);
        }

        instance.SetActive(true);

        return instance;
    }

    public async UniTask<GameObject> SpawnAsync(string key, Transform parent, Transform spawnTransform, CancellationToken cancellationToken = default)
    {
        if (spawnTransform == null)
        {
            Debug.LogError($"[{nameof(ObjectManager)}:{nameof(SpawnAsync)}] Spawn Transform이 Null입니다.");
            return null;
        }

        GameObject instance = await SpawnAsync(key, parent, spawnTransform.position, spawnTransform.rotation, cancellationToken);
        return instance;
    }

    public void Despawn(GameObject instance)
    {
        if (instance == null)
        {
            Debug.LogError($"[{nameof(ObjectManager)}:{nameof(Despawn)}] 반환할 오브젝트가 Null입니다.");
            return;
        }

        if (!_spawnedObjectPoolCacheDictionary.TryGetValue(instance, out ObjectPool objectPool))
        {
            Debug.LogWarning($"[{nameof(ObjectManager)}:{nameof(Despawn)}] '{instance.name}' 오브젝트는 관리 중인 스폰 오브젝트가 아닙니다.");
            return;
        }

        objectPool.Release(instance);
        _spawnedObjectPoolCacheDictionary.Remove(instance);
    }

    public void ClearAll()
    {
        foreach (var spawnedObjectPool in _spawnedObjectPoolCacheDictionary)
        {
            GameObject instance = spawnedObjectPool.Key;
            ObjectPool objectPool = spawnedObjectPool.Value;

            objectPool.Release(instance);
        }

        _spawnedObjectPoolCacheDictionary.Clear();

        foreach (ObjectPool objectPool in _poolCacheDictionary.Values)
        {
            objectPool.Dispose();
        }

        _poolCacheDictionary.Clear();
    }

    private async UniTask CreateRoot()
    {
        GameObject rootPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(AddressableKey.Prefab.ObjectPoolRoot, destroyCancellationToken);

        if (rootPrefab == null)
        {
            Debug.LogError($"[{nameof(ObjectManager)}:{nameof(CreateRoot)}] ObjectPoolRoot 프리팹을 로드하지 못했습니다.");
            return;
        }

        Transform rootTransform = rootPrefab.transform;

        ObjectPoolRoot = Instantiate(rootTransform);
        ObjectPoolRoot.name = rootPrefab.name;
    }

    private async UniTask<ObjectPool> CreateObjectPool(string key, CancellationToken cancellationToken)
    {
        ObjectPool objectPool = new ObjectPool();

        bool isInitialized = await objectPool.InitializePoolAsync(key, cancellationToken);

        if (!isInitialized)
        {
            Debug.LogError($"[{nameof(ObjectManager)}:{nameof(CreateObjectPool)}] '{key}' 오브젝트 풀 초기화에 실패했습니다.");
            return null;
        }

        _poolCacheDictionary.Add(key, objectPool);

        return objectPool;
    }

    private async UniTask<ObjectPool> GetOrCreatePoolAsync(string key, CancellationToken cancellationToken)
    {
        if (_poolCacheDictionary.TryGetValue(key, out ObjectPool objectPool))
        {
            return objectPool;
        }

        objectPool = await CreateObjectPool(key, cancellationToken);

        return objectPool;
    }
}
