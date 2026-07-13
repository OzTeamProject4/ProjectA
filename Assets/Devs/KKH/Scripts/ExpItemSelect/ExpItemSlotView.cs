using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpItemSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Button _selectButton;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _unusableAlpha = 0.4f;

    private ExpItemSlotViewModel _viewModel;
    private bool _isSubscribed;

    public event Action<string> OnClicked;

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void Bind(ExpItemSlotViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: ExpItemSlotViewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();

        _viewModel = viewModel;
        _nameText.text = _viewModel.Name;

        Subscribe();
    }

    private void RefreshDisplay()
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

    private void Subscribe()
    {
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnChanged += HandleChanged;
        _selectButton.onClick.AddListener(HandleClick);
        _isSubscribed = true;

        _viewModel.Initialize();
        RefreshDisplay();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnChanged -= HandleChanged;
        _viewModel.Dispose();
        _selectButton.onClick.RemoveListener(HandleClick);
        _isSubscribed = false;
    }

    private void HandleChanged()
    {
        RefreshDisplay();
    }

    private void HandleClick()
    {
        OnClicked?.Invoke(_viewModel.DataId);
    }

}
