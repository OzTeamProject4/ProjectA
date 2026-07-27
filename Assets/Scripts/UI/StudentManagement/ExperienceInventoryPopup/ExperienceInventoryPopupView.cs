using System.Collections.Generic;
using UnityEngine;

public class ExperienceInventoryPopupView : BaseUI
{
    [SerializeField] private ExperienceItemSlotView _slotPrefab;
    [SerializeField] private Transform _content;

    private ExperienceInventoryPopupViewModel _experienceInventoryPopupViewModel;

    private readonly List<ExperienceItemSlotView> _spawnedSlotList = new List<ExperienceItemSlotView>();

    //오브젝트 풀에 슬롯 미리 생성 (고려)
    private void Awake()
    {
        UnityUtil.ValidateReference(_slotPrefab, nameof(ExperienceInventoryPopupView), nameof(_slotPrefab));
        UnityUtil.ValidateReference(_content, nameof(ExperienceInventoryPopupView), nameof(_content));
        _experienceInventoryPopupViewModel = new ExperienceInventoryPopupViewModel();
    }

    private void OnDestroy()
    {
        _experienceInventoryPopupViewModel.Dispose();
        _experienceInventoryPopupViewModel = null;
    }

    public void SetModel(StudentModel studentModel)
    {
        _experienceInventoryPopupViewModel.SetModel(studentModel);
        RefreshSlots();
    }

    //TODO 슬롯 오브젝트 풀 사용 생성
    private void RefreshSlots()
    {
        ReleaseSlots();

        foreach (MaterialModel materialModel in _experienceInventoryPopupViewModel.ExperienceItems.Values)
        {
            ExperienceItemSlotView experienceItemSlotView = Instantiate(_slotPrefab, _content);
            experienceItemSlotView.SetModel(materialModel);
            experienceItemSlotView.OnSlotClicked += HandleSlotClicked;
            _spawnedSlotList.Add(experienceItemSlotView);
        }
    }

    //TODO 슬롯 오브젝트 풀 사용 해제
    private void ReleaseSlots()
    {
        foreach (ExperienceItemSlotView experienceItemSlotView in _spawnedSlotList)
        {
            if (null == experienceItemSlotView)
            {
                continue;
            }

            experienceItemSlotView.OnSlotClicked -= HandleSlotClicked;
            Destroy(experienceItemSlotView.gameObject);
        }

        _spawnedSlotList.Clear();
    }

    private void HandleSlotClicked(MaterialModel materialModel)
    {
        _experienceInventoryPopupViewModel.UseExpItem(materialModel);
    }
}