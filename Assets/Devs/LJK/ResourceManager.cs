using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : BaseManager<ResourceManager>
{
    private readonly Dictionary<string, AssetHandleInfo> _assetHandleDictionary = new Dictionary<string, AssetHandleInfo>();
    
    private readonly Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>> _loadingSourceDictionary = new Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>>();

    private CancellationTokenSource _releaseAllCts = new CancellationTokenSource();

    public override  void Initialize()
    {
        ReleaseAllAssets();
    }

    public async UniTask<T> LoadAssetAsync<T>(string key, CancellationToken cancellationToken = default) where T : UnityEngine.Object
    {
        T asset = await GetOrLoadAssetAsync<T>(key, cancellationToken);

        return asset;
    }

    public void ReleaseAsset(string key)
    {
        if (!_assetHandleDictionary.TryGetValue(key, out var handleInfo))
        {
            return;
        }

        handleInfo.RemoveReferenceCount();

        if (handleInfo.ReferenceCount > 0)
        {
            return;
        }

        ReleaseHandle(handleInfo.Handle);
        _assetHandleDictionary.Remove(key);
    }

    public void ReleaseAllAssets()
    {
        _releaseAllCts.Cancel();

        foreach (var pair in _assetHandleDictionary)
        {
            ReleaseHandle(pair.Value.Handle);
        }

        _assetHandleDictionary.Clear();

        foreach (var pair in _loadingSourceDictionary)
        {
            pair.Value.TrySetResult(null);
        }

        _loadingSourceDictionary.Clear();

        _releaseAllCts.Dispose();
        _releaseAllCts = new CancellationTokenSource();
    }

    private async UniTask<T> GetOrLoadAssetAsync<T>(string key, CancellationToken cancellationToken) where T : UnityEngine.Object
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError("[ResourceManager:LoadAssetAsync] 전달된 key가 null이거나 빈 문자열 또는 공백 문자열입니다.");

            return null;
        }

        if (_assetHandleDictionary.TryGetValue(key, out AssetHandleInfo loadedAssetHandle))
        {
            T assetFromLoadedHandle = GetAssetFromHandle<T>(loadedAssetHandle.Handle, key);

            if (assetFromLoadedHandle != null)
            {
                loadedAssetHandle.AddReferenceCount();

                return assetFromLoadedHandle;
            }

            Debug.LogWarning($"[ResourceManager:LoadAssetAsync] '{key}'의 Handle이 유효하지 않아 다시 로드합니다.");
            _assetHandleDictionary.Remove(key);
        }


        if (_loadingSourceDictionary.TryGetValue(key, out var loadingSource))
        {
            var assetFromLoadingSource = await loadingSource.Task.AttachExternalCancellation(cancellationToken: cancellationToken);

            T typeCastAsset = assetFromLoadingSource as T;

            if (typeCastAsset == null)
            {
                Debug.LogError($"[ResourceManager:LoadAssetAsync] '{key}' 타입 캐스팅 실패");

                return null;
            }

            if (!TryAddAssetReference(key))
            {
                return null;
            }

            return typeCastAsset;
        }

        var newCompletionSource = new UniTaskCompletionSource<UnityEngine.Object>();
        _loadingSourceDictionary[key] = newCompletionSource;

        ExecuteLoadAssetAsync<T>(key, newCompletionSource).Forget();

        var loadedObject = await newCompletionSource.Task.AttachExternalCancellation(cancellationToken);
       
        T loadAsset = loadedObject as T;
        
        if (loadAsset == null)
        {
            Debug.LogError($"[ResourceManager:LoadAssetAsync] '{key}' 타입 캐스팅 실패");

            if (_assetHandleDictionary.TryGetValue(key, out var handleInfo))
            {
                handleInfo.RemoveReferenceCount();
            }

            return null;
        }

        if (!TryAddAssetReference(key))
        {
            return null;
        }

        return loadAsset;
    }

    private async UniTask ExecuteLoadAssetAsync<T>(string key, UniTaskCompletionSource<UnityEngine.Object> source) where T : UnityEngine.Object
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);

        try
        {
            await handle.ToUniTask(cancellationToken: _releaseAllCts.Token);

            T asset = GetAssetFromHandle<T>(handle, key);

            if (asset == null)
            {
                throw new InvalidOperationException($"'{key}' 에셋 로드에 성공했지만 결과가 null입니다.");
            }

            _assetHandleDictionary[key] = new AssetHandleInfo(handle);

            RemoveLoadingTask(key, source);

            source.TrySetResult(asset);
        }
        catch (OperationCanceledException)
        {
            CleanupFailedLoad(key, source, handle);
        }
        catch (Exception exception)
        {
            Debug.LogError($"[ResourceManager:LoadAssetAsync] '{key}'의 에셋을 로드하는 중 예외가 발생했습니다.\n{exception}");

            CleanupFailedLoad(key, source, handle);
        }
    }

    private T GetAssetFromHandle<T>(AsyncOperationHandle handle, string key) where T : UnityEngine.Object
    {
        if (!handle.IsValid())
        {
            Debug.LogError($"[ResourceManager:GetAssetFromHandle] '{key}'의 Handle이 유효하지 않습니다.");
            return null;
        }

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[ResourceManager:GetAssetFromHandle] '{key}'의 에셋을 로드하지 못했습니다.");
            return null;
        }

        T asset = handle.Result as T;

        if (asset == null)
        {
            Debug.LogError($"[ResourceManager:GetAssetFromHandle] '{key}'의 로드된 에셋이 null입니다.");
            return null;
        }

        return asset;
    }

    private bool TryAddAssetReference(string key)
    {
        if (!_assetHandleDictionary.TryGetValue(key, out AssetHandleInfo assetHandleInfo))
        {
            Debug.LogError($"[ResourceManager:AddAssetReference] '{key}'의 Handle 정보를 찾을 수 없습니다.");

            return false;
        }

        assetHandleInfo.AddReferenceCount();

        return true;
    }

 
    private void RemoveLoadingTask(string key, UniTaskCompletionSource<UnityEngine.Object> source)
    {
        if (_loadingSourceDictionary.TryGetValue(key, out var currentSource))
        {
            if (currentSource == source)
            {
                _loadingSourceDictionary.Remove(key);
            }
        }
    }

    private void ReleaseHandle(AsyncOperationHandle handle)
    {
        if (!handle.IsValid())
        {
            return;
        }

        Addressables.Release(handle);
    }

    private void CleanupFailedLoad(string key, UniTaskCompletionSource<UnityEngine.Object> source, AsyncOperationHandle handle)
    {
        ReleaseHandle(handle);
        RemoveLoadingTask(key, source);

        source.TrySetResult(null);
    }
}