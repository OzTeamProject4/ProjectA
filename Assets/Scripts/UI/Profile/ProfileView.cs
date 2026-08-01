using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileView : BaseUI
{
    private const string DateFormat = "yyyy-MM-dd";
    private const string TotalLoginDaysFormat = "{0}일";
    private const string MainStoryFormat = "{0}-{1}";
    private const string OwnedStudentCountFormat = "{0}명";
    private const string TotalClearedStageCountFormat = "{0}개";
    private const string NotAvailableText = "-";

    [SerializeField] private TopbarView _topbarView;

    [Header("Profile")]
    [SerializeField] private TMP_Text _nicknameText;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _accountCreatedAtText;
    [SerializeField] private TMP_Text _lastConnectAtText;
    [SerializeField] private TMP_Text _totalLoginDaysText;
    [SerializeField] private TMP_InputField _introductionInputField;
    [SerializeField] private Image _representativeStudentImage;
    [SerializeField] private Button _representativeStudentButton;

    [Header("Progress")]
    [SerializeField] private TMP_Text _mainStoryText;
    [SerializeField] private TMP_Text _ownedStudentCountText;
    [SerializeField] private TMP_Text _totalClearedStageCountText;
    [SerializeField] private TMP_Text _achievementCountText;

    [Header("Currency")]
    [SerializeField] private ProfileCurrencySlotView[] _currencySlotViews;

    private ProfileViewModel _profileViewModel;
    private ProfileStudentSelectPopupView _profileStudentSelectPopupView;

    private CancellationTokenSource _disableCts;

    private void Awake()
    {
        UnityUtil.ValidateReference(_topbarView, nameof(ProfileView), nameof(_topbarView));
        UnityUtil.ValidateReference(_nicknameText, nameof(ProfileView), nameof(_nicknameText));
        UnityUtil.ValidateReference(_levelText, nameof(ProfileView), nameof(_levelText));
        UnityUtil.ValidateReference(_accountCreatedAtText, nameof(ProfileView), nameof(_accountCreatedAtText));
        UnityUtil.ValidateReference(_lastConnectAtText, nameof(ProfileView), nameof(_lastConnectAtText));
        UnityUtil.ValidateReference(_totalLoginDaysText, nameof(ProfileView), nameof(_totalLoginDaysText));
        UnityUtil.ValidateReference(_introductionInputField, nameof(ProfileView), nameof(_introductionInputField));
        UnityUtil.ValidateReference(_representativeStudentImage, nameof(ProfileView), nameof(_representativeStudentImage));
        UnityUtil.ValidateReference(_representativeStudentButton, nameof(ProfileView), nameof(_representativeStudentButton));
        UnityUtil.ValidateReference(_mainStoryText, nameof(ProfileView), nameof(_mainStoryText));
        UnityUtil.ValidateReference(_ownedStudentCountText, nameof(ProfileView), nameof(_ownedStudentCountText));
        UnityUtil.ValidateReference(_totalClearedStageCountText, nameof(ProfileView), nameof(_totalClearedStageCountText));
        UnityUtil.ValidateReference(_achievementCountText, nameof(ProfileView), nameof(_achievementCountText));

        ValidateCurrencySlotViews();

        _profileViewModel = new ProfileViewModel();
    }

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        _topbarView.OnBackClicked += HandleBackClicked;
        _introductionInputField.onEndEdit.AddListener(HandleIntroductionEndEdit);
        _representativeStudentButton.onClick.AddListener(HandleRepresentativeStudentClicked);

        _profileViewModel.PropertyChanged += OnPropertyChanged;
        _profileViewModel.Refresh();

        UpdateAchievementCountText();
    }

    private void OnDisable()
    {
        _topbarView.OnBackClicked -= HandleBackClicked;
        _introductionInputField.onEndEdit.RemoveListener(HandleIntroductionEndEdit);
        _representativeStudentButton.onClick.RemoveListener(HandleRepresentativeStudentClicked);

        UnsubscribeStudentSelectPopup();

        _profileViewModel.PropertyChanged -= OnPropertyChanged;

        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }
    }

    private void OnDestroy()
    {
        _profileViewModel.Dispose();
        _profileViewModel = null;
    }

    private void ValidateCurrencySlotViews()
    {
        if (_currencySlotViews == null || _currencySlotViews.Length == 0)
        {
            Debug.LogError($"[{nameof(ProfileView)}:{nameof(ValidateCurrencySlotViews)}] '{nameof(_currencySlotViews)}' 배열이 비어 있습니다.");
            return;
        }

        for (int i = 0; i < _currencySlotViews.Length; i++)
        {
            UnityUtil.ValidateReference(_currencySlotViews[i], nameof(ProfileView), $"{nameof(_currencySlotViews)}[{i}]");
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_profileViewModel.Nickname):
                UpdateNicknameText();
                break;
            case nameof(_profileViewModel.Level):
                UpdateLevelText();
                break;
            case nameof(_profileViewModel.AccountCreatedAt):
                UpdateAccountCreatedAtText();
                break;
            case nameof(_profileViewModel.LastConnectAt):
                UpdateLastConnectAtText();
                break;
            case nameof(_profileViewModel.TotalLoginDays):
                UpdateTotalLoginDaysText();
                break;
            case nameof(_profileViewModel.Introduction):
                UpdateIntroductionInputField();
                break;
            case nameof(_profileViewModel.MainStoryChapter):
            case nameof(_profileViewModel.MainStoryStage):
                UpdateMainStoryText();
                break;
            case nameof(_profileViewModel.CurrencyList):
                UpdateCurrencySlots();
                break;
            case nameof(_profileViewModel.OwnedStudentCount):
                UpdateOwnedStudentCountText();
                break;
            case nameof(_profileViewModel.ClearedStageCount):
                UpdateTotalClearedStageCountText();
                break;
            case nameof(_profileViewModel.RepresentativeStudentFullBodyKey):
                UpdateRepresentativeStudentImageAsync().Forget();
                break;
        }
    }

    private UniTask UpdateRepresentativeStudentImageAsync()
    {
        return SpriteLoader.LoadIntoAsync(_representativeStudentImage, _profileViewModel.RepresentativeStudentFullBodyKey, _disableCts.Token);
    }

    private void UpdateCurrencySlots()
    {
        IReadOnlyList<MaterialModel> currencyList = _profileViewModel.CurrencyList;

        for (int i = 0; i < _currencySlotViews.Length; i++)
        {
            ProfileCurrencySlotView currencySlotView = _currencySlotViews[i];

            if (currencySlotView == null)
            {
                continue;
            }

            if (i >= currencyList.Count)
            {
                currencySlotView.gameObject.SetActive(false);
                continue;
            }

            currencySlotView.gameObject.SetActive(true);
            currencySlotView.SetModel(currencyList[i]);
        }
    }

    private void UpdateNicknameText()
    {
        _nicknameText.text = _profileViewModel.Nickname;
    }

    private void UpdateLevelText()
    {
        _levelText.text = _profileViewModel.Level.ToString();
    }

    private void UpdateAccountCreatedAtText()
    {
        _accountCreatedAtText.text = _profileViewModel.AccountCreatedAt.ToString(DateFormat);
    }

    private void UpdateLastConnectAtText()
    {
        _lastConnectAtText.text = _profileViewModel.LastConnectAt.ToString(DateFormat);
    }

    private void UpdateTotalLoginDaysText()
    {
        _totalLoginDaysText.text = string.Format(TotalLoginDaysFormat, _profileViewModel.TotalLoginDays);
    }

    private void UpdateIntroductionInputField()
    {
        _introductionInputField.text = _profileViewModel.Introduction;
    }

    private void UpdateMainStoryText()
    {
        _mainStoryText.text = string.Format(MainStoryFormat, _profileViewModel.MainStoryChapter, _profileViewModel.MainStoryStage);
    }

    private void UpdateOwnedStudentCountText()
    {
        _ownedStudentCountText.text = string.Format(OwnedStudentCountFormat, _profileViewModel.OwnedStudentCount);
    }

    private void UpdateTotalClearedStageCountText()
    {
        _totalClearedStageCountText.text = string.Format(TotalClearedStageCountFormat, _profileViewModel.ClearedStageCount);
    }

    private void UpdateAchievementCountText()
    {
        _achievementCountText.text = NotAvailableText;
    }

    private async UniTaskVoid OpenStudentSelectPopupAsync()
    {
        ProfileStudentSelectPopupView popupView = await GameManager.Instance.UIManager.OpenProfileStudentSelectPopupAsync(_disableCts.Token);

        if (popupView == null)
        {
            Debug.LogError($"[{nameof(ProfileView)}:{nameof(OpenStudentSelectPopupAsync)}] 대표 학생 선택 팝업을 열지 못했습니다.");
            return;
        }

        UnsubscribeStudentSelectPopup();

        _profileStudentSelectPopupView = popupView;
        _profileStudentSelectPopupView.OnStudentSelected += HandleStudentSelected;
    }

    private void UnsubscribeStudentSelectPopup()
    {
        if (_profileStudentSelectPopupView == null)
        {
            return;
        }

        _profileStudentSelectPopupView.OnStudentSelected -= HandleStudentSelected;
        _profileStudentSelectPopupView = null;
    }

    private void HandleRepresentativeStudentClicked()
    {
        OpenStudentSelectPopupAsync().Forget();
    }

    private void HandleStudentSelected(StudentModel studentModel)
    {
        if (studentModel == null)
        {
            return;
        }

        _profileViewModel.SetRepresentativeStudentId(studentModel.DataId);

        UnsubscribeStudentSelectPopup();
        GameManager.Instance.UIManager.CloseProfileStudentSelectPopup();
    }

    private void HandleIntroductionEndEdit(string introduction)
    {
        _profileViewModel.SetIntroduction(introduction);
    }

    private void HandleBackClicked()
    {
        GameManager.Instance.UIManager.CloseProfile();
    }
}
