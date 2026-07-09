using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Button _selectButton;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _unusableAlpha = 0.4f;

    private ItemSlotViewModel _viewModel;

    public event Action<string> OnClicked;

    public void Bind(ItemSlotViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: ItemSlotViewModel 이 null 입니다.");
            return;
        }

        _viewModel = viewModel;

        RefreshDisplay();

        _selectButton.onClick.AddListener(HandleClick);
    }

    public void RefreshDisplay()
    {
        if (null == _viewModel)
        {
            return;
        }

        _countText.text = _viewModel.OwnedCount.ToString();

        if (null != _canvasGroup)
        {
            _canvasGroup.alpha = _viewModel.IsUsable ? 1f : _unusableAlpha;
        }

        _selectButton.interactable = _viewModel.IsUsable;
    }


    private void OnDestroy()
    {
        if (null != _selectButton)
        {
            _selectButton.onClick.RemoveListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        OnClicked?.Invoke(_viewModel.ItemId);
    }
}
