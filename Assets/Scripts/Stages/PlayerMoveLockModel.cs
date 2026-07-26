using System;
using System.Collections.Generic;

public enum MoveLockReason
{
    StageInfoPopup,
    ReturnToLobbyPopup,
}

public class PlayerMoveLockModel
{
    private readonly HashSet<MoveLockReason> _lockReasons = new HashSet<MoveLockReason>();

    public bool CanMove
    {
        get { return _lockReasons.Count == 0; }
    }

    public event Action<bool> OnCanMoveChanged;

    public void Lock(MoveLockReason reason)
    {
        bool wasMovable = CanMove;

        if (!_lockReasons.Add(reason))
        {
            return;
        }

        NotifyIfChanged(wasMovable);
    }

    public void Unlock(MoveLockReason reason)
    {
        bool wasMovable = CanMove;

        if (!_lockReasons.Remove(reason))
        {
            return;
        }

        NotifyIfChanged(wasMovable);
    }

    public void ClearLocks()
    {
        if (_lockReasons.Count == 0)
        {
            return;
        }

        bool wasMovable = CanMove;

        _lockReasons.Clear();

        NotifyIfChanged(wasMovable);
    }

    private void NotifyIfChanged(bool wasMovable)
    {
        if (wasMovable == CanMove)
        {
            return;
        }

        OnCanMoveChanged?.Invoke(CanMove);
    }
}
