using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftListItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _goldText;

    [SerializeField] private GameObject _mat1Root;
    [SerializeField] private Image _mat1Icon;
    [SerializeField] private TMP_Text _mat1TierText;
    [SerializeField] private TMP_Text _mat1CountText;

    [SerializeField] private GameObject _mat2Root;
    [SerializeField] private Image _mat2Icon;
    [SerializeField] private TMP_Text _mat2TierText;
    [SerializeField] private TMP_Text _mat2CountText;

    [SerializeField] private Button _craftButton;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _uncraftableAlpha = 0.4f;

    private CraftListItemViewModel _viewModel;
    private bool _isSubscribed;
    private int _refreshToken;

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

    private void RefreshDisplay()
    {
        if (null == _viewModel)
        {
            return;
        }

        _goldText.text = _viewModel.RequiredGold.ToString();

        RefreshMaterialSlotsAsync().Forget();

        bool canCraft = _viewModel.CanCraft;
        _craftButton.interactable = canCraft;

        if (null != _canvasGroup)
        {
            _canvasGroup.alpha = canCraft ? 1f : _uncraftableAlpha;
        }
    }

    private async UniTaskVoid RefreshMaterialSlotsAsync()
    {
        int myToken = ++_refreshToken;

        var materials = _viewModel.GetMaterials();

        await SetMaterialSlotAsync(_mat1Root, _mat1Icon, _mat1TierText, _mat1CountText, materials, 0, myToken);
        await SetMaterialSlotAsync(_mat2Root, _mat2Icon, _mat2TierText, _mat2CountText, materials, 1, myToken);
    }

    private async UniTask SetMaterialSlotAsync(GameObject root, Image icon, TMP_Text tierText, TMP_Text countText,
        IReadOnlyList<(string Name, string SpritePath, int Tier, int Owned, int Required)> materials,
        int index, int token)
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

        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(spritePath);

        if (token != _refreshToken || null == icon)
        {
            return;
        }

        icon.sprite = sprite;
    }

    private void HandleClickCraft()
    {
        _viewModel.CraftCommand();
        OnCrafted?.Invoke(_viewModel.DataId);
    }
}