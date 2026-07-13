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

    [SerializeField] private Button _statTabButton;
    [SerializeField] private Button _equipmentTabButton;
    [SerializeField] private GameObject _statPanel;
    [SerializeField] private GameObject _equipmentPanel;

    [SerializeField] private EquipmentSlotView[] _equipmentSlots;

    private bool _isSubscribed;
    private CharacterDetailViewModel _viewModel;

    public event Action OnUseItemButtonClicked;
    public event Action OnCloseButtonClicked;
    public event Action<EquipType> OnEquipmentSlotClicked;

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

        if (null != _statTabButton)
        {
            _statTabButton.onClick.AddListener(HandleClickStatTab);
        }

        if (null != _equipmentTabButton)
        {
            _equipmentTabButton.onClick.AddListener(HandleClickEquipmentTab);
        }

        SubscribeEquipmentSlots();

        _isSubscribed = true;

        _viewModel.Initialize();

        ShowStatTab();
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

        if (null != _statTabButton)
        {
            _statTabButton.onClick.RemoveListener(HandleClickStatTab);
        }

        if (null != _equipmentTabButton)
        {
            _equipmentTabButton.onClick.RemoveListener(HandleClickEquipmentTab);
        }

        UnsubscribeEquipmentSlots();

        _isSubscribed = false;
    }

    private void SubscribeEquipmentSlots()
    {
        if (null == _equipmentSlots)
        {
            return;
        }

        foreach (EquipmentSlotView slot in _equipmentSlots)
        {
            if (null != slot)
            {
                slot.OnSlotClicked += HandleEquipmentSlotClicked;
            }
        }
    }

    private void UnsubscribeEquipmentSlots()
    {
        if (null == _equipmentSlots)
        {
            return;
        }

        foreach (EquipmentSlotView slot in _equipmentSlots)
        {
            if (null != slot)
            {
                slot.OnSlotClicked -= HandleEquipmentSlotClicked;
            }
        }
    }

    private void HandleEquipmentSlotClicked(EquipType slotType)
    {
        OnEquipmentSlotClicked?.Invoke(slotType);
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

    private void HandleClickStatTab()
    {
        ShowStatTab();
    }

    private void HandleClickEquipmentTab()
    {
        ShowEquipmentTab();
    }

    private void ShowStatTab()
    {
        if (null != _statPanel)
        {
            _statPanel.SetActive(true);
        }

        if (null != _equipmentPanel)
        {
            _equipmentPanel.SetActive(false);
        }
    }

    private void ShowEquipmentTab()
    {
        if (null != _statPanel)
        {
            _statPanel.SetActive(false);
        }

        if (null != _equipmentPanel)
        {
            _equipmentPanel.SetActive(true);
        }
    }
}