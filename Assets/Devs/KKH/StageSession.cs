public class StageSession
{
    public static StageSession Instance { get; private set; }

    public ScreenStateModel ScreenState { get; }
    public StageProgressModel Progress { get; }
    public StageDataModel Stages { get; }

    private StageSession()
    {
        ScreenState = new ScreenStateModel(ScreenType.StageSelect);
        Progress = new StageProgressModel();
        Stages = new StageDataModel();
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
