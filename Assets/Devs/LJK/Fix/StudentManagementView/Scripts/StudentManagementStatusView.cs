using TMPro;
using UnityEngine;

public class StudentManagementStatusView : MonoBehaviour
{
    [Header("Tab")]
    [SerializeField] private TabButton _statTabButton;
    [SerializeField] private TabButton _skillTabButton;

    [Header("Stat")]
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _attackText;
    [SerializeField] private TMP_Text _defenseText;
    [SerializeField] private TMP_Text _moveSpeedText;

    private void OnEnable()
    {
        _statTabButton.OnTabClicked += HandleStatTabClicked;
        _skillTabButton.OnTabClicked += HandleSkillTabClicked;
    }

    private void OnDisable()
    {
        _statTabButton.OnTabClicked -= HandleStatTabClicked;
        _skillTabButton.OnTabClicked -= HandleSkillTabClicked;
    }

    //TODO 스탯별 문자열 수정
    public void UpdateHpText(float value)
    {
        _hpText.text = value.ToString();
    }

    public void UpdateAttackText(float value)
    {
        _attackText.text = value.ToString();
    }

    public void UpdateDefenseText(float value)
    {
        _defenseText.text = value.ToString();
    }

    public void UpdateMoveSpeedText(float value)
    {
        _moveSpeedText.text = value.ToString();
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