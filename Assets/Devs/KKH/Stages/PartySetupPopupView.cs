using System;
using UnityEngine;
using UnityEngine.UI;

public class PartySetupPopupView : BaseUI
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _closeButton;

    // TODO: 파티 편성 로직 구현 시 각 슬롯 버튼에 캐릭터 선택 UI 연결
    [SerializeField] private Button[] _characterSlotButtons;
    [SerializeField] private Button _blockerButton;

    private bool _isSubscribed;

    public event Action OnStartButtonClicked;
    public event Action OnCloseButtonClicked;

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (_isSubscribed)
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
        OnStartButtonClicked?.Invoke();
    }

    private void HandleClickClose()
    {
        OnCloseButtonClicked?.Invoke();
    }
}