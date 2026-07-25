using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftEquipmentInfoPopupView : BaseUI
{
    [SerializeField] private RectTransform _rootRect;

    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _itemDescriptionText;
    [SerializeField] private Image _itemIconImage;

    //TODO List로
    [SerializeField] private StatItemView[] _statRows;

    private CraftEquipmentInfoPopupViewModel _craftEquipmentInfoPopupViewModel;

    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_itemNameText, nameof(CraftEquipmentInfoPopupView), nameof(_itemNameText));
        UnityUtil.ValidateReference(_itemDescriptionText, nameof(CraftEquipmentInfoPopupView), nameof(_itemDescriptionText));
        UnityUtil.ValidateReference(_itemIconImage, nameof(CraftEquipmentInfoPopupView), nameof(_itemIconImage));

        _craftEquipmentInfoPopupViewModel = new CraftEquipmentInfoPopupViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        if (_disableCts == null)
        {
            return;
        }

        _disableCts.Cancel();
        _disableCts.Dispose();
        _disableCts = null;
    }

    public void SetModel(EquipmentCraftModel equipmentModel, Vector3 position)
    {
        _craftEquipmentInfoPopupViewModel.SetModel(equipmentModel);

        Refresh();
        MovePosition(position);
    }

    private void Refresh()
    {
        RefreshNameText();
        RefreshIconImageAsync().Forget();
        RefreshStatRows();
    }

    private void RefreshNameText()
    {
        _itemNameText.text = _craftEquipmentInfoPopupViewModel.Name;
    }

    private async UniTask RefreshIconImageAsync()
    {
        string iconkey = _craftEquipmentInfoPopupViewModel.IconKey;

        if (string.IsNullOrWhiteSpace(iconkey))
        {
            return;
        }

        Sprite iconSprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(iconkey, _disableCts.Token);

        if (iconSprite == null)
        {
            return;
        }

        _itemIconImage.sprite = iconSprite;
    }

    public void MovePosition(Vector3 worldPosition)
    {
        _rootRect.pivot = new Vector2(1, 1);
        _rootRect.position = worldPosition;
    }

    //TODO List로 바꾸기 기능 수정 값이 0이면 끄기
    private void RefreshStatRows()
    {
        IReadOnlyList<StatInfo> statInfos = _craftEquipmentInfoPopupViewModel.StatInfos;

        for (int i = 0; i < _statRows.Length; i++)
        {
            if (null == _statRows[i])
            {
                continue;
            }

            if (i >= statInfos.Count)
            {
                _statRows[i].Hide();
                continue;
            }

            _statRows[i].SetValue(statInfos[i].Type, statInfos[i].Value);
        }
    }
}