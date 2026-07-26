using System;
using System.Collections.Generic;
using UnityEngine;

public class StudentManagementEquipmentView : MonoBehaviour
{
    [SerializeField] private List<EquipmentView> _equipmentViewList;

    private readonly Dictionary<EquipType, EquipmentView> _equipmentViewCacheDictionary = new Dictionary<EquipType, EquipmentView>();

    public event Action<EquipType> OnSlotClicked;

    private void Awake()
    {
        foreach (EquipmentView equipmentView in _equipmentViewList)
        {
            if (equipmentView == null)
            {
                Debug.LogError("EquipmentView 리스트에 비어있는 항목이 있습니다.");
                return;
            }

            if (!_equipmentViewCacheDictionary.TryAdd(equipmentView.EquipType, equipmentView))
            {
                Debug.LogError($"장비 타입 '{equipmentView.EquipType}'이 중복되어 캐싱에 실패했습니다.");
                return;
            }
        }
    }

    private void OnEnable()
    {
        foreach (EquipmentView slot in _equipmentViewList)
        {
            slot.OnButtonClicked += HandleSlotClicked;
        }
    }

    private void OnDisable()
    {
        foreach (EquipmentView slot in _equipmentViewList)
        {
            slot.OnButtonClicked -= HandleSlotClicked;
        }
    }

    public void ClearSlots()
    {
        foreach (EquipmentView equipmentView in _equipmentViewList)
        {
            if (equipmentView == null)
            {
                continue;
            }

            equipmentView.ClearView();
        }
    }

    public void UpdateEquipmentSlot(EquipmentModel equipmentModel)
    {
        if (equipmentModel == null)
        {
            Debug.LogError("장비 모델이 null입니다.", this);
            return;
        }

        if (!_equipmentViewCacheDictionary.TryGetValue(equipmentModel.EquipType, out EquipmentView equipmentView))
        {
            Debug.LogError($"'{equipmentModel.EquipType}' 슬롯을 찾을 수 없습니다.", this);
            return;
        }

        equipmentView.UpdateView(equipmentModel);
    }

    private void HandleSlotClicked(EquipType equipType)
    {
        if (OnSlotClicked == null)
        {
            return;
        }

        OnSlotClicked.Invoke(equipType);
    }
}
