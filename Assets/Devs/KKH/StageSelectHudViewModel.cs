using System;
using UnityEngine;

public class StageSelectHudViewModel
{
    private ScreenStateModel _screenStateModel;
    private StageSelectPlayer _player;

    public event Action OnReturnToLobbyConfirmRequested;

    public StageSelectHudViewModel(ScreenStateModel screenStateModel, StageSelectPlayer player)
    {
        if (null == screenStateModel)
        {
            Debug.LogError("[StageSelectHudViewModel] screenStateModel 이 null 입니다.");
        }

        if (null == player)
        {
            Debug.LogError("[StageSelectHudViewModel] player 가 null 입니다.");
        }

        _screenStateModel = screenStateModel;
        _player = player;
    }

    // ===== 로비 복귀 =====

    public void ReturnToLobbyCommand()
    {
        StopPlayer();

        OnReturnToLobbyConfirmRequested?.Invoke();
    }

    public void ConfirmReturnToLobbyCommand()
    {
        if (null == _screenStateModel)
        {
            return;
        }

        _screenStateModel.ChangeScreen(ScreenType.Lobby);
    }

    public void CancelReturnToLobbyCommand()
    {
        ResumePlayer();
    }

    public void ReturnToLobbyPopupFailedCommand()
    {
        Debug.LogError("[StageSelectHudViewModel] 로비 복귀 팝업을 열지 못했습니다. 복귀를 취소합니다.");

        ResumePlayer();
    }

    // ===== 플레이어 이동 제어 =====

    private void StopPlayer()
    {
        if (null == _player)
        {
            return;
        }

        _player.StopMove();
    }

    private void ResumePlayer()
    {
        if (null == _player)
        {
            return;
        }

        _player.ResumeMove();
    }

    public void Dispose()
    {
        _screenStateModel = null;
        _player = null;
    }
}
