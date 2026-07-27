using System;
using UnityEngine;

public class StagePlayerPartyViewModel
{
    private PlayerMoveLockModel _moveLockModel;
    private ScreenStateModel _screenStateModel;

    public bool CanMove
    {
        get
        {
            return null == _moveLockModel || _moveLockModel.CanMove;
        }
    }

    public bool IsActive
    {
        get
        {
            return null != _screenStateModel && _screenStateModel.CurrentScreen == ScreenType.StageSelect;
        }
    }

    public event Action<bool> OnCanMoveChanged;
    public event Action<bool> OnActiveChanged;

    public StagePlayerPartyViewModel(PlayerMoveLockModel moveLockModel, ScreenStateModel screenStateModel)
    {
        if (null == moveLockModel)
        {
            Debug.LogError("[StagePlayerPartyViewModel] moveLockModel 이 null 입니다.");
        }

        if (null == screenStateModel)
        {
            Debug.LogError("[StagePlayerPartyViewModel] screenStateModel 이 null 입니다.");
        }

        _moveLockModel = moveLockModel;
        _screenStateModel = screenStateModel;

        if (null != _moveLockModel)
        {
            _moveLockModel.OnCanMoveChanged += HandleCanMoveChanged;
        }

        if (null != _screenStateModel)
        {
            _screenStateModel.OnScreenChanged += HandleScreenChanged;
        }
    }

    public void Refresh()
    {
        OnActiveChanged?.Invoke(IsActive);
        OnCanMoveChanged?.Invoke(CanMove);
    }

    public void Dispose()
    {
        if (null != _moveLockModel)
        {
            _moveLockModel.OnCanMoveChanged -= HandleCanMoveChanged;
        }

        if (null != _screenStateModel)
        {
            _screenStateModel.OnScreenChanged -= HandleScreenChanged;
        }

        _moveLockModel = null;
        _screenStateModel = null;
    }

    private void HandleCanMoveChanged(bool canMove)
    {
        OnCanMoveChanged?.Invoke(canMove);
    }

    private void HandleScreenChanged(ScreenType screen)
    {
        OnActiveChanged?.Invoke(IsActive);
    }
}
