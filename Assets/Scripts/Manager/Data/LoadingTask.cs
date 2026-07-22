using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class LoadingTask
{
    public LoadingStep Step { get; }
    public string Key { get; }
    public Func<string, CancellationToken, UniTask> Execute { get; }

    public LoadingTask(LoadingStep step, string key, Func<string, CancellationToken, UniTask> execute)
    {
        Step = step;
        Key = key;
        Execute = execute;
    }
}
