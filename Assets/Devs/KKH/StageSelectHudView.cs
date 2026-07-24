using UnityEngine;
using UnityEngine.UI;

public class StageSelectHudView : BaseUI
{
    [SerializeField] private Button _returnToLobbyButton;

    private StageSelectMapViewModel _viewModel;
    private bool _isSubscribed;

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();

        _viewModel = null;
    }

    public void Bind(StageSelectMapViewModel viewModel)
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
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        if (null == _returnToLobbyButton)
        {
            Debug.LogError("[StageSelectHudView] _returnToLobbyButton 이 인스펙터에 연결되지 않았습니다.");
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
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.RequestReturnToLobby();
    }
}
