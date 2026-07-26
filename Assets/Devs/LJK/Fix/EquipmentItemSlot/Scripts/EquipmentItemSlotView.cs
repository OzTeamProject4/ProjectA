using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentItemSlotView : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransfrom;
    [SerializeField] private Button _slotButton;
    [SerializeField] private Image _itemIconImage;
    [SerializeField] private Image _equippedCharacterIconImage;
    [SerializeField] private GameObject _selector;
    [SerializeField] private TMP_Text _equippedText;

    private EquipmentItemSlotViewModel _equipmentItemSlotViewModel;

    public event Action<EquipmentModel, RectTransform> OnSlotClicked;

    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_rectTransfrom, nameof(EquipmentItemSlotView), nameof(_rectTransfrom));
        UnityUtil.ValidateReference(_slotButton, nameof(EquipmentItemSlotView), nameof(_slotButton));
        UnityUtil.ValidateReference(_itemIconImage, nameof(EquipmentItemSlotView), nameof(_itemIconImage));

        _equipmentItemSlotViewModel = new EquipmentItemSlotViewModel();
    }

    public void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _equipmentItemSlotViewModel.PropertyChanged += OnPropertyChanged;
        _slotButton.onClick.AddListener(HandleSlotClicked);
    }

    public void OnDisable()
    {
        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        _equipmentItemSlotViewModel.PropertyChanged -= OnPropertyChanged;
        _slotButton.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        _equipmentItemSlotViewModel.Dispose();
        _equipmentItemSlotViewModel = null;
    }

    public void SetModel(EquipmentModel equipmentModel)
    {
        _equipmentItemSlotViewModel.SetModel(equipmentModel);
        _equipmentItemSlotViewModel.Refresh();

        RefreshEquippedMark();
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_equipmentItemSlotViewModel.Name):
                UpdateNameText();
                break;
            case nameof(_equipmentItemSlotViewModel.IconKey):
                RefreshIconImageAsync().Forget();
                break;
            case nameof(_equipmentItemSlotViewModel.EquippedBy):
                RefreshEquippedMark();
                break;
        }
    }

    private void UpdateNameText()
    {
        if (_equippedText == null)
        {
            return;
        }

        _equippedText.text = _equipmentItemSlotViewModel.Name;
    }

    private void RefreshEquippedMark()
    {
        bool isEquipped = _equipmentItemSlotViewModel.IsEquipped;

        if (_selector != null)
        {
            _selector.SetActive(isEquipped);
        }

        UpdateEquippedCharacterIconAsync().Forget();
    }

    private async UniTask RefreshIconImageAsync()
    {
        string iconKey = _equipmentItemSlotViewModel.IconKey;

        if (string.IsNullOrWhiteSpace(iconKey))
        {
            return;
        }

        Sprite iconSprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconKey, _disableCts.Token);

        if (iconSprite == null)
        {
            return;
        }

        _itemIconImage.sprite = iconSprite;
    }

    private async UniTaskVoid UpdateEquippedCharacterIconAsync()
    {
        if (_equippedCharacterIconImage == null)
        {
            return;
        }

        string portraitKey = _equipmentItemSlotViewModel.EquippedStudentPortraitKey;

        if (string.IsNullOrWhiteSpace(portraitKey))
        {
            _equippedCharacterIconImage.enabled = false;
            return;
        }

        Sprite portraitSprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(portraitKey, _disableCts.Token);

        if (portraitSprite == null)
        {
            _equippedCharacterIconImage.enabled = false;
            return;
        }

        _equippedCharacterIconImage.enabled = true;
        _equippedCharacterIconImage.sprite = portraitSprite;
    }

    private void HandleSlotClicked()
    {
        if (OnSlotClicked == null)
        {
            return;
        }

        OnSlotClicked.Invoke(_equipmentItemSlotViewModel.EquipmentModel, _rectTransfrom);
    }
}