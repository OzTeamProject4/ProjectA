using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentManagementExperienceView : MonoBehaviour
{
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Slider _experienceSlider;
    [SerializeField] private TMP_Text _experienceText;
    [SerializeField] private Button _openExperienceInventoryButton;

    public event Action OnOpenExperienceInventoryClicked;

    private void Awake()
    {
        UnityUtil.ValidateReference(_portraitImage, nameof(StudentManagementExperienceView), nameof(_portraitImage));
        UnityUtil.ValidateReference(_levelText, nameof(StudentManagementExperienceView), nameof(_levelText));
        UnityUtil.ValidateReference(_experienceSlider, nameof(StudentManagementExperienceView), nameof(_experienceSlider));
        UnityUtil.ValidateReference(_experienceText, nameof(StudentManagementExperienceView), nameof(_experienceText));
        UnityUtil.ValidateReference(_openExperienceInventoryButton, nameof(StudentManagementExperienceView), nameof(_openExperienceInventoryButton));
    }

    public void OnEnable()
    {
        _openExperienceInventoryButton.onClick.AddListener(HandleOpenExperienceInventoryButtonClicked);
    }

    public void OnDisable()
    {
        _openExperienceInventoryButton.onClick.RemoveAllListeners();
    }

    public async UniTask UpdatePortraitImage(string fullBodyKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fullBodyKey))
        {
            return;
        }

        Sprite portraitSprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(fullBodyKey, cancellationToken);

        if (portraitSprite == null)
        {
            return;
        }

        _portraitImage.sprite = portraitSprite;
    }

    public void UpdateLevelText(int level)
    {
        _levelText.text = $"Lv.{level}";
    }

    public void UpdateExperienceSliderValue(int currentExperience)
    {
        _experienceSlider.value = currentExperience;
    }

    public void UpdateExperienceSliderRange(int currentExperience, int beforeLevelRequiredExp, int nextLevelRequiredExp)
    {
        _experienceSlider.minValue = beforeLevelRequiredExp;
        _experienceSlider.maxValue = nextLevelRequiredExp;
        _experienceSlider.value = currentExperience;
    }

    public void UpdateExperienceText()
    {
        _experienceText.text = $"{_experienceSlider.value} / {_experienceSlider.maxValue}";
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
