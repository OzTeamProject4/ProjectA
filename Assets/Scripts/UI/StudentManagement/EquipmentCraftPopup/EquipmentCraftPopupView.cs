using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentCraftPopupView : BaseUI
{
    [SerializeField] private EquipmentCraftSlotView _slotPrefab;
    [SerializeField] private Transform _content;

    private readonly List<EquipmentCraftSlotView> _spawnedSlotList = new List<EquipmentCraftSlotView>();

    private EquipmentCraftPopupViewModel _equipmentCraftPopupViewModel;

    private void Awake()
    {
        _equipmentCraftPopupViewModel = new EquipmentCraftPopupViewModel();
    }

    private void OnDestroy()
    {
        _equipmentCraftPopupViewModel.Dispose();
        _equipmentCraftPopupViewModel = null;
    }

    public void SetEquipType(EquipType equipType)
    {
        _equipmentCraftPopupViewModel.UpdateEquipmentCraftModels(equipType);
        RefreshSlots();
    }

    //TODO 슬롯 오브젝트 풀 사용 생성
    private void RefreshSlots()
    {
        ReleaseSlots();

        foreach (EquipmentCraftModel equipmentCraftModel in _equipmentCraftPopupViewModel.FilteredEquipmentCraftModels)
        {
            EquipmentCraftSlotView equipmentCraftSlotView = Instantiate(_slotPrefab, _content);
            equipmentCraftSlotView.SetModel(equipmentCraftModel);
            _spawnedSlotList.Add(equipmentCraftSlotView);
        }
    }

    //TODO 슬롯 오브젝트 풀 사용 해제
    private void ReleaseSlots()
    {
        foreach (EquipmentCraftSlotView equipmentCraftSlotView in _spawnedSlotList)
        {
            if (null == equipmentCraftSlotView)
            {
                continue;
            }

            Destroy(equipmentCraftSlotView.gameObject);
        }

        _spawnedSlotList.Clear();
    }
}