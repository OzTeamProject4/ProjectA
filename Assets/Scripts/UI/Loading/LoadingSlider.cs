using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSlider : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _text;

    [Header("Settings")]
    [SerializeField] private float _sliderSpeed = 2f;

    private readonly Dictionary<LoadingStep, string> _loadingStepMessages = new Dictionary<LoadingStep, string>();
    
    private float _targetProgress;

    public bool IsCompleted
    {
        get
        {
            return _targetProgress >= 1f && _slider.value >= 1f;
        }
    }

    private void Awake()
    {
        UnityUtil.ValidateReference(_slider, nameof(LoadingSlider), nameof(_slider));
        UnityUtil.ValidateReference(_text, nameof(LoadingSlider), nameof(_text));
        
        InitializeLoadingStepMessages();
    }

    private void Update()
    {
        UpdateSliderValue();
    }

    public void UpdateProgress(LoadingProgress loadingProgress)
    {
        _targetProgress = loadingProgress.Progress;

        UpdateLoadingText(loadingProgress.LoadingStep);
    }

    //TODO 로딩 다듬기
    private void InitializeLoadingStepMessages()
    {
        _loadingStepMessages.Clear();

        _loadingStepMessages.Add(LoadingStep.Initialize, "초기화 중...");

        _loadingStepMessages.Add(LoadingStep.LoadStudentData, "학생 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadStudentGradeData, "학생 등급 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadStudentLevelData, "학생 레벨 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadItemData, "아이템 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadCurrencyData, "재화 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadEquipmentData, "장비 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadSignatureData, "시그니처 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadSkillData, "스킬 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadStageData, "스테이지 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadStageWaveData, "스테이지 웨이브 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadEnemyData, "적 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadEnemySkillData, "적 스킬 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadAudioData, "오디오 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadMissionData, "미션 데이터 불러오는 중...");
        _loadingStepMessages.Add(LoadingStep.LoadStoryInfoData, "Preparing storyInfo...");

        _loadingStepMessages.Add(LoadingStep.Complete, "로딩 완료.");
    }

    private void UpdateSliderValue()
    {
        if (_slider.value == _targetProgress)
        {
            return;
        }

        _slider.value = Mathf.MoveTowards(_slider.value, _targetProgress, _sliderSpeed * Time.deltaTime);
    }

    private void UpdateLoadingText(LoadingStep loadingStep)
    {
        _text.text = GetLoadingStepMessage(loadingStep);
    }

    private string GetLoadingStepMessage(LoadingStep loadingStep)
    {
        if (!_loadingStepMessages.TryGetValue(loadingStep, out string message))
        {
            return string.Empty;
        }

        return message;
    }
}