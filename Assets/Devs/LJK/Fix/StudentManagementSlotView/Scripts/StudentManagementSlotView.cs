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

    private CancellationTokenSource _portraitLoadCts;

    public event Action<StudentModel> OnSlotClicked;

    private void Awake()
    {
        _studentManagementSlotViewModel = new StudentManagementSlotViewModel();
    }

    private void OnEnable()
    {
        _portraitLoadCts = new CancellationTokenSource();

        _studentManagementSlotViewModel.PropertyChanged += OnPropertyChanged;
        _slotButton.onClick.AddListener(HandleClickSelect);
    }

    private void OnDisable()
    {
        if (_portraitLoadCts != null)
        {
            _portraitLoadCts.Cancel();
            _portraitLoadCts.Dispose();
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

    private async UniTask UpdatePortraitImageAsync()
    {
        string portraitKey = _studentManagementSlotViewModel.PortraitKey;

        if (string.IsNullOrWhiteSpace(portraitKey))
        {
            return;
        }

        Sprite portraitSprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(portraitKey, _portraitLoadCts.Token);

        if (portraitSprite == null)
        {
            return;
        }

        _portraitImage.sprite = portraitSprite;
    }

    private void HandleClickSelect()
    {
        if (OnSlotClicked == null)
        {
            return;
        }

        OnSlotClicked.Invoke(_studentManagementSlotViewModel.StudentModel);
    }
}