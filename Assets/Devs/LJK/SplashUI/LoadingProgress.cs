public readonly struct LoadingProgress
{
    public float Progress { get; }
    public LoadingStep LoadingStep { get; }

    public LoadingProgress(float progress, LoadingStep loadingStep)
    {
        Progress = progress;
        LoadingStep = loadingStep;
    }
}
