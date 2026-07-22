//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class EquipmentListItemView : MonoBehaviour
//{
//    [SerializeField] private Image _iconImage;
//    [SerializeField] private Image _equippedCharacterIconImage;
//    [SerializeField] private GameObject _selector;
//    [SerializeField] private TMP_Text _equippedText;
//    [SerializeField] private Button _itemButton;

//    [SerializeField] private CanvasGroup _canvasGroup;
//    [SerializeField] private float _dimmedAlpha = 0.4f;

//    private EquipmentListItemViewModel _viewModel;
//    private bool _isSubscribed;

//    private readonly List<string> _loadedSpriteKeys = new();

//    public event Action<EquipmentListItemViewModel, RectTransform> OnItemClicked;

//    private void OnDisable()
//    {
//        Unsubscribe();
//    }

//    private void OnDestroy()
//    {
//        Unsubscribe();
//        ReleaseAllSprites();
//    }

//    public void Bind(EquipmentListItemViewModel viewModel)
//    {
//        if (null == viewModel)
//        {
//            Debug.LogError("Bind: EquipmentListItemViewModel 이 null 입니다.");
//            return;
//        }

//        Unsubscribe();

//        _viewModel = viewModel;

//        LoadIconAsync().Forget();
//        LoadEquippedCharacterIconAsync().Forget();

//        Subscribe();
//    }

//    private void Subscribe()
//    {
//        if (_isSubscribed || null == _viewModel)
//        {
//            return;
//        }

//        _viewModel.OnChanged += HandleChanged;
//        _itemButton.onClick.AddListener(HandleClickItem);
//        _isSubscribed = true;

//        _viewModel.Initialize();
//        RefreshDisplay();
//    }

//    private void Unsubscribe()
//    {
//        if (!_isSubscribed || null == _viewModel)
//        {
//            return;
//        }

//        _viewModel.OnChanged -= HandleChanged;
//        _viewModel.Dispose();
//        _itemButton.onClick.RemoveListener(HandleClickItem);
//        _isSubscribed = false;
//    }

//    private void HandleChanged()
//    {
//        RefreshDisplay();
//        // TODO: 이 화면이 열려있는 동안 장착 캐릭터가 바뀌는 경우의 초상화 실시간 갱신은 안됨
//        // 현재는 Bind 시점 1회 로드만 지원.
//    }

//    private void RefreshDisplay()
//    {
//        if (null == _viewModel)
//        {
//            return;
//        }

//        if (null != _selector)
//        {
//            _selector.SetActive(_viewModel.IsSelected);
//        }

//        if (null != _equippedText)
//        {
//            _equippedText.gameObject.SetActive(_viewModel.IsEquippedByThisCharacter);
//        }

//        if (null != _canvasGroup)
//        {
//            _canvasGroup.alpha = _viewModel.IsEquippedByOther ? _dimmedAlpha : 1f;
//        }
//    }

//    private async UniTaskVoid LoadIconAsync()
//    {
//        if (null == _iconImage)
//        {
//            return;
//        }

//        string spritePath = _viewModel.SpritePath;

//        if (string.IsNullOrEmpty(spritePath))
//        {
//            _iconImage.enabled = false;
//            return;
//        }

//        try
//        {
//            Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(spritePath, destroyCancellationToken);

//            if (null == sprite)
//            {
//                _iconImage.enabled = false;
//                return;
//            }

//            _loadedSpriteKeys.Add(spritePath);

//            _iconImage.enabled = true;
//            _iconImage.sprite = sprite;
//        }
//        catch (OperationCanceledException)
//        {
//            // 오브젝트 파괴로 취소됨, 무시
//        }
//        catch (Exception exception)
//        {
//            Debug.LogWarning($"[EquipmentListItemView] 스프라이트 로드 실패. spritePath={spritePath}\n{exception}");
//        }
//    }

//    private async UniTaskVoid LoadEquippedCharacterIconAsync()
//    {
//        if (null == _equippedCharacterIconImage)
//        {
//            return;
//        }

//        string iconPath = _viewModel.EquippedCharacterIconPath;

//        if (string.IsNullOrEmpty(iconPath))
//        {
//            _equippedCharacterIconImage.enabled = false;
//            return;
//        }

//        try
//        {
//            Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconPath, destroyCancellationToken);

//            if (null == sprite)
//            {
//                _equippedCharacterIconImage.enabled = false;
//                return;
//            }

//            _loadedSpriteKeys.Add(iconPath);

//            _equippedCharacterIconImage.enabled = true;
//            _equippedCharacterIconImage.sprite = sprite;
//        }
//        catch (OperationCanceledException)
//        {
//            // 오브젝트 파괴로 취소됨, 무시
//        }
//        catch (Exception exception)
//        {
//            Debug.LogWarning($"[EquipmentListItemView] 장착 캐릭터 초상화 로드 실패. iconPath={iconPath}\n{exception}");
//        }
//    }

//    private void HandleClickItem()
//    {
//        OnItemClicked?.Invoke(_viewModel, transform as RectTransform);
//    }

//    private void ReleaseAllSprites()
//    {
//        foreach (string key in _loadedSpriteKeys)
//        {
//            GameManager.Instance.ResourceManager.ReleaseAsset(key);
//        }

//        _loadedSpriteKeys.Clear();
//    }
//}