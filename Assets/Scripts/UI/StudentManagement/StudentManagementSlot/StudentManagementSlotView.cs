using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentManagementSlotView : MonoBehaviour
{
    [SerializeField] private Button _slotButton;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private List<GameObject> _starIconList;

    private StudentManagementSlotViewModel _studentManagementSlotViewModel;

    private CancellationTokenSource _disableCts;

    public event Action<StudentModel> OnSlotClicked;

    private void Awake()
    {
        UnityUtil.ValidateReference(_slotButton, nameof(StudentManagementSlotView), nameof(_slotButton));
        UnityUtil.ValidateReference(_portraitImage, nameof(StudentManagementSlotView), nameof(_portraitImage));
        UnityUtil.ValidateReference(_nameText, nameof(StudentManagementSlotView), nameof(_nameText));

        _studentManagementSlotViewModel = new StudentManagementSlotViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _studentManagementSlotViewModel.PropertyChanged += OnPropertyChanged;
        _slotButton.onClick.AddListener(HandleSlotClicked);
    }

    private void OnDisable()
    {
        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        _studentManagementSlotViewModel.PropertyChanged -= OnPropertyChanged;
        _slotButton.onClick.RemoveAllListeners();

    }

    private void OnDestroy()
    {
        _studentManagementSlotViewModel.Dispose();
        _studentManagementSlotViewModel = null;
    }

    public void SetModel(StudentModel characterModel)
    {
        _studentManagementSlotViewModel.SetModel(characterModel);
        _studentManagementSlotViewModel.Refresh();
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_studentManagementSlotViewModel.Name):
                UpdateNameText();
                break;
            case nameof(_studentManagementSlotViewModel.Star):
                UpdateStarIcons();
                break;
            case nameof(_studentManagementSlotViewModel.PortraitKey):
                UpdatePortraitImageAsync().Forget();
                break;
        }
    }

    private void UpdateNameText()
    {
        _nameText.text = _studentManagementSlotViewModel.Name;
    }

    private void UpdateStarIcons()
    {
        int star = _studentManagementSlotViewModel.Star;

        for (int i = 0; i < _starIconList.Count; i++)
        {
            bool isActive = i < star;
            _starIconList[i].SetActive(isActive);
        }
    }

    private UniTask UpdatePortraitImageAsync()
    {
        return SpriteLoader.LoadIntoAsync(_portraitImage, _studentManagementSlotViewModel.PortraitKey, _disableCts.Token);
    }

    private void HandleSlotClicked()
    {
        if (OnSlotClicked == null)
        {
            return;
        }

        OnSlotClicked.Invoke(_studentManagementSlotViewModel.StudentModel);
    }
}