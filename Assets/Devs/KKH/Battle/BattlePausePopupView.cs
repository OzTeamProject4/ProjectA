using Cysharp.Threading.Tasks;
using UnityEngine;
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

        if (null != _resumeButton)
        {
            _resumeButton.onClick.AddListener(HandleClickResume);
        }

        if (null != _settingsButton)
        {
            _settingsButton.onClick.AddListener(HandleClickSettings);
        }

        if (null != _backToStageButton)
        {
            _backToStageButton.onClick.AddListener(HandleClickBackToStage);
        }

        if (null != _blockerButton)
        {
            _blockerButton.onClick.AddListener(HandleClickResume);
        }

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (_isSubscribed)
        {
            if (null != _resumeButton)
            {
                _resumeButton.onClick.RemoveListener(HandleClickResume);
            }

            if (null != _settingsButton)
            {
                _settingsButton.onClick.RemoveListener(HandleClickSettings);
            }

            if (null != _backToStageButton)
            {
                _backToStageButton.onClick.RemoveListener(HandleClickBackToStage);
            }

            if (null != _blockerButton)
            {
                _blockerButton.onClick.RemoveListener(HandleClickResume);
            }

            _isSubscribed = false;
        }

        Complete(BattlePauseChoice.Resume);
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
        // TODO: 세팅 팝업 연동 (아직 없음). 일시정지 유지, 완료하지 않음
        Debug.Log("[BattlePausePopupView] 세팅 팝업");
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
