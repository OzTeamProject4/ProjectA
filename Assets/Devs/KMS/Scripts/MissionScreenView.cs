using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MissionScreenView : BaseUI
{
    [SerializeField] private Button _backToLobbyButton;
    [SerializeField] private Button _homeButton;

    [SerializeField] private Button _allMissionTabButton;
    [SerializeField] private Button _scenarioMissionTabButton;
    [SerializeField] private Button _dailyMissionTabButton;
    [SerializeField] private Button _weeklyMissionTabButton;

    [SerializeField] private Transform _missionContent;
    [SerializeField] private MissionSlotView _missionSlotTemplate;


    [SerializeField] private Color _normalTabColor = Color.white;
    [SerializeField] private Color _selectedTabColor = new Color32(170, 240, 110, 255);



    private readonly List<MissionSlotView> _spawnedSlots = new();


    private void OnEnable()
    {
        ResetRectTransform();

        if (HasMissingReference())
        {
            return;
        }

        CheckMissionData();

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
        _homeButton.onClick.AddListener(OnBackToLobbyButtonClicked);
        _allMissionTabButton.onClick.AddListener(ShowAllMission);
        _scenarioMissionTabButton.onClick.AddListener(ShowScenarioMission);
        _dailyMissionTabButton.onClick.AddListener(ShowDailyMission);
        _weeklyMissionTabButton.onClick.AddListener(ShowWeeklyMission);
    }

    private void UnRegisterButtonEvents()
    {
        if (_backToLobbyButton == null)
        {
            return;
        }

        _backToLobbyButton.onClick.RemoveListener(OnBackToLobbyButtonClicked);
        _homeButton.onClick.RemoveListener(OnBackToLobbyButtonClicked);
        _allMissionTabButton.onClick.RemoveListener(ShowAllMission);
        _scenarioMissionTabButton.onClick.RemoveListener(ShowScenarioMission);
        _dailyMissionTabButton.onClick.RemoveListener(ShowDailyMission);
        _weeklyMissionTabButton.onClick.RemoveListener(ShowWeeklyMission);
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
        SetSelectedTab(_allMissionTabButton);
        RefreshMissionSlots(null);
    }

    private void ShowScenarioMission()
    {
        SetSelectedTab(_scenarioMissionTabButton);
        RefreshMissionSlots("Scenario");
    }

    private void ShowDailyMission()
    {
        SetSelectedTab(_dailyMissionTabButton);
        RefreshMissionSlots("Daily");
    }

    private void ShowWeeklyMission()
    {
        SetSelectedTab(_weeklyMissionTabButton);
        RefreshMissionSlots("Weekly");
    }

    private void RefreshMissionSlots(string category)
    {
        ClearMissionSlots();

        if (!GameManager.Instance.DataManager.TryGetDataTable(
            out Dictionary<string, MissionData> missionTable))
        {
            Debug.LogError("미션 데이터를 찾을 수 없습니다.");
            return;
        }

        foreach (MissionData mission in missionTable.Values)
        {
            if (category != null && mission.Category != category)
            {
                continue;
            }

            MissionSlotView slot =
                Instantiate(_missionSlotTemplate, _missionContent);

            slot.Bind(mission);
            slot.gameObject.SetActive(true);

            _spawnedSlots.Add(slot);
        }
    }

    private void ClearMissionSlots()
    {
        foreach (MissionSlotView slot in _spawnedSlots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }

        _spawnedSlots.Clear();
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
            _missionContent == null ||
            _missionSlotTemplate == null)
        {
            Debug.LogError("MissionScreen reference is not assigned.");
            return true;
        }

        return false;
    }

    private void CheckMissionData()
    {
        if (GameManager.Instance.DataManager.TryGetDataTable(
            out Dictionary<string, MissionData> missionTable))
        {
            Debug.Log($"미션 데이터 개수: {missionTable.Count}");
        }
    }

    private void SetSelectedTab(Button selectedButton)
    {
        SetTabColor(
            _allMissionTabButton,
            _allMissionTabButton == selectedButton);

        SetTabColor(
            _scenarioMissionTabButton,
            _scenarioMissionTabButton == selectedButton);

        SetTabColor(
            _dailyMissionTabButton,
            _dailyMissionTabButton == selectedButton);

        SetTabColor(
            _weeklyMissionTabButton,
            _weeklyMissionTabButton == selectedButton);
    }

    private void SetTabColor(Button button, bool isSelected)
    {
        if (button == null || button.targetGraphic == null)
        {
            return;
        }

        button.targetGraphic.color =
            isSelected ? _selectedTabColor : _normalTabColor;
    }

}
