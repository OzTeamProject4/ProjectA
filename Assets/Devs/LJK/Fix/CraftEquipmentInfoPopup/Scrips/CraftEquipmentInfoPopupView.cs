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

    // 장비마다 붙는 스탯 종류가 달라 고정 칸 대신 필요한 만큼 생성한다
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

    //TODO 슬롯 오브젝트 풀 사용 생성
    private void RefreshStatRows()
    {
        ReleaseStatRows();

        IReadOnlyList<StatInfo> statInfos = _craftEquipmentInfoPopupViewModel.StatInfos;

        if (statInfos == null)
        {
            return;
        }

        foreach (StatInfo statInfo in statInfos)
        {
            // 0인 스탯은 이 장비가 올려주지 않는 항목이라 표시하지 않는다
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
}