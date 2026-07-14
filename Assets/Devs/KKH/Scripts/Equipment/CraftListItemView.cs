using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftListItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Sprite _placeholderSprite;

    [Header("Gold Slots")]
    [SerializeField] private GameObject _goldRoot;
    [SerializeField] private Image _goldIcon;
    [SerializeField] private TMP_Text _goldText;

    [Header("Material1 Slots")]
    [SerializeField] private GameObject _mat1Root;
    [SerializeField] private Image _mat1Icon;
    [SerializeField] private TMP_Text _mat1TierText;
    [SerializeField] private TMP_Text _mat1CountText;

    [Header("Material2 Slots")]
    [SerializeField] private GameObject _mat2Root;
    [SerializeField] private Image _mat2Icon;
    [SerializeField] private TMP_Text _mat2TierText;
    [SerializeField] private TMP_Text _mat2CountText;

    [SerializeField] private Button _craftButton;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _uncraftableAlpha = 0.4f;

    private CraftListItemViewModel _viewModel;
    private bool _isSubscribed;

    public event Action<string> OnSpriteLoaded;
    public event Action<string> OnCrafted;

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void Bind(CraftListItemViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: CraftListItemViewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();

        _viewModel = viewModel;
        _nameText.text = _viewModel.Name;

        LoadIconAsync().Forget();

        Subscribe();
    }

    private void Subscribe()
    {
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnChanged += HandleChanged;
        _craftButton.onClick.AddListener(HandleClickCraft);
        _isSubscribed = true;

        _viewModel.Initialize();
        RefreshDisplay();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnChanged -= HandleChanged;
        _viewModel.Dispose();
        _craftButton.onClick.RemoveListener(HandleClickCraft);
        _isSubscribed = false;
    }

    private void HandleChanged()
    {
        RefreshDisplay();
    }

    private async UniTaskVoid LoadIconAsync()
    {
        await LoadSpriteIntoAsync(_iconImage, _viewModel.SpritePath);

        IReadOnlyList<(string Name, string SpritePath, int Tier, int Owned, int Required)> materials = _viewModel.GetMaterials();

        await LoadMaterialIconAsync(_mat1Icon, materials, 0);
        await LoadMaterialIconAsync(_mat2Icon, materials, 1);
    }

    private async UniTask LoadMaterialIconAsync(Image icon,
        IReadOnlyList<(string Name, string SpritePath, int Tier, int Owned, int Required)> materials, int index)
    {
        if (index >= materials.Count)
        {
            return;
        }

        await LoadSpriteIntoAsync(icon, materials[index].SpritePath);
    }

    private void RefreshDisplay()
    {
        if (null == _viewModel)
        {
            return;
        }

        _goldText.text = _viewModel.RequiredGold.ToString();

        RefreshMaterialTexts();

        bool canCraft = _viewModel.CanCraft;
        _craftButton.interactable = canCraft;

        if (null != _canvasGroup)
        {
            _canvasGroup.alpha = canCraft ? 1f : _uncraftableAlpha;
        }
    }

    private void RefreshMaterialTexts()
    {
        IReadOnlyList<(string Name, string SpritePath, int Tier, int Owned, int Required)> materials = _viewModel.GetMaterials();

        SetMaterialText(_mat1Root, _mat1TierText, _mat1CountText, materials, 0);
        SetMaterialText(_mat2Root, _mat2TierText, _mat2CountText, materials, 1);
    }

    private void SetMaterialText(GameObject root, TMP_Text tierText, TMP_Text countText,
    IReadOnlyList<(string Name, string SpritePath, int Tier, int Owned, int Required)> materials, int index)
    {
        if (null == root)
        {
            return;
        }

        if (index >= materials.Count)
        {
            root.SetActive(false);
            return;
        }

        root.SetActive(true);
        (string name, string spritePath, int tier, int owned, int required) = materials[index];

        tierText.text = tier > 0 ? $"T{tier}" : "-";
        countText.text = $"{owned}/{required}";
    }

    private async UniTask LoadSpriteIntoAsync(Image image, string spritePath)
    {
        if (null == image)
        {
            return;
        }

        if (string.IsNullOrEmpty(spritePath))
        {
            image.enabled = false;
            return;
        }

        try
        {
            Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(spritePath, destroyCancellationToken);

            if (null == sprite)
            {
                image.enabled = false;
                return;
            }

            OnSpriteLoaded?.Invoke(spritePath);

            image.enabled = true;
            image.sprite = sprite;
        }
        catch (OperationCanceledException)
        {
            // 오브젝트 파괴로 취소됨 — 무시
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[CraftListItemView] 스프라이트 로드 실패. spritePath={spritePath}\n{exception}");
        }
    }

    private void HandleClickCraft()
    {
        _viewModel.CraftCommand();
        OnCrafted?.Invoke(_viewModel.DataId);
    }
}