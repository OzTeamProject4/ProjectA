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

        _viewModel = null;
    }

    public void Bind(StageSelectHudViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("[StageSelectHudView] Bind: viewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();

        _viewModel = viewModel;

        Subscribe();
    }

    private void Subscribe()
    {
        if (_isSubscribed || null == _returnToLobbyButton || null == _viewModel)
        {
            return;
        }

        _returnToLobbyButton.onClick.AddListener(HandleReturnToLobbyClicked);

        _viewModel.OnReturnToLobbyConfirmRequested += HandleReturnToLobbyConfirmRequested;

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

        if (null != _viewModel)
        {
            _viewModel.OnReturnToLobbyConfirmRequested -= HandleReturnToLobbyConfirmRequested;
        }

        _isSubscribed = false;
    }

    private void HandleReturnToLobbyClicked()
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.ReturnToLobbyCommand();
    }

    private void HandleReturnToLobbyConfirmRequested()
    {
        ShowReturnToLobbyPopupAsync().Forget();
    }

    private async UniTaskVoid ShowReturnToLobbyPopupAsync()
    {
        ReturnToLobbyChoice choice = await WaitForReturnToLobbyChoiceAsync();

        if (null == _viewModel)
        {
            return;
        }

        if (choice == ReturnToLobbyChoice.None)
        {
            _viewModel.ReturnToLobbyPopupFailedCommand();
            return;
        }

        if (choice == ReturnToLobbyChoice.Confirm)
        {
            _viewModel.ConfirmReturnToLobbyCommand();
            return;
        }

        _viewModel.CancelReturnToLobbyCommand();
    }

    private async UniTask<ReturnToLobbyChoice> WaitForReturnToLobbyChoiceAsync()
    {
        ReturnToLobbyPopupView view = await GameManager.Instance.UIManager.OpenReturnToLobbyPopupAsync(destroyCancellationToken);

        if (null == view)
        {
            return ReturnToLobbyChoice.None;
        }

        ReturnToLobbyChoice choice = await view.WaitForChoiceAsync();

        GameManager.Instance.UIManager.CloseReturnToLobbyPopup();

        return choice;
    }
}
