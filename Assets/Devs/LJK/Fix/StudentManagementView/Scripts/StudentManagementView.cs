using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class StudentManagementView : BaseUI
{
    [SerializeField] private StudentManagementExperienceView _studentManagementExperienceView;
    [SerializeField] private StudentManagementInfoView _studentManagementInfoView;
    [SerializeField] private StudentManagementEquipmentView _studentManagementEquipmentView;
    [SerializeField] private StudentManagementStatusView _studentManagementStatusView;

    private StudentManagementViewModel _studentManagementViewModel;

    private void Awake()
    {
        UnityUtil.ValidateReference(_studentManagementExperienceView, nameof(StudentManagementExperienceView), nameof(_studentManagementExperienceView));
        UnityUtil.ValidateReference(_studentManagementInfoView, nameof(StudentManagementExperienceView), nameof(_studentManagementInfoView));
        UnityUtil.ValidateReference(_studentManagementEquipmentView, nameof(StudentManagementExperienceView), nameof(_studentManagementEquipmentView));
        UnityUtil.ValidateReference(_studentManagementStatusView, nameof(StudentManagementExperienceView), nameof(_studentManagementStatusView));

        _studentManagementViewModel = new StudentManagementViewModel();
    }

    private CancellationTokenSource _disableCts;

    private void OnEnable()
    {
        _studentManagementExperienceView.OnOpenExperienceInventoryClicked += HandleOpenExperienceInventoryClicked;
        _studentManagementInfoView.OnGradeUpClicked += HandleGradeUpClicked;
        _studentManagementEquipmentView.OnSlotClicked += HandleEquipmentSlotClicked;

        _disableCts = new CancellationTokenSource();
        _studentManagementViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnDisable()
    {
        _studentManagementExperienceView.OnOpenExperienceInventoryClicked -= HandleOpenExperienceInventoryClicked;
        _studentManagementInfoView.OnGradeUpClicked -= HandleGradeUpClicked;
        _studentManagementEquipmentView.OnSlotClicked -= HandleEquipmentSlotClicked;

        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        _studentManagementViewModel.PropertyChanged -= OnPropertyChanged;
    }

    private void OnDestroy()
    {
        _studentManagementViewModel.Dispose();
        _studentManagementViewModel = null;
    }

    public void SetModel(StudentModel studentModel)
    {
        _studentManagementViewModel.SetModel(studentModel);
        _studentManagementViewModel.Refresh();
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_studentManagementViewModel.Name):
                HandleStudentNameChanged();
                break;
            case nameof(_studentManagementViewModel.Star):
                HandleStudentStarChanged();
                break;
            case nameof(_studentManagementViewModel.FullBodyKey):
                HandleStudentFullBodyImageChanged();
                break;
            case nameof(_studentManagementViewModel.TotalExperience):
                HandleStudentTotalExperienceChanged();
                break;
            case nameof(_studentManagementViewModel.Level):
                HandleStudentLevelChanged();
                break;
            case nameof(_studentManagementViewModel.IsMaxLevel):
                HandleStudentIsMaxLevelChanged();
                break;
            case nameof(_studentManagementViewModel.TotalHp):
                HandleStudentHpChanged();
                break;
            case nameof(_studentManagementViewModel.TotalAttack):
                HandleStudentAttackChanged();
                break;
            case nameof(_studentManagementViewModel.TotalDefense):
                HandleStudentDefenseChanged();
                break;
            case nameof(_studentManagementViewModel.TotalMoveSpeed):
                HandleStudentMoveSpeedChanged();
                break;
            case nameof(_studentManagementViewModel.OwnedGradeUpItemCount):
                HandleOwnedGradeUpItemCountChanged();
                break;
        }
    }

    private void HandleStudentNameChanged()
    {
        _studentManagementInfoView.UpdateName(_studentManagementViewModel.Name);
    }

    private void HandleStudentStarChanged()
    {
        _studentManagementInfoView.UpdateStars(_studentManagementViewModel.Star);
        HandleOwnedGradeUpItemCountChanged();
    }

    private void HandleStudentFullBodyImageChanged()
    {
        _studentManagementExperienceView.UpdatePortraitImage(_studentManagementViewModel.FullBodyKey, _disableCts.Token).Forget();
    }

    private void HandleStudentTotalExperienceChanged()
    {
        int value = _studentManagementViewModel.TotalExperience;

        _studentManagementExperienceView.UpdateExperienceSliderValue(value);
        _studentManagementExperienceView.UpdateExperienceText();
    }

    private void HandleStudentLevelChanged()
    {
        int value = _studentManagementViewModel.IsMaxLevel ? _studentManagementViewModel.RequiredExp : _studentManagementViewModel.TotalExperience;

        _studentManagementExperienceView.UpdateLevelText(_studentManagementViewModel.Level);

        //TODO 이전 경험치 총량 가져오기
        //_studentManagementExperienceView.UpdateExperienceSliderRange(value, 이전 경험치 총량, _studentManagementViewModel.RequiredExp);
        _studentManagementExperienceView.UpdateExperienceText();
    }

    private void HandleStudentIsMaxLevelChanged()
    {
        _studentManagementInfoView.InterectiveGradeUp(_studentManagementViewModel.IsMaxLevel);
    }

    private void HandleStudentHpChanged()
    {
        _studentManagementStatusView.UpdateHpText(_studentManagementViewModel.TotalHp);
    }

    private void HandleStudentAttackChanged()
    {
        _studentManagementStatusView.UpdateAttackText(_studentManagementViewModel.TotalAttack);
    }

    private void HandleStudentDefenseChanged()
    {
        _studentManagementStatusView.UpdateDefenseText(_studentManagementViewModel.TotalDefense);
    }

    private void HandleStudentMoveSpeedChanged()
    {
        _studentManagementStatusView.UpdateMoveSpeedText(_studentManagementViewModel.TotalMoveSpeed);
    }

    private void HandleOwnedGradeUpItemCountChanged()
    {
        _studentManagementInfoView.UpdateRequiredGradeUpItemText(_studentManagementViewModel.OwnedGradeUpItemCount, _studentManagementViewModel.RequiredGradeUpItemCount);
    }

    private void HandleOpenExperienceInventoryClicked()
    {
        GameManager.Instance.UIManager.OpenExperienceInventoryPopupAsync(_studentManagementViewModel.StudentModel, _disableCts.Token).Forget();
    }

    //승급 기능 추가
    private void HandleGradeUpClicked()
    {

    }

    private void HandleEquipmentSlotClicked(EquipType equipType)
    {
        GameManager.Instance.UIManager.OpenEquipmentInventoryPopupAsync(equipType, _studentManagementViewModel.StudentModel, _disableCts.Token).Forget();
        GameManager.Instance.UIManager.OpenEquipmentCraftPopupAsync(equipType, _disableCts.Token).Forget();
    }
}

