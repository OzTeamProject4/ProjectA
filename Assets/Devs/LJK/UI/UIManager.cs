using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    private readonly HashSet<UIType> _loadingUISet = new HashSet<UIType>();
    private readonly HashSet<UIType> _openedUISet = new HashSet<UIType>();

    private readonly Dictionary<UIRoot, RectTransform> _rootCacheDictionary = new Dictionary<UIRoot, RectTransform>();
    private readonly Dictionary<UIType, GameObject> _createdUICacheDictionary = new Dictionary<UIType, GameObject>();

    private UILayer _uiLayer;

    public override async UniTask InitializeAsync()
    {
        if (_uiLayer == null)
        {
            await CreateLayer();
            CacheRootDictionary();
        }
    }

    public async UniTask OpenTestRootAsync(UIType uiType)
    {
        await OpenUIAsync(uiType, UIRoot.A);
    }

    public void Close(UIType uiType)
    {
        CloseUI(uiType);
    }

    private void CacheRootDictionary()
    {
        _rootCacheDictionary.Clear();

        CacheUIRoot(UIRoot.A, _uiLayer.A);
        CacheUIRoot(UIRoot.B, _uiLayer.B);
        CacheUIRoot(UIRoot.C, _uiLayer.C);
        CacheUIRoot(UIRoot.D, _uiLayer.D);
        CacheUIRoot(UIRoot.E, _uiLayer.E);
    }

    private async UniTask OpenUIAsync(UIType uiType, UIRoot uiRoot)
    {
        if (_openedUISet.Contains(uiType) || _loadingUISet.Contains(uiType))
        {
            Debug.LogWarning($"[{nameof(UIManager)}:{nameof(OpenUIAsync)}] '{uiType}' UI가 이미 열려 있거나 로딩 중입니다.");
            return;
        }

        _loadingUISet.Add(uiType);

        try
        {
            if (_createdUICacheDictionary.TryGetValue(uiType, out GameObject cachedUI))
            {
                OpenUI(uiType, cachedUI);
                return;
            }

            GameObject createdUI = await CreateUIAsync(uiType, uiRoot);

            if (createdUI == null)
            {
                Debug.LogError($"[{nameof(UIManager)}:{nameof(OpenUIAsync)}] '{uiType}' UI 생성에 실패하여 UI를 열 수 없습니다.");
                return;
            }

            _createdUICacheDictionary[uiType] = createdUI;

            OpenUI(uiType, createdUI);
        }
        finally
        {
            _loadingUISet.Remove(uiType);
        }
    }

    private void OpenUI(UIType uiType, GameObject uiObject)
    {
        _openedUISet.Add(uiType);
        uiObject.SetActive(true);
    }

    private async UniTask<GameObject> CreateUIAsync(UIType uiType, UIRoot uiRoot)
    {
        try
        {
            if (!TryGetRootRectTransform(uiRoot, out RectTransform rootRectTransform))
            {
                Debug.LogError($"[{nameof(UIManager)}:{nameof(CreateUIAsync)}] '{uiRoot}'에 해당하는 Root RectTransform을 찾을 수 없습니다.");
                return null;
            }
            
            string addressableKey = AddressableKey.GetUIKey(uiType);
            GameObject uiPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(addressableKey);
            
            if (uiPrefab == null)
            {
                Debug.LogError($"[{nameof(UIManager)}:{nameof(CreateUIAsync)}] '{uiType}' UI 프리팹을 로드하지 못했습니다.");
                return null;
            }

            GameObject uiInstance = Instantiate(uiPrefab, rootRectTransform);
            return uiInstance;
        }
        catch (Exception exception)
        {
            Debug.LogError($"[{nameof(UIManager)}:{nameof(CreateUIAsync)}] '{uiType}' UI 생성 중 예외가 발생했습니다.\n{exception}");
            return null;
        }
    }

    private void CloseUI(UIType uiType)
    {
        if (!_openedUISet.Contains(uiType))
        {
            Debug.LogWarning($"[{nameof(UIManager)}:{nameof(CloseUI)}] '{uiType}' UI가 열려 있지 않습니다.");
            return;
        }

        GameObject uiObject = _createdUICacheDictionary[uiType];

        _openedUISet.Remove(uiType);
        uiObject.SetActive(false);
    }

    private bool TryGetRootRectTransform(UIRoot uiRoot, out RectTransform rectTransform)
    {
        bool hasRootRectTransform = _rootCacheDictionary.TryGetValue(uiRoot, out rectTransform);
        return hasRootRectTransform;
    }

    private async UniTask CreateLayer()
    {
        GameObject uiLayerPrefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(AddressableKey.Prefab.UILayer);

        if (uiLayerPrefab == null)
        {
            Debug.LogError($"[{nameof(UIManager)}:{nameof(CreateLayer)}] UILayer 프리팹을 로드하지 못했습니다.");
            return;
        }

        if (!uiLayerPrefab.TryGetComponent(out UILayer uiLayer))
        {
            Debug.LogError($"[{nameof(UIManager)}:{nameof(CreateLayer)}] UILayer 프리팹에 UILayer 컴포넌트가 없습니다.");
            return;
        }

        _uiLayer = Instantiate(uiLayer);
    }

    private void CacheUIRoot(UIRoot uiRoot, RectTransform rectTransform)
    {
        if (!_rootCacheDictionary.TryAdd(uiRoot, rectTransform))
        {
            Debug.LogError($"[{nameof(UIManager)}:{nameof(CacheUIRoot)}] UIRoot '{uiRoot}'가 이미 루트 캐시에 등록되어 있습니다.");
        }
    }
}