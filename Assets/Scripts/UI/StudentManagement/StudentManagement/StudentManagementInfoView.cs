using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentManagementInfoView : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _elementIcon;
    [SerializeField] private Image _requiredGradeUpItemIconImage;
    [SerializeField] private TMP_Text _requiredGradeUpItemCountText;
    [SerializeField] private Button _gradeUpButton;
    [SerializeField] private List<GameObject> _starIconList;
    [SerializeField] private Sprite[] _elementIcons; // TODO: 속성 아이콘 따로 빼서 어드레서블 등록하는 방식으로 교체

    [Header("Experience")]
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Slider _experienceSlider;
    [SerializeField] private TMP_Text _experienceText;
    [SerializeField] private Button _openExperienceInventoryButton;

    public event Action OnGradeUpClicked;
    public event Action OnOpenExperienceInventoryClicked;

    private void Awake()
    {
        UnityUtil.ValidateReference(_nameText, nameof(StudentManagementInfoView), nameof(_nameText));
        UnityUtil.ValidateReference(_elementIcon, nameof(StudentManagementInfoView), nameof(_elementIcon));
        UnityUtil.ValidateReference(_requiredGradeUpItemIconImage, nameof(StudentManagementInfoView), nameof(_requiredGradeUpItemIconImage));
        UnityUtil.ValidateReference(_requiredGradeUpItemCountText, nameof(StudentManagementInfoView), nameof(_requiredGradeUpItemCountText));
        UnityUtil.ValidateReference(_gradeUpButton, nameof(StudentManagementInfoView), nameof(_gradeUpButton));
        UnityUtil.ValidateReference(_portraitImage, nameof(StudentManagementInfoView), nameof(_portraitImage));
        UnityUtil.ValidateReference(_levelText, nameof(StudentManagementInfoView), nameof(_levelText));
        UnityUtil.ValidateReference(_experienceSlider, nameof(StudentManagementInfoView), nameof(_experienceSlider));
        UnityUtil.ValidateReference(_experienceText, nameof(StudentManagementInfoView), nameof(_experienceText));
        UnityUtil.ValidateReference(_openExperienceInventoryButton, nameof(StudentManagementInfoView), nameof(_openExperienceInventoryButton));
    }

    private void OnEnable()
    {
        _gradeUpButton.onClick.AddListener(HandleGradeUpButtonClicked);
        _openExperienceInventoryButton.onClick.AddListener(HandleOpenExperienceInventoryButtonClicked);
    }

    private void OnDisable()
    {
        _gradeUpButton.onClick.RemoveAllListeners();
        _openExperienceInventoryButton.onClick.RemoveAllListeners();
    }

    public void UpdateName(string studentName)
    {
        _nameText.text = studentName;
    }

    public void UpdateRequiredGradeUpItemText(int ownedItemCount, int requiredItemCount)
    {
        _requiredGradeUpItemCountText.text = $"{ownedItemCount} / {requiredItemCount}";
    }

    // TODO: 속성별 문양 갱신. 어드레서블 교체 목록
    public void UpdateElementIcon(ElementType elementType)
    {
        int index = (int)elementType;

        if (_elementIcons == null || index < 0 || index >= _elementIcons.Length || _elementIcons[index] == null)
        {
            Debug.LogWarning($"{elementType} 속성 아이콘이 지정되지 않았습니다.", this);
            _elementIcon.enabled = false;
            return;
        }

        _elementIcon.enabled = true;
        _elementIcon.sprite = _elementIcons[index];
    }

    public void UpdateStars(int starCount)
    {
        for (int i = 0; i < _starIconList.Count; i++)
        {
            bool isActive = i < starCount;
            _starIconList[i].SetActive(isActive);
        }
    }

    public void InterectiveGradeUp(bool interective)
    {
        _gradeUpButton.interactable = interective;
    }

    public UniTask UpdatePortraitImage(string fullBodyKey, CancellationToken cancellationToken)
    {
        return SpriteLoader.LoadIntoAsync(_portraitImage, fullBodyKey, cancellationToken);
    }

    public UniTask UpdateRequiredGradeUpItemIcon(string iconKey, CancellationToken cancellationToken)
    {
        return SpriteLoader.LoadIntoAsync(_requiredGradeUpItemIconImage, iconKey, cancellationToken);
    }

    public void UpdateLevelText(int level)
    {
        _levelText.text = $"Lv.{level}";
    }

    public void UpdateExperienceSliderValue(int currentExperience)
    {
        _experienceSlider.value = currentExperience;
    }

    public void UpdateExperienceSliderRange(int currentExperience, int requiredExperience)
    {
        _experienceSlider.minValue = 0;
        _experienceSlider.maxValue = requiredExperience;
        _experienceSlider.value = currentExperience;
    }

    public void UpdateExperienceText()
    {
        _experienceText.text = $"{_experienceSlider.value} / {_experienceSlider.maxValue}";
    }

    private void HandleGradeUpButtonClicked()
    {
        if (OnGradeUpClicked == null)
        {
            return;
        }

        OnGradeUpClicked.Invoke();
    }

    private void HandleOpenExperienceInventoryButtonClicked()
    {
        if (OnOpenExperienceInventoryClicked == null)
        {
            return;
        }

        OnOpenExperienceInventoryClicked.Invoke();
    }
}
