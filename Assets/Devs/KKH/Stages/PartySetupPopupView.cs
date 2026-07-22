using UnityEngine;
using UnityEngine.UI;

public class PartySetupPopupView : BaseUI
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _closeButton;

    [SerializeField] private Button[] _characterSlotButtons;

    [SerializeField] private Button _blockerButton;

    private PartySetupPopupViewModel _viewModel;
    private bool _isSubscribed;

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Bind(PartySetupPopupViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("[PartySetupPopupView] Bind: viewModel 이 null 입니다.");
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

        if (null != _startButton)
        {
            _startButton.onClick.AddListener(HandleClickStart);
        }

        if (null != _closeButton)
        {
            _closeButton.onClick.AddListener(HandleClickClose);
        }

        if (null != _blockerButton)
        {
            _blockerButton.onClick.AddListener(HandleClickClose);
        }

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        if (null != _startButton)
        {
            _startButton.onClick.RemoveListener(HandleClickStart);
        }

        if (null != _closeButton)
        {
            _closeButton.onClick.RemoveListener(HandleClickClose);
        }

        if (null != _blockerButton)
        {
            _blockerButton.onClick.RemoveListener(HandleClickClose);
        }

        _isSubscribed = false;
    }

    private void HandleClickStart()
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.StartBattleCommand();
    }

    private void HandleClickClose()
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.CloseCommand();
    }
}