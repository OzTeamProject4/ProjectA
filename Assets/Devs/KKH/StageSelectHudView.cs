using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectHudView : BaseUI
{
    [SerializeField] private Button _returnToLobbyButton;

    private StageSelectHudViewModel _viewModel;
    private bool _isSubscribed;

    private void Awake()
    {
        UnityUtil.ValidateReference(_returnToLobbyButton, nameof(StageSelectHudView), nameof(_returnToLobbyButton));

        _viewModel = new StageSelectHudViewModel();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();

        if (null != _viewModel)
        {
            _viewModel.Dispose();
            _viewModel = null;
        }
    }

    private void Subscribe()
    {
        if (_isSubscribed || null == _returnToLobbyButton)
        {
            return;
        }

        _returnToLobbyButton.onClick.AddListener(HandleReturnToLobbyClicked);

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        if (null != _returnToLobbyButton)
        {
            _returnToLobbyButton.onClick.RemoveListener(HandleReturnToLobbyClicked);
        }

        _isSubscribed = false;
    }

    private void HandleReturnToLobbyClicked()
    {
        ShowReturnToLobbyPopupAsync().Forget();
    }

    private async UniTaskVoid ShowReturnToLobbyPopupAsync()
    {
        StopPlayer();

        ReturnToLobbyChoice choice = await WaitForReturnToLobbyChoiceAsync();

        if (choice == ReturnToLobbyChoice.Confirm)
        {
            if (null != _viewModel)
            {
                _viewModel.ReturnToLobby();
            }

            return;
        }

        ResumePlayer();
    }

    private async UniTask<ReturnToLobbyChoice> WaitForReturnToLobbyChoiceAsync()
    {
        ReturnToLobbyPopupView view = await GameManager.Instance.UIManager.OpenReturnToLobbyPopupAsync(destroyCancellationToken);

        if (null == view)
        {
            Debug.LogError("[StageSelectHudView] 로비 복귀 팝업을 열지 못했습니다. 복귀를 취소합니다.");
            return ReturnToLobbyChoice.Cancel;
        }

        ReturnToLobbyChoice choice = await view.WaitForChoiceAsync();

        GameManager.Instance.UIManager.CloseReturnToLobbyPopup();

        return choice;
    }

    private void StopPlayer()
    {
        StageSession session = StageSession.Instance;

        if (null == session || null == session.Player)
        {
            return;
        }

        session.Player.StopMove();
    }

    private void ResumePlayer()
    {
        StageSession session = StageSession.Instance;

        if (null == session || null == session.Player)
        {
            return;
        }

        session.Player.ResumeMove();
    }
}
