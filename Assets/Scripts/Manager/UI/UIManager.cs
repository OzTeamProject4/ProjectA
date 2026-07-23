using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    private readonly HashSet<UIType> _loadingUISet =
        new HashSet<UIType>();

    private readonly HashSet<UIType> _openedUISet =
        new HashSet<UIType>();

    private readonly Dictionary<UIRoot, RectTransform> _rootCacheDictionary =
        new Dictionary<UIRoot, RectTransform>();

    private readonly Dictionary<UIType, BaseUI> _createdUICacheDictionary =
        new Dictionary<UIType, BaseUI>();

    private readonly SemaphoreSlim _initializeSemaphore =
        new SemaphoreSlim(1, 1);

    private UILayer _uiLayer;
    private bool _isInitialized;

    public override async UniTask InitializeAsync()
    {
        await EnsureInitializedAsync(destroyCancellationToken);
    }

    public async UniTask<BaseUI> OpenTestRootAsync(
        UIType uiType,
        CancellationToken cancellationToken = default)
    {
        return await OpenUIAsync(
            uiType,
            UIRoot.A,
            cancellationToken);
    }

    public async UniTask<BaseUI> OpenContentRootAsync(
        UIType uiType,
        CancellationToken cancellationToken = default)
    {
        return await OpenUIAsync(
            uiType,
            UIRoot.B,
            cancellationToken);
    }

    public async UniTask<BaseUI> OpenPopupRootAsync(
        UIType uiType,
        CancellationToken cancellationToken = default)
    {
        return await OpenUIAsync(
            uiType,
            UIRoot.C,
            cancellationToken);
    }

    public async UniTask<BaseUI> OpenOverlayRootAsync(
        UIType uiType,
        CancellationToken cancellationToken = default)
    {
        return await OpenUIAsync(
            uiType,
            UIRoot.D,
            cancellationToken);
    }

    public void Close(UIType uiType)
    {
        CloseUI(uiType);
    }

    private async UniTask<bool> EnsureInitializedAsync(
        CancellationToken cancellationToken)
    {
        if (_isInitialized)
        {
            return true;
        }

        await _initializeSemaphore.WaitAsync(cancellationToken);

        try
        {
            if (_isInitialized)
            {
                return true;
            }

            if (_uiLayer == null)
            {
                _uiLayer = await CreateLayerAsync(cancellationToken);
            }

            if (_uiLayer == null)
            {
                Debug.LogError(
                    $"[{nameof(UIManager)}:{nameof(EnsureInitializedAsync)}] " +
                    "UILayer 초기화에 실패했습니다.");

                return false;
            }

            if (!CacheRootDictionary())
            {
                Debug.LogError(
                    $"[{nameof(UIManager)}:{nameof(EnsureInitializedAsync)}] " +
                    "UIRoot 캐시 생성에 실패했습니다.");

                return false;
            }

            _isInitialized = true;
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(EnsureInitializedAsync)}] " +
                $"UIManager 초기화 중 오류가 발생했습니다.\n{exception}");

            return false;
        }
        finally
        {
            _initializeSemaphore.Release();
        }
    }

    private async UniTask<BaseUI> OpenUIAsync(
        UIType uiType,
        UIRoot uiRoot,
        CancellationToken cancellationToken)
    {
        bool isInitialized =
            await EnsureInitializedAsync(cancellationToken);

        if (!isInitialized)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(OpenUIAsync)}] " +
                $"'{uiType}' UI를 열기 전에 UIManager 초기화에 실패했습니다.");

            return null;
        }

        if (!TryGetRootRectTransform(
                uiRoot,
                out RectTransform rootRectTransform))
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(OpenUIAsync)}] " +
                $"'{uiRoot}'에 해당하는 Root RectTransform을 찾을 수 없습니다.");

            return null;
        }

        if (_loadingUISet.Contains(uiType))
        {
            Debug.LogWarning(
                $"[{nameof(UIManager)}:{nameof(OpenUIAsync)}] " +
                $"'{uiType}' UI가 이미 로딩 중입니다.");

            return null;
        }

        if (_openedUISet.Contains(uiType))
        {
            Debug.LogWarning(
                $"[{nameof(UIManager)}:{nameof(OpenUIAsync)}] " +
                $"'{uiType}' UI가 이미 열려 있습니다.");

            if (_createdUICacheDictionary.TryGetValue(
                    uiType,
                    out BaseUI openedUI))
            {
                return openedUI;
            }

            _openedUISet.Remove(uiType);
        }

        _loadingUISet.Add(uiType);

        try
        {
            if (_createdUICacheDictionary.TryGetValue(
                    uiType,
                    out BaseUI cachedUI))
            {
                SetCreatedUIRoot(cachedUI, rootRectTransform);
                OpenUI(uiType, cachedUI);

                return cachedUI;
            }

            BaseUI createdUI = await CreateUIAsync(
                uiType,
                rootRectTransform,
                cancellationToken);

            if (createdUI == null)
            {
                Debug.LogError(
                    $"[{nameof(UIManager)}:{nameof(OpenUIAsync)}] " +
                    $"'{uiType}' UI 생성에 실패했습니다.");

                return null;
            }

            _createdUICacheDictionary.Add(uiType, createdUI);

            OpenUI(uiType, createdUI);

            return createdUI;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(OpenUIAsync)}] " +
                $"'{uiType}' UI를 여는 중 오류가 발생했습니다.\n{exception}");

            return null;
        }
        finally
        {
            _loadingUISet.Remove(uiType);
        }
    }

    private void OpenUI(
        UIType uiType,
        BaseUI uiObject)
    {
        if (uiObject == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(OpenUI)}] " +
                $"'{uiType}' UI가 null입니다.");

            return;
        }

        _openedUISet.Add(uiType);
        uiObject.gameObject.SetActive(true);
    }

    private async UniTask<BaseUI> CreateUIAsync(
        UIType uiType,
        RectTransform rootRectTransform,
        CancellationToken cancellationToken)
    {
        if (rootRectTransform == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CreateUIAsync)}] " +
                $"'{uiType}' UI의 부모 RectTransform이 null입니다.");

            return null;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CreateUIAsync)}] " +
                "GameManager가 존재하지 않습니다.");

            return null;
        }

        if (GameManager.Instance.ResourceManager == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CreateUIAsync)}] " +
                "ResourceManager가 존재하지 않습니다.");

            return null;
        }

        string addressableKey =
            AddressableKey.GetUIKey(uiType);

        GameObject uiPrefab =
            await GameManager.Instance.ResourceManager
                .LoadAssetAsync<GameObject>(
                    addressableKey,
                    cancellationToken);

        if (uiPrefab == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CreateUIAsync)}] " +
                $"'{addressableKey}' UI 프리팹을 로드하지 못했습니다.");

            return null;
        }

        if (!uiPrefab.TryGetComponent(out BaseUI uiBasePrefab))
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CreateUIAsync)}] " +
                $"'{uiType}' UI 프리팹에 BaseUI 컴포넌트가 없습니다.");

            return null;
        }

        BaseUI uiInstance = Instantiate(
            uiBasePrefab,
            rootRectTransform,
            false);

        return uiInstance;
    }

    private void CloseUI(UIType uiType)
    {
        if (!_openedUISet.Contains(uiType))
        {
            Debug.LogWarning(
                $"[{nameof(UIManager)}:{nameof(CloseUI)}] " +
                $"'{uiType}' UI가 열려 있지 않습니다.");

            return;
        }

        if (!_createdUICacheDictionary.TryGetValue(
                uiType,
                out BaseUI uiObject))
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CloseUI)}] " +
                $"'{uiType}' UI 캐시를 찾을 수 없습니다.");

            _openedUISet.Remove(uiType);
            return;
        }

        if (uiObject == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CloseUI)}] " +
                $"'{uiType}' UI 오브젝트가 null입니다.");

            _openedUISet.Remove(uiType);
            _createdUICacheDictionary.Remove(uiType);
            return;
        }

        _openedUISet.Remove(uiType);
        uiObject.gameObject.SetActive(false);
    }

    private bool CacheRootDictionary()
    {
        _rootCacheDictionary.Clear();

        if (_uiLayer == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CacheRootDictionary)}] " +
                "UILayer가 null입니다.");

            return false;
        }

        bool isValid = true;

        if (!CacheUIRoot(UIRoot.A, _uiLayer.A))
        {
            isValid = false;
        }

        if (!CacheUIRoot(UIRoot.B, _uiLayer.B))
        {
            isValid = false;
        }

        if (!CacheUIRoot(UIRoot.C, _uiLayer.C))
        {
            isValid = false;
        }

        if (!CacheUIRoot(UIRoot.D, _uiLayer.D))
        {
            isValid = false;
        }

        if (!CacheUIRoot(UIRoot.E, _uiLayer.E))
        {
            isValid = false;
        }

        if (!isValid)
        {
            _rootCacheDictionary.Clear();
        }

        return isValid;
    }

    private bool CacheUIRoot(
        UIRoot uiRoot,
        RectTransform rootRectTransform)
    {
        if (rootRectTransform == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CacheUIRoot)}] " +
                $"UIRoot '{uiRoot}'의 RectTransform이 null입니다.");

            return false;
        }

        if (!_rootCacheDictionary.TryAdd(
                uiRoot,
                rootRectTransform))
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CacheUIRoot)}] " +
                $"UIRoot '{uiRoot}'가 이미 캐시에 등록되어 있습니다.");

            return false;
        }

        return true;
    }

    private bool TryGetRootRectTransform(
        UIRoot uiRoot,
        out RectTransform rootRectTransform)
    {
        return _rootCacheDictionary.TryGetValue(
            uiRoot,
            out rootRectTransform);
    }

    private async UniTask<UILayer> CreateLayerAsync(
        CancellationToken cancellationToken)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CreateLayerAsync)}] " +
                "GameManager가 존재하지 않습니다.");

            return null;
        }

        if (GameManager.Instance.ResourceManager == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CreateLayerAsync)}] " +
                "ResourceManager가 존재하지 않습니다.");

            return null;
        }

        GameObject uiLayerPrefab =
            await GameManager.Instance.ResourceManager
                .LoadAssetAsync<GameObject>(
                    AddressableKey.Prefab.UILayer,
                    cancellationToken);

        if (uiLayerPrefab == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CreateLayerAsync)}] " +
                $"'{AddressableKey.Prefab.UILayer}' UILayer 프리팹을 " +
                "로드하지 못했습니다.");

            return null;
        }

        if (!uiLayerPrefab.TryGetComponent(
                out UILayer uiLayerPrefabComponent))
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(CreateLayerAsync)}] " +
                "UILayer 프리팹에 UILayer 컴포넌트가 없습니다.");

            return null;
        }

        UILayer uiLayerInstance =
            Instantiate(uiLayerPrefabComponent);

        uiLayerInstance.name =
            uiLayerPrefabComponent.name;

        DontDestroyOnLoad(uiLayerInstance.gameObject);

        return uiLayerInstance;
    }

    private void SetCreatedUIRoot(
        BaseUI uiObject,
        RectTransform rootRectTransform)
    {
        if (uiObject == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(SetCreatedUIRoot)}] " +
                "UI 오브젝트가 null입니다.");

            return;
        }

        if (rootRectTransform == null)
        {
            Debug.LogError(
                $"[{nameof(UIManager)}:{nameof(SetCreatedUIRoot)}] " +
                "Root RectTransform이 null입니다.");

            return;
        }

        if (uiObject.transform.parent == rootRectTransform)
        {
            return;
        }

        uiObject.transform.SetParent(
            rootRectTransform,
            false);
    }
}