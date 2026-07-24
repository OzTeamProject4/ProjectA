using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ReturnToLobbyChoice
{
    None,
    Confirm,
    Cancel
}

public class ReturnToLobbyPopupView : BaseUI
{
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _blockerButton;

    private UniTaskCompletionSource<ReturnToLobbyChoice> _completionSource;
    private bool _isSubscribed;

    private void OnDisable()
    {
        Unsubscribe();
    }

    public UniTask<ReturnToLobbyChoice> WaitForChoiceAsync()
    {
        _completionSource = new UniTaskCompletionSource<ReturnToLobbyChoice>();

        Subscribe();

        return _completionSource.Task;
    }

    private void Subscribe()
    {
        if (_isSubscribed)
        {
            return;
        }

        AddClickListener(_confirmButton, HandleClickConfirm, nameof(_confirmButton));
        AddClickListener(_cancelButton, HandleClickCancel, nameof(_cancelButton));
        AddClickListener(_blockerButton, HandleClickCancel, nameof(_blockerButton));

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (_isSubscribed)
        {
            RemoveClickListener(_confirmButton, HandleClickConfirm);
            RemoveClickListener(_cancelButton, HandleClickCancel);
            RemoveClickListener(_blockerButton, HandleClickCancel);

            _isSubscribed = false;
        }

        Complete(ReturnToLobbyChoice.Cancel);
    }

    private void AddClickListener(Button button, UnityAction callback, string fieldName)
    {
        if (null == button)
        {
            Debug.LogError($"[{nameof(ReturnToLobbyPopupView)}] {fieldName} 이(가) 인스펙터에 연결되지 않았습니다. 프리팹의 Button 참조를 확인하세요.");
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

    private void HandleClickConfirm()
    {
        Complete(ReturnToLobbyChoice.Confirm);
    }

    private void HandleClickCancel()
    {
        Complete(ReturnToLobbyChoice.Cancel);
    }

    private void Complete(ReturnToLobbyChoice choice)
    {
        if (null == _completionSource)
        {
            return;
        }

        UniTaskCompletionSource<ReturnToLobbyChoice> source = _completionSource;
        _completionSource = null;
        source.TrySetResult(choice);
    }
}
