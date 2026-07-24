using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class StudentManagementListView : BaseUI
{
    [SerializeField] private StudentManagementSlotView _slotPrefab;
    [SerializeField] private Transform _content;

    private readonly List<StudentManagementSlotView> _spawnedSlotList = new List<StudentManagementSlotView>();

    private StudentManagementListViewModel _studentManagementListViewModel;

    //오브젝트 풀에 슬롯 미리 생성 (고려)
    private void Awake()
    {
        UnityUtil.ValidateReference(_slotPrefab, nameof(StudentManagementListView), nameof(_slotPrefab));
        UnityUtil.ValidateReference(_content, nameof(StudentManagementListView), nameof(_content));

        _studentManagementListViewModel = new StudentManagementListViewModel();
    }

    private void OnEnable()
    {
        RefreshSlots();
    }

    private void OnDestroy()
    {
        _studentManagementListViewModel.Dispose();
        _studentManagementListViewModel = null;
    }

    //TODO 슬롯 오브젝트 풀 사용 생성
    private void RefreshSlots()
    {
        ReleaseSlots();

        foreach (StudentModel studentModel in _studentManagementListViewModel.StudentList)
        {
            StudentManagementSlotView studentManagementSlotView = Instantiate(_slotPrefab, _content);
            studentManagementSlotView.SetModel(studentModel);
            studentManagementSlotView.OnSlotClicked += HandleSlotClicked;
            _spawnedSlotList.Add(studentManagementSlotView);
        }
    }

    //TODO 슬롯 오브젝트 풀 사용 해제
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
        GameManager.Instance.UIManager.OpenStudentManagementAsync(studentModel, destroyCancellationToken).Forget();
    }
}