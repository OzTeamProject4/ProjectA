using TMPro;
using UnityEngine;

public class StudentManagementStatusView : MonoBehaviour
{
    private const string StatValueFormat = "0.##";

    [Header("Tab")]
    [SerializeField] private TabButton _statTabButton;
    [SerializeField] private TabButton _skillTabButton;

    [Header("Stat")]
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _attackText;
    [SerializeField] private TMP_Text _defenseText;
    [SerializeField] private TMP_Text _moveSpeedText;

    private void Awake()
    {
        UnityUtil.ValidateReference(_statTabButton, nameof(StudentManagementStatusView), nameof(_statTabButton));
        UnityUtil.ValidateReference(_skillTabButton, nameof(StudentManagementStatusView), nameof(_skillTabButton));
        UnityUtil.ValidateReference(_hpText, nameof(StudentManagementStatusView), nameof(_hpText));
        UnityUtil.ValidateReference(_attackText, nameof(StudentManagementStatusView), nameof(_attackText));
        UnityUtil.ValidateReference(_defenseText, nameof(StudentManagementStatusView), nameof(_defenseText));
        UnityUtil.ValidateReference(_moveSpeedText, nameof(StudentManagementStatusView), nameof(_moveSpeedText));
    }

    private void OnEnable()
    {
        _statTabButton.OnTabClicked += HandleStatTabClicked;
        _skillTabButton.OnTabClicked += HandleSkillTabClicked;

        HandleStatTabClicked();
    }

    private void OnDisable()
    {
        _statTabButton.OnTabClicked -= HandleStatTabClicked;
        _skillTabButton.OnTabClicked -= HandleSkillTabClicked;
    }

    public void UpdateHpText(float value)
    {
        _hpText.text = value.ToString(StatValueFormat);
    }

    public void UpdateAttackText(float value)
    {
        _attackText.text = value.ToString(StatValueFormat);
    }

    public void UpdateDefenseText(float value)
    {
        _defenseText.text = value.ToString(StatValueFormat);
    }

    public void UpdateMoveSpeedText(float value)
    {
        _moveSpeedText.text = value.ToString(StatValueFormat);
    }

    private void HandleStatTabClicked()
    {
        SetActiveTab(_statTabButton, _skillTabButton);
    }

    private void HandleSkillTabClicked()
    {
        SetActiveTab(_skillTabButton, _statTabButton);
    }

    private void SetActiveTab(TabButton selectedTab, TabButton otherTab)
    {
        selectedTab.SetPanelActive(true);
        otherTab.SetPanelActive(false);
    }
}