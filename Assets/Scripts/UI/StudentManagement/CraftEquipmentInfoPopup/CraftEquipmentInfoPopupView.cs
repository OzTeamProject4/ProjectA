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

    [Header("Stat")]
    [SerializeField] private EquipmentStatOptionView _statRowPrefab;
    [SerializeField] private Transform _statRowContent;

    private CraftEquipmentInfoPopupViewModel _craftEquipmentInfoPopupViewModel;

    private readonly List<EquipmentStatOptionView> _spawnedStatRowList = new List<EquipmentStatOptionView>();

    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_itemNameText, nameof(CraftEquipmentInfoPopupView), nameof(_itemNameText));
        UnityUtil.ValidateReference(_itemDescriptionText, nameof(CraftEquipmentInfoPopupView), nameof(_itemDescriptionText));
        UnityUtil.ValidateReference(_itemIconImage, nameof(CraftEquipmentInfoPopupView), nameof(_itemIconImage));
        UnityUtil.ValidateReference(_statRowPrefab, nameof(CraftEquipmentInfoPopupView), nameof(_statRowPrefab));
        UnityUtil.ValidateReference(_statRowContent, nameof(CraftEquipmentInfoPopupView), nameof(_statRowContent));

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
        RefreshDescriptionText();
        RefreshIconImageAsync().Forget();
        RefreshStatRows();
    }

    private void RefreshNameText()
    {
        _itemNameText.text = _craftEquipmentInfoPopupViewModel.Name;
    }

    private void RefreshDescriptionText()
    {
        _itemDescriptionText.text = _craftEquipmentInfoPopupViewModel.Description;
    }

    private UniTask RefreshIconImageAsync()
    {
        return SpriteLoader.LoadIntoAsync(_itemIconImage, _craftEquipmentInfoPopupViewModel.IconKey, _disableCts.Token);
    }

    public void MovePosition(Vector3 worldPosition)
    {
        _rootRect.pivot = new Vector2(1, 1);
        _rootRect.position = worldPosition;

        PopupPositioner.ClampInsideCanvas(_rootRect);
    }

    //TODO 슬롯 오브젝트 풀 사용 생성
    private void RefreshStatRows()
    {
        ReleaseStatRows();

        IReadOnlyList<StatDelta> statDeltas = _craftEquipmentInfoPopupViewModel.CreateStatDeltas();

        foreach (StatDelta statDelta in statDeltas)
        {
            EquipmentStatOptionView statRowView = Instantiate(_statRowPrefab, _statRowContent);

            statRowView.SetStat(statDelta);

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
}