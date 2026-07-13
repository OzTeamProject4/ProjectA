using System;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotView : MonoBehaviour
{
    [SerializeField] private EquipType _slotType;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Button _slotButton;

    public EquipType SlotType
    {
        get { return _slotType; }
    }

    public event Action<EquipType> OnSlotClicked;

    private void OnEnable()
    {
        if (null != _slotButton)
        {
            _slotButton.onClick.AddListener(HandleClick);
        }
    }

    private void OnDisable()
    {
        if (null != _slotButton)
        {
            _slotButton.onClick.RemoveListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        OnSlotClicked?.Invoke(_slotType);
    }
}