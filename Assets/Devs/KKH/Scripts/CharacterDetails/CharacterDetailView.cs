using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetailView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _levelText;

    [SerializeField] private Slider _expSlider;
    [SerializeField] private TMP_Text _expText;

    [SerializeField] private TMP_Text _hpValueText;
    [SerializeField] private TMP_Text _atkValueText;
    [SerializeField] private TMP_Text _defValueText;
    [SerializeField] private TMP_Text _atkSpeedValueText;
    [SerializeField] private TMP_Text _moveSpeedValueText;

    [SerializeField] private Image[] _starImages;
    [SerializeField] private Button _starUpButton;
    [SerializeField] private TMP_Text _promoteRequirementText;

    [SerializeField] private Button _useItemButton;
    [SerializeField] private Button _closeButton;

    private bool _isSubscribed;
    private CharacterDetailViewModel _viewModel;

    public event Action OnUseItemButtonClicked;
    public event Action OnCloseButtonClicked;

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void Bind(CharacterDetailViewModel viewModel, string characterName)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: CharacterDetailViewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();

        _viewModel = viewModel;
        _nameText.text = characterName;

        TrySubscribe();
    }

    public void ConfirmUseItem(string itemId)
    {
        _viewModel.UseExpItemCommand(itemId);
    }

    private void HandleDisplayChanged()
    {
        RefreshView();
    }

    private void RefreshView()
    {
        _levelText.text = $"Lv.{_viewModel.CurrentLevel}";

        _expSlider.value = _viewModel.IsMaxLevel
            ? 1f : (float)_viewModel.CurrentExp / _viewModel.RequiredExpForNextLevel;
        _expText.text = _viewModel.IsMaxLevel
            ? "MAX" : $"{_viewModel.CurrentExp} / {_viewModel.RequiredExpForNextLevel}";

        _hpValueText.text = $"MaxHp  {_viewModel.DisplayHp}";
        _atkValueText.text = $"Atk  {_viewModel.DisplayAtk}";
        _defValueText.text = $"Def  {_viewModel.DisplayDef}";
        _atkSpeedValueText.text = $"AtkSpeed  {_viewModel.DisplayAtkSpeed:F2}";
        _moveSpeedValueText.text = $"MoveSpeed  {_viewModel.DisplayMoveSpeed:F2}";
        RefreshStars();

        _starUpButton.interactable = _viewModel.CanPromote;
        _promoteRequirementText.text = _viewModel.IsMaxStar
            ? "MAX" : $"{_viewModel.OwnedDuplicates}/{_viewModel.RequiredDuplicatesForPromotion}";
    }

    private void RefreshStars()
    {
        if (null == _starImages)
        {
            return;
        }

        for (int i = 0; i < _starImages.Length; i++)
        {
            _starImages[i].enabled = i < _viewModel.CurrentStar;
        }
    }

    private void TrySubscribe()
    {
        if (_isSubscribed || null == _viewModel || !isActiveAndEnabled)
        {
            return;
        }

        _viewModel.OnDisplayChanged += HandleDisplayChanged;

        _starUpButton.onClick.AddListener(HandleClickStarUp);
        _useItemButton.onClick.AddListener(HandleClickUseItem);
        _closeButton.onClick.AddListener(HandleClickClose);

        _isSubscribed = true;

        _viewModel.Initialize();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnDisplayChanged -= HandleDisplayChanged;
        _viewModel.Dispose();

        _starUpButton.onClick.RemoveListener(HandleClickStarUp);
        _useItemButton.onClick.RemoveListener(HandleClickUseItem);
        _closeButton.onClick.RemoveListener(HandleClickClose);

        _isSubscribed = false;
    }

    private void HandleClickStarUp()
    {
        _viewModel.PromoteCommand();
    }

    private void HandleClickUseItem()
    {
        OnUseItemButtonClicked?.Invoke();
    }

    private void HandleClickClose()
    {
        OnCloseButtonClicked?.Invoke();
    }
}