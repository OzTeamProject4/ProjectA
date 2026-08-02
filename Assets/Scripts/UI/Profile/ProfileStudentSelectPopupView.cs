using System;
using System.Collections.Generic;
using UnityEngine;

public class ProfileStudentSelectPopupView : BaseUI
{
    [SerializeField] private StudentManagementSlotView _slotPrefab;
    [SerializeField] private Transform _content;

    private readonly List<StudentManagementSlotView> _spawnedSlotList = new List<StudentManagementSlotView>();

    private ProfileStudentSelectPopupViewModel _profileStudentSelectPopupViewModel;

    public event Action<StudentModel> OnStudentSelected;

    private void Awake()
    {
        UnityUtil.ValidateReference(_slotPrefab, nameof(ProfileStudentSelectPopupView), nameof(_slotPrefab));
        UnityUtil.ValidateReference(_content, nameof(ProfileStudentSelectPopupView), nameof(_content));

        _profileStudentSelectPopupViewModel = new ProfileStudentSelectPopupViewModel();
    }

    private void OnEnable()
    {
        RefreshSlots();
    }

    private void OnDisable()
    {
        ReleaseSlots();
    }

    private void OnDestroy()
    {
        _profileStudentSelectPopupViewModel.Dispose();
        _profileStudentSelectPopupViewModel = null;
    }

    private void RefreshSlots()
    {
        ReleaseSlots();

        foreach (StudentModel studentModel in _profileStudentSelectPopupViewModel.StudentList)
        {
            StudentManagementSlotView studentManagementSlotView = Instantiate(_slotPrefab, _content);
            studentManagementSlotView.SetModel(studentModel);
            studentManagementSlotView.OnSlotClicked += HandleSlotClicked;
            _spawnedSlotList.Add(studentManagementSlotView);
        }
    }

    private void ReleaseSlots()
    {
        foreach (StudentManagementSlotView studentManagementSlotView in _spawnedSlotList)
        {
            if (null == studentManagementSlotView)
            {
                continue;
            }

            studentManagementSlotView.OnSlotClicked -= HandleSlotClicked;
            Destroy(studentManagementSlotView.gameObject);
        }

        _spawnedSlotList.Clear();
    }

    private void HandleSlotClicked(StudentModel studentModel)
    {
        if (OnStudentSelected == null)
        {
            return;
        }

        OnStudentSelected.Invoke(studentModel);
    }
}
