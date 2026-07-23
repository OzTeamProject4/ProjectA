using UnityEngine;
using UnityEngine.UI;

public class SettingPopupView : BaseUI
{
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _applyButton;

    private void OnEnable()
    {
        ResetRectTransform();

        if (_cancelButton != null)
        {
            _cancelButton.onClick.AddListener(ClosePopup);
        }

        if (_applyButton != null)
        {
            _applyButton.onClick.AddListener(ClosePopup);
        }
    }

    private void OnDisable()
    {
        if (_cancelButton != null)
        {
            _cancelButton.onClick.RemoveListener(ClosePopup);
        }

        if (_applyButton != null)
        {
            _applyButton.onClick.RemoveListener(ClosePopup);
        }
    }

    private void ClosePopup()
    {
        GameManager.Instance.UIManager.CloseSettingPopup();
    }

    private void ResetRectTransform()
    {
        if (transform is not RectTransform rectTransform)
        {
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}