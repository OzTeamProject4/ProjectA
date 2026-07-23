using UnityEngine;
using UnityEngine.UI;

public sealed class PracticeFieldScreenView : BaseUI
{
    [SerializeField] private Button _backButton;

    private void OnEnable()
    {
        if (_backButton == null)
        {
            Debug.LogError(
                "[PracticeFieldScreenView] BackButton이 할당되지 않았습니다.");
            return;
        }

        _backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnDisable()
    {
        if (_backButton == null)
        {
            return;
        }

        _backButton.onClick.RemoveListener(OnBackButtonClicked);
    }

    private void OnBackButtonClicked()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "[PracticeFieldScreenView] GameManager.Instance가 존재하지 않습니다.");
            return;
        }

        UIManager uiManager = GameManager.Instance.UIManager;

        if (uiManager == null)
        {
            Debug.LogError(
                "[PracticeFieldScreenView] UIManager가 존재하지 않습니다.");
            return;
        }

        uiManager.ClosePracticeFieldScreen();
    }
}