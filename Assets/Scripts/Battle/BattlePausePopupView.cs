using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum BattlePauseChoice
{
    None,
    Resume,
    Settings,
    BackToStage
}

public class BattlePausePopupView : BaseUI
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _backToStageButton;
    [SerializeField] private Button _blockerButton;

    private UniTaskCompletionSource<BattlePauseChoice> _completionSource;
    private bool _isSubscribed;

    private void OnDisable()
    {
        Unsubscribe();
    }

    public UniTask<BattlePauseChoice> WaitForChoiceAsync()
    {
        _completionSource = new UniTaskCompletionSource<BattlePauseChoice>();

        Subscribe();

        return _completionSource.Task;
    }

    private void Subscribe()
    {
        if (_isSubscribed)
        {
            return;
        }

        AddClickListener(_resumeButton, HandleClickResume, nameof(_resumeButton));
        AddClickListener(_settingsButton, HandleClickSettings, nameof(_settingsButton));
        AddClickListener(_backToStageButton, HandleClickBackToStage, nameof(_backToStageButton));
        AddClickListener(_blockerButton, HandleClickResume, nameof(_blockerButton));

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (_isSubscribed)
        {
            RemoveClickListener(_resumeButton, HandleClickResume);
            RemoveClickListener(_settingsButton, HandleClickSettings);
            RemoveClickListener(_backToStageButton, HandleClickBackToStage);
            RemoveClickListener(_blockerButton, HandleClickResume);

            _isSubscribed = false;
        }

        Complete(BattlePauseChoice.Resume);
    }

    private void AddClickListener(Button button, UnityAction callback, string fieldName)
    {
        if (null == button)
        {
            Debug.LogError($"[{nameof(BattlePausePopupView)}] {fieldName} 이(가) 인스펙터에 연결되지 않았습니다. 프리팹의 Button 참조를 확인하세요.");
            return;
        }

        button.onClick.AddListener(callback);
    }

    private void RemoveClickListener(Button button, UnityAction callback)
    {
        if (null == button)
        {
            return;
        }

        button.onClick.RemoveListener(callback);
    }

    private void HandleClickResume()
    {
        Complete(BattlePauseChoice.Resume);
    }

    private void HandleClickBackToStage()
    {
        Complete(BattlePauseChoice.BackToStage);
    }

    private void HandleClickSettings()
    {
        // TODO: 세팅 팝업 연동. 프로젝트에 SettingsPopup 프리팹과 UIType 이 추가되면 여기서 열 것
        //       일시정지 상태는 유지하고 선택을 완료하지 않는다
        Debug.LogWarning("[BattlePausePopupView] 세팅 팝업이 아직 구현되지 않았습니다.");
    }

    private void Complete(BattlePauseChoice choice)
    {
        if (null == _completionSource)
        {
            return;
        }

        UniTaskCompletionSource<BattlePauseChoice> source = _completionSource;
        _completionSource = null;
        source.TrySetResult(choice);
    }
}
