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

    private void InitializeLoadingStepMessages()
    {
        _loadingStepMessages.Clear();

        _loadingStepMessages.Add(LoadingStep.Initialize, "Initialized...");
        _loadingStepMessages.Add(LoadingStep.Load_1, "First job completed.");
        _loadingStepMessages.Add(LoadingStep.Load_2, "Second job completed.");
        _loadingStepMessages.Add(LoadingStep.Complete, "All jobs completed.");
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