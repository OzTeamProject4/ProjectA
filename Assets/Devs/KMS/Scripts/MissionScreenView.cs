using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionScreenView : BaseUI
{
    [SerializeField] private Button _backToLobbyButton;

    [SerializeField] private Button _allMissionTabButton;
    [SerializeField] private Button _scenarioMissionTabButton;
    [SerializeField] private Button _dailyMissionTabButton;
    [SerializeField] private Button _weeklyMissionTabButton;

    [SerializeField] private TMP_Text _missionContentText;
    [SerializeField] private Button _completeButton;

    private void OnEnable()
    {
        ResetRectTransform();

        if (HasMissingReference())
        {
            return;
        }

        RegisterButtonEvents();
        ShowAllMission();
    }

    private void OnDisable()
    {
        UnRegisterButtonEvents();
    }

    private void RegisterButtonEvents()
    {
        _backToLobbyButton.onClick.AddListener(OnBackToLobbyButtonClicked);

        _allMissionTabButton.onClick.AddListener(ShowAllMission);
        _scenarioMissionTabButton.onClick.AddListener(ShowScenarioMission);
        _dailyMissionTabButton.onClick.AddListener(ShowDailyMission);
        _weeklyMissionTabButton.onClick.AddListener(ShowWeeklyMission);

        _completeButton.onClick.AddListener(OnCompleteButtonClicked);
    }

    private void UnRegisterButtonEvents()
    {
        if (_backToLobbyButton == null)
        {
            return;
        }

        _backToLobbyButton.onClick.RemoveListener(OnBackToLobbyButtonClicked);

        _allMissionTabButton.onClick.RemoveListener(ShowAllMission);
        _scenarioMissionTabButton.onClick.RemoveListener(ShowScenarioMission);
        _dailyMissionTabButton.onClick.RemoveListener(ShowDailyMission);
        _weeklyMissionTabButton.onClick.RemoveListener(ShowWeeklyMission);

        _completeButton.onClick.RemoveListener(OnCompleteButtonClicked);
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

        GameManager.Instance.UIManager.CloseMissionScreen();
    }

    private void ShowAllMission()
    {
        SetMissionContent("전체 미션입니다.");
    }

    private void ShowScenarioMission()
    {
        SetMissionContent("시나리오 미션입니다.");
    }

    private void ShowDailyMission()
    {
        SetMissionContent("매일 미션입니다.");
    }

    private void ShowWeeklyMission()
    {
        SetMissionContent("주간 미션입니다.");
    }

    private void OnCompleteButtonClicked()
    {
        _missionContentText.text = "미션을 완료했습니다.";
        _completeButton.interactable = false;

        Debug.Log("Mission Completed.");
    }

    private void SetMissionContent(string missionContent)
    {
        _missionContentText.text = missionContent;
        _completeButton.interactable = true;
    }

    private void ResetRectTransform()
    {
        if (transform is not RectTransform rectTransform)
        {
            Debug.LogError("MissionScreen RectTransform was not found.");
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }

    private bool HasMissingReference()
    {
        if (_backToLobbyButton == null ||
            _allMissionTabButton == null ||
            _scenarioMissionTabButton == null ||
            _dailyMissionTabButton == null ||
            _weeklyMissionTabButton == null ||
            _missionContentText == null ||
            _completeButton == null)
        {
            Debug.LogError("MissionScreen reference is not assigned.");
            return true;
        }

        return false;
    }
}