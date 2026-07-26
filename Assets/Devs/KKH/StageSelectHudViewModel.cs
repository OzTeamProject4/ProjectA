using System;
using UnityEngine;

public class StageSelectHudViewModel
{
    private ScreenStateModel _screenStateModel;
    private PlayerMoveLockModel _moveLockModel;

    public event Action OnReturnToLobbyConfirmRequested;

    public StageSelectHudViewModel(ScreenStateModel screenStateModel, PlayerMoveLockModel moveLockModel)
    {
        if (null == screenStateModel)
        {
            Debug.LogError("[StageSelectHudViewModel] screenStateModel 이 null 입니다.");
        }

        if (null == moveLockModel)
        {
            Debug.LogError("[StageSelectHudViewModel] moveLockModel 이 null 입니다.");
        }

        _screenStateModel = screenStateModel;
        _moveLockModel = moveLockModel;
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
        if (null == _moveLockModel)
        {
            return;
        }

        _moveLockModel.Lock(MoveLockReason.ReturnToLobbyPopup);
    }

    private void ResumePlayer()
    {
        if (null == _moveLockModel)
        {
            return;
        }

        _moveLockModel.Unlock(MoveLockReason.ReturnToLobbyPopup);
    }

    public void Dispose()
    {
        ResumePlayer();

        _screenStateModel = null;
        _moveLockModel = null;
    }
}
