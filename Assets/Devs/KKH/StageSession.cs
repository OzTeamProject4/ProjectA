public class StageSession
{
    public static StageSession Instance { get; private set; }

    public ScreenStateModel ScreenState { get; }
    public StageProgressModel Progress { get; }
    public StageSelectPlayer Player { get; set; }

    private StageSession()
    {
        ScreenState = new ScreenStateModel(ScreenType.StageSelect);
        Progress = new StageProgressModel();
    }

    public static StageSession Create()
    {
        Instance = new StageSession();
        return Instance;
    }

    public static void Clear()
    {
        Instance = null;
    }
}
