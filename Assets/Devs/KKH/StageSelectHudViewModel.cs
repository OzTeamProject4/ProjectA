public class StageSelectHudViewModel
{
    private ScreenStateModel _screenStateModel;

    public StageSelectHudViewModel()
    {
        StageSession session = StageSession.Instance;

        if (null == session)
        {
            UnityEngine.Debug.LogError("[StageSelectHudViewModel] StageSession.Instance 가 null 입니다.");
            return;
        }

        _screenStateModel = session.ScreenState;
    }

    public void ReturnToLobby()
    {
        if (null == _screenStateModel)
        {
            return;
        }

        _screenStateModel.ChangeScreen(ScreenType.Lobby);
    }

    public void Dispose()
    {
        _screenStateModel = null;
    }
}
