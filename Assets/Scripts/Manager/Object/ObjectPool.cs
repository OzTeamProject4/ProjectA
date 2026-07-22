using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObjectPool
{
    private Queue<GameObject> _poolQueue;

    private Transform _rootTransform;
    private GameObject _poolObjectPrefab;

    public async UniTask<bool> InitializePoolAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_poolQueue != null)
        {
            Debug.LogWarning($"[{nameof(ObjectPool)}:{nameof(InitializePoolAsync)}] 이미 초기화된 오브젝트 풀입니다.");
            return true;
        }

        _poolObjectPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(key, cancellationToken);

        if (_poolObjectPrefab == null)
        {
            Debug.LogError($"[{nameof(ObjectPool)}:{nameof(InitializePoolAsync)}] '{key}' 프리팹을 로드하지 못했습니다.");
            return false;
        }

        _poolQueue = new Queue<GameObject>();
        _rootTransform = GameManager.Instance.ObjectManager.ObjectPoolRoot;
        return true;
    }

    public void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject instance = InstantiatePrefab();
            _poolQueue.Enqueue(instance);
        }
    }

    public GameObject Get()
    {
        if (_poolQueue.TryDequeue(out GameObject instance))
        {
            return instance;
        }

        instance = InstantiatePrefab();
        return instance;
    }

    public void Release(GameObject instance)
    {
        if (instance == null)
        {
            Debug.LogError($"[{nameof(ObjectPool)}:{nameof(Release)}] 반환할 오브젝트가 Null입니다.");
            return;
        }

        instance.SetActive(false);

        if (instance.TryGetComponent(out IInitializable initializable))
        {
            initializable.Reset();
        }

        instance.transform.SetParent(_rootTransform, false);
        instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        _poolQueue.Enqueue(instance);
    }

    public void Dispose()
    {
        while (_poolQueue.Count > 0)
        {
            GameObject instance = _poolQueue.Dequeue();
            Object.Destroy(instance);
        }

        _poolQueue = null;
        _rootTransform = null;
        _poolObjectPrefab = null;
    }

    private GameObject InstantiatePrefab()
    {
        GameObject instance = Object.Instantiate(_poolObjectPrefab, _rootTransform);

        instance.SetActive(false);

        return instance;
    }
}