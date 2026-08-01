using System.Collections.Generic;
using System.Threading;
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

    [Header("Skill")]
    [SerializeField] private SkillInfoView _basicSkillItemView;
    [SerializeField] private SkillInfoView _normalSkillItemView;
    [SerializeField] private SkillInfoView _ultimateSkillItemView;

    private void Awake()
    {
        UnityUtil.ValidateReference(_statTabButton, nameof(StudentManagementStatusView), nameof(_statTabButton));
        UnityUtil.ValidateReference(_skillTabButton, nameof(StudentManagementStatusView), nameof(_skillTabButton));
        UnityUtil.ValidateReference(_hpText, nameof(StudentManagementStatusView), nameof(_hpText));
        UnityUtil.ValidateReference(_attackText, nameof(StudentManagementStatusView), nameof(_attackText));
        UnityUtil.ValidateReference(_defenseText, nameof(StudentManagementStatusView), nameof(_defenseText));
        UnityUtil.ValidateReference(_moveSpeedText, nameof(StudentManagementStatusView), nameof(_moveSpeedText));
        UnityUtil.ValidateReference(_basicSkillItemView, nameof(StudentManagementStatusView), nameof(_basicSkillItemView));
        UnityUtil.ValidateReference(_normalSkillItemView, nameof(StudentManagementStatusView), nameof(_normalSkillItemView));
        UnityUtil.ValidateReference(_ultimateSkillItemView, nameof(StudentManagementStatusView), nameof(_ultimateSkillItemView));
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

    public void SetSkills(IReadOnlyList<CharacterSkillData> skills, CancellationToken cancellationToken)
    {
        _basicSkillItemView.Clear();
        _normalSkillItemView.Clear();
        _ultimateSkillItemView.Clear();

        foreach (CharacterSkillData skillData in skills)
        {
            switch (skillData.Category)
            {
                case CharacterSkillCategory.Basic:
                    _basicSkillItemView.SetSkill(skillData, cancellationToken);
                    break;
                case CharacterSkillCategory.Normal:
                    _normalSkillItemView.SetSkill(skillData, cancellationToken);
                    break;
                case CharacterSkillCategory.Ultimate:
                    _ultimateSkillItemView.SetSkill(skillData, cancellationToken);
                    break;
            }
        }
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