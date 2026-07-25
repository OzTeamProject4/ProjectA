using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInfoPopupView : BaseUI
{
    [SerializeField] private RectTransform _rootRect;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _itemDescriptionText;
    [SerializeField] private Image _itemIconImage;

    //TODO ADD하는 식으로 수정
    [SerializeField] private StatItemView[] _statRows;

    [SerializeField] private Button _equipButton;
    [SerializeField] private Button _unequipButton;

    private EquipmentInfoPopupViewModel _equipmentInfoPopupViewModel;

    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_itemNameText, nameof(EquipmentInfoPopupView), nameof(_itemNameText));
        UnityUtil.ValidateReference(_itemDescriptionText, nameof(EquipmentInfoPopupView), nameof(_itemDescriptionText));
        UnityUtil.ValidateReference(_itemIconImage, nameof(EquipmentInfoPopupView), nameof(_itemIconImage));

        _equipmentInfoPopupViewModel = new EquipmentInfoPopupViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _equipmentInfoPopupViewModel.PropertyChanged += OnModelPropertyChanged;
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
    }

    private void OnDestroy()
    {
        _equipmentInfoPopupViewModel.Dispose();
        _equipmentInfoPopupViewModel = null;
    }

    public void SetModel(EquipmentModel equipmentModel, StudentModel studentModel, Vector3 position)
    {
        _equipmentInfoPopupViewModel.SetModel(equipmentModel, studentModel);

        _equipButton.onClick.AddListener(HandleEquipClicked);
        _unequipButton.onClick.AddListener(HandleUnequipClicked);

        Refresh();
        MoveCardTo(position);
    }

    private void Refresh()
    {
        UpdateIconImageAsync().Forget();
        RefreshStatRows();
    }

    //TODO 필요한 기능 추가
    private void OnModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            //case nameof(_equipmentInfoPopupViewModel.IconPath):
            //    //UpdateList();
            //    break;
        }
    }

    private async UniTask UpdateIconImageAsync()
    {
        string iconKey = _equipmentInfoPopupViewModel.IconKey;

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

    public void MoveCardTo(Vector3 worldPosition)
    {
        _rootRect.pivot = new Vector2(1, 1);
        _rootRect.position = worldPosition;
    }

    //List로 바꾸기
    private void RefreshStatRows()
    {
        IReadOnlyList<StatInfo> info = _equipmentInfoPopupViewModel.StatInfo;

        for (int i = 0; i < _statRows.Length; i++)
        {
            if (null == _statRows[i])
            {
                continue;
            }

            if (i >= info.Count)
            {
                _statRows[i].Hide();
                continue;
            }

            _statRows[i].SetValue(info[i].Type, info[i].Value);
        }
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