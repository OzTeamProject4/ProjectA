using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentInventoryPopupView : BaseUI
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private EquipmentItemSlotView _slotPrefab;
    [SerializeField] private Transform _content;

    private EquipmentInventoryPopupViewModel _equipmentInventoryPopupViewModel;

    private readonly List<EquipmentItemSlotView> _spawnedSlotList = new List<EquipmentItemSlotView>();

    //오브젝트 풀에 슬롯 미리 생성 (고려)
    private void Awake()
    {
        UnityUtil.ValidateReference(_slotPrefab, nameof(ExperienceInventoryPopupView), nameof(_slotPrefab));
        UnityUtil.ValidateReference(_content, nameof(ExperienceInventoryPopupView), nameof(_content));

        _equipmentInventoryPopupViewModel = new EquipmentInventoryPopupViewModel();
    }

    private void OnEnable()
    {
        _equipmentInventoryPopupViewModel.EquipmentsChanged += HandleEquipmentsChanged;
    }

    private void OnDisable()
    {
        _equipmentInventoryPopupViewModel.EquipmentsChanged -= HandleEquipmentsChanged;
    }

    private void OnDestroy()
    {
        _equipmentInventoryPopupViewModel.Dispose();
        _equipmentInventoryPopupViewModel = null;
    }

    private void HandleEquipmentsChanged()
    {
        RefreshSlots();
    }

    public void SetModel(EquipType equipType, StudentModel studentModel)
    {
        _equipmentInventoryPopupViewModel.SetModel(equipType, studentModel);
      
        Refresh(equipType);
    }

    private void Refresh(EquipType equipType)
    {
        RefreshTitleText(equipType);
        RefreshSlots();
    }

    private void RefreshTitleText(EquipType equipType)
    {
        _titleText.text = equipType.ToString();
    }

    //TODO 슬롯 오브젝트 풀 사용 생성
    private void RefreshSlots()
    {
        ReleaseSlots();

        foreach (EquipmentModel equipmentModel in _equipmentInventoryPopupViewModel.FilteredEquipmentItems)
        {
            EquipmentItemSlotView equipmentItemSlotView = Instantiate(_slotPrefab, _content);
            equipmentItemSlotView.SetModel(equipmentModel);
            equipmentItemSlotView.OnSlotClicked += HandleSlotClicked;
            _spawnedSlotList.Add(equipmentItemSlotView);
        }
    }

    //TODO 슬롯 오브젝트 풀 사용 해제
    private void ReleaseSlots()
    {
        foreach (EquipmentItemSlotView equipmentItemSlotView in _spawnedSlotList)
        {
            if (null == equipmentItemSlotView)
            {
                continue;
            }

            equipmentItemSlotView.OnSlotClicked -= HandleSlotClicked;
            Destroy(equipmentItemSlotView.gameObject);
        }

        _spawnedSlotList.Clear();
    }

    private void HandleSlotClicked(EquipmentModel equipmentModel, RectTransform itemRect)
    {
        Vector3[] corners = new Vector3[4];
        itemRect.GetWorldCorners(corners);

        Vector3 bottomLeft = corners[0];
        Vector3 bottomRight = corners[3];

        Vector3 bottomCenter = (bottomLeft + bottomRight) * 0.5f;

        GameManager.Instance.UIManager.OpenEquipmentInfoPopupAsync(_equipmentInventoryPopupViewModel.StudentModel, equipmentModel, bottomCenter).Forget();
    }
}