using UnityEngine;
using UnityEngine.UI;

public class StageSelectScreenView : BaseUI
{
    [SerializeField] private Button _backToLobbyButton;

    private void OnEnable()
    {
        ResetRectTransform();

        if (_backToLobbyButton == null)
        {
            Debug.LogError("BackToLobbyButton is not assigned.");
            return;
        }

        _backToLobbyButton.onClick.AddListener(
            OnBackToLobbyButtonClicked);
    }

    private void OnDisable()
    {
        if (_backToLobbyButton == null)
        {
            return;
        }

        _backToLobbyButton.onClick.RemoveListener(
            OnBackToLobbyButtonClicked);
    }

    private void OnBackToLobbyButtonClicked()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null.");
            return;
        }

        if (GameManager.Instance.UIManager == null)
        {
            Debug.LogError("UIManager is null.");
            return;
        }

        GameManager.Instance.UIManager.CloseStageSelectScreen();
    }

    private void ResetRectTransform()
    {
        RectTransform rectTransform = transform as RectTransform;

        if (rectTransform == null)
        {
            Debug.LogError(
                "StageSelectScreen RectTransform was not found.");
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }
}