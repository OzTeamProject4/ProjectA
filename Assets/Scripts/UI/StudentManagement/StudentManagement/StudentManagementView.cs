using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class StudentManagementView : BaseUI
{
    [SerializeField] private TopbarView _topbarView;
    [SerializeField] private StudentManagementInfoView _studentManagementInfoView;
    [SerializeField] private StudentManagementEquipmentView _studentManagementEquipmentView;
    [SerializeField] private StudentManagementStatusView _studentManagementStatusView;

    private StudentManagementViewModel _studentManagementViewModel;

    private void Awake()
    {
        UnityUtil.ValidateReference(_topbarView, nameof(StudentManagementView), nameof(_topbarView));
        UnityUtil.ValidateReference(_studentManagementInfoView, nameof(StudentManagementView), nameof(_studentManagementInfoView));
        UnityUtil.ValidateReference(_studentManagementEquipmentView, nameof(StudentManagementView), nameof(_studentManagementEquipmentView));
        UnityUtil.ValidateReference(_studentManagementStatusView, nameof(StudentManagementView), nameof(_studentManagementStatusView));

        _studentManagementViewModel = new StudentManagementViewModel();
    }

    private CancellationTokenSource _disableCts;

    private void OnEnable()
    {
        _topbarView.OnBackClicked += HandleBackClicked;
        _studentManagementInfoView.OnOpenExperienceInventoryClicked += HandleOpenExperienceInventoryClicked;
        _studentManagementInfoView.OnGradeUpClicked += HandleGradeUpClicked;
        _studentManagementEquipmentView.OnSlotClicked += HandleEquipmentSlotClicked;

        _disableCts = new CancellationTokenSource();
        _studentManagementViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnDisable()
    {
        _topbarView.OnBackClicked -= HandleBackClicked;
        _studentManagementInfoView.OnOpenExperienceInventoryClicked -= HandleOpenExperienceInventoryClicked;
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

        _studentManagementStatusView.SetSkills(_studentManagementViewModel.GetSkills(), _disableCts.Token);
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
            case nameof(_studentManagementViewModel.ElementType):
                HandleStudentElementTypeChanged();
                break;
            case nameof(_studentManagementViewModel.FullBodyKey):
                HandleStudentFullBodyImageChanged();
                break;
            case nameof(_studentManagementViewModel.CurrentExperience):
                HandleStudentCurrentExperienceChanged();
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
            case nameof(_studentManagementViewModel.EquippedItemIds):
                HandleEquippedItemsChanged();
                break;
            case nameof(StudentModel.RequiredGradeUpItemId):
                HandleRequiredGradeUpItemIdChanged();
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

    private void HandleStudentElementTypeChanged()
    {
        _studentManagementInfoView.UpdateElementIcon(_studentManagementViewModel.ElementType);
    }

    private void HandleStudentFullBodyImageChanged()
    {
        _studentManagementInfoView.UpdatePortraitImage(_studentManagementViewModel.FullBodyKey, _disableCts.Token).Forget();
    }

    private void HandleStudentCurrentExperienceChanged()
    {
        RefreshExperienceDisplay();
    }

    private void HandleStudentLevelChanged()
    {
        _studentManagementInfoView.UpdateLevelText(_studentManagementViewModel.Level);

        RefreshExperienceDisplay();
    }

    private void HandleStudentIsMaxLevelChanged()
    {
        bool isMaxLevel = _studentManagementViewModel.IsMaxLevel;

        _studentManagementInfoView.InterectiveGradeUp(isMaxLevel);

        _studentManagementInfoView.SetExperienceInventoryInteractable(!isMaxLevel);

        RefreshExperienceDisplay();
    }

    private void RefreshExperienceDisplay()
    {
        if (_studentManagementViewModel.IsMaxLevel)
        {
            _studentManagementInfoView.UpdateExperienceAsMax();
            return;
        }

        _studentManagementInfoView.UpdateExperienceSliderRange(_studentManagementViewModel.CurrentExperience, _studentManagementViewModel.RequiredExp);
        _studentManagementInfoView.UpdateExperienceText();
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

    private void HandleEquippedItemsChanged()
    {
        _studentManagementEquipmentView.ClearSlots();

        foreach (string instanceId in _studentManagementViewModel.EquippedItemIds.Values)
        {
            if (!_studentManagementViewModel.TryGetEquipment(instanceId, out EquipmentModel equipmentModel))
            {
                continue;
            }

            _studentManagementEquipmentView.UpdateEquipmentSlot(equipmentModel);
        }
    }

    private void HandleRequiredGradeUpItemIdChanged()
    {
        _studentManagementInfoView.UpdateRequiredGradeUpItemIcon(_studentManagementViewModel.RequiredGradeUpItemIconKey, _disableCts.Token).Forget();
    }

    private void HandleOwnedGradeUpItemCountChanged()
    {
        _studentManagementInfoView.UpdateRequiredGradeUpItemText(_studentManagementViewModel.OwnedGradeUpItemCount, _studentManagementViewModel.RequiredGradeUpItemCount);
    }

    //TODO UIManager에 네비게이션 스택이 붙으면 삭제하기
    private void HandleBackClicked()
    {
        // 이 화면 위에 열린 장비/경험치 팝업이 남아 있으면 같이 닫기
        GameManager.Instance.UIManager.CloseStudentManagementPopups();
        GameManager.Instance.UIManager.CloseStudentManagement();
    }

    private void HandleOpenExperienceInventoryClicked()
    {
        GameManager.Instance.UIManager.OpenExperienceInventoryPopupAsync(_studentManagementViewModel.StudentModel, _disableCts.Token).Forget();
    }

    private void HandleGradeUpClicked()
    {
        _studentManagementViewModel.RequestGradeUp();
    }

    private void HandleEquipmentSlotClicked(EquipType equipType)
    {
        GameManager.Instance.UIManager.OpenEquipmentInventoryPopupAsync(equipType, _studentManagementViewModel.StudentModel, _disableCts.Token).Forget();
        GameManager.Instance.UIManager.OpenEquipmentCraftPopupAsync(equipType, _disableCts.Token).Forget();
    }
}

