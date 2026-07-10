using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpItemSelectPopupView : MonoBehaviour
{
    [SerializeField] private ExpItemSlotView _slotPrefab;
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private Button _closeButton;

    private readonly List<ExpItemSlotView> _spawnedSlots = new();

    public event Action<string> OnItemSelected;
    public event Action OnCloseButtonClicked;

    public void Bind(ExpItemSelectPopupViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: ItemSelectPopupViewModel 이 null 입니다.");
            return;
        }

        if (null == _slotPrefab || null == _slotContainer)
        {
            Debug.LogError("Bind: 슬롯 프리팹 또는 컨테이너가 연결되지 않았습니다.");
            return;
        }

        ClearSlots();

        foreach (ExpItemSlotViewModel slotViewModel in viewModel.Items)
        {
            ExpItemSlotView slotView = Instantiate(_slotPrefab, _slotContainer);
            slotView.Bind(slotViewModel);
            slotView.OnClicked += HandleSlotClicked;

            _spawnedSlots.Add(slotView);
        }

        if (null != _closeButton)
        {
            _closeButton.onClick.AddListener(HandleCloseClicked);
        }
    }

    public void RefreshSlots()
    {
        foreach (ExpItemSlotView slot in _spawnedSlots)
        {
            if (null != slot)
            {
                slot.RefreshDisplay();
            }
        }
    }

    private void OnDestroy()
    {
        ClearSlots();

        if (null != _closeButton)
        {
            _closeButton.onClick.RemoveListener(HandleCloseClicked);
        }
    }

    private void ClearSlots()
    {
        foreach (ExpItemSlotView slot in _spawnedSlots)
        {
            if (null == slot)
            {
                continue;
            }

            slot.OnClicked -= HandleSlotClicked;
            Destroy(slot.gameObject);
        }

        _spawnedSlots.Clear();
    }

    private void HandleSlotClicked(string dataId)
    {
        OnItemSelected?.Invoke(dataId);
    }

    private void HandleCloseClicked()
    {
        OnCloseButtonClicked?.Invoke();
    }
}
