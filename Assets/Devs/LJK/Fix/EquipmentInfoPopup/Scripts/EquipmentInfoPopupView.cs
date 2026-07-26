using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInfoPopupView : BaseUI
{
    [SerializeField] private RectTransform _rootRect;
    [SerializeField] private PopupBackgroundButton _backgroundButton;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _itemDescriptionText;
    [SerializeField] private Image _itemIconImage;

    [Header("Stat")]
    [SerializeField] private EquipmentStatOptionView _statRowPrefab;
    [SerializeField] private Transform _statRowContent;

    [SerializeField] private Button _equipButton;
    [SerializeField] private Button _unequipButton;

    private EquipmentInfoPopupViewModel _equipmentInfoPopupViewModel;

    private readonly List<EquipmentStatOptionView> _spawnedStatRowList = new List<EquipmentStatOptionView>();

    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_itemNameText, nameof(EquipmentInfoPopupView), nameof(_itemNameText));
        UnityUtil.ValidateReference(_itemDescriptionText, nameof(EquipmentInfoPopupView), nameof(_itemDescriptionText));
        UnityUtil.ValidateReference(_itemIconImage, nameof(EquipmentInfoPopupView), nameof(_itemIconImage));
        UnityUtil.ValidateReference(_backgroundButton, nameof(EquipmentInfoPopupView), nameof(_backgroundButton));
        UnityUtil.ValidateReference(_statRowPrefab, nameof(EquipmentInfoPopupView), nameof(_statRowPrefab));
        UnityUtil.ValidateReference(_statRowContent, nameof(EquipmentInfoPopupView), nameof(_statRowContent));

        _equipmentInfoPopupViewModel = new EquipmentInfoPopupViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _equipmentInfoPopupViewModel.PropertyChanged += OnModelPropertyChanged;

        _equipButton.onClick.AddListener(HandleEquipClicked);
        _unequipButton.onClick.AddListener(HandleUnequipClicked);

        _backgroundButton.OnBackgroundClicked += HandleBackgroundClicked;
    }

    private void OnDisable()
    {
        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }
    
        _equipmentInfoPopupViewModel.PropertyChanged -= OnModelPropertyChanged;

        _equipButton.onClick.RemoveAllListeners();
        _unequipButton.onClick.RemoveAllListeners();

        _backgroundButton.OnBackgroundClicked -= HandleBackgroundClicked;
    }

    private void HandleBackgroundClicked()
    {
        GameManager.Instance.UIManager.CloseEquipmentInfoPopup();
    }

    private void OnDestroy()
    {
        _equipmentInfoPopupViewModel.Dispose();
        _equipmentInfoPopupViewModel = null;
    }

    public void SetModel(EquipmentModel equipmentModel, StudentModel studentModel, Vector3 position)
    {
        _equipmentInfoPopupViewModel.SetModel(equipmentModel, studentModel);

        Refresh();
        MoveCardTo(position);
    }

    private void Refresh()
    {
        _itemNameText.text = _equipmentInfoPopupViewModel.Name;
        _itemDescriptionText.text = _equipmentInfoPopupViewModel.Description;

        UpdateIconImageAsync().Forget();
        RefreshStatRows();
        RefreshEquipButtons();
    }

    private void RefreshEquipButtons()
    {
        bool isEquipped = _equipmentInfoPopupViewModel.IsEquippedByCurrentStudent;

        _equipButton.gameObject.SetActive(!isEquipped);
        _unequipButton.gameObject.SetActive(isEquipped);
    }

    private void OnModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_equipmentInfoPopupViewModel.Name):
                _itemNameText.text = _equipmentInfoPopupViewModel.Name;
                break;
            case nameof(_equipmentInfoPopupViewModel.IconKey):
                UpdateIconImageAsync().Forget();
                break;
            case nameof(EquipmentModel.EquippedBy):
                RefreshEquipButtons();
                break;
        }
    }

    private UniTask UpdateIconImageAsync()
    {
        return SpriteLoader.LoadIntoAsync(_itemIconImage, _equipmentInfoPopupViewModel.IconKey, _disableCts.Token);
    }

    public void MoveCardTo(Vector3 worldPosition)
    {
        _rootRect.pivot = new Vector2(1, 1);
        _rootRect.position = worldPosition;
    }

    //TODO 슬롯 오브젝트 풀 사용 생성
    private void RefreshStatRows()
    {
        ReleaseStatRows();

        IReadOnlyList<StatInfo> statInfos = _equipmentInfoPopupViewModel.StatInfo;

        if (statInfos == null)
        {
            return;
        }

        foreach (StatInfo statInfo in statInfos)
        {
            if (Mathf.Approximately(statInfo.Value, 0f))
            {
                continue;
            }

            EquipmentStatOptionView statRowView = Instantiate(_statRowPrefab, _statRowContent);

            statRowView.SetValue(statInfo.Type, statInfo.Value);

            _spawnedStatRowList.Add(statRowView);
        }
    }

    //TODO 슬롯 오브젝트 풀 사용 해제
    private void ReleaseStatRows()
    {
        foreach (EquipmentStatOptionView statRowView in _spawnedStatRowList)
        {
            if (statRowView == null)
            {
                continue;
            }

            Destroy(statRowView.gameObject);
        }

        _spawnedStatRowList.Clear();
    }

    private void HandleEquipClicked()
    {
        _equipmentInfoPopupViewModel.RequestEquip();
    }

    private void HandleUnequipClicked()
    {
        _equipmentInfoPopupViewModel.RequestUnequip();
    }
}