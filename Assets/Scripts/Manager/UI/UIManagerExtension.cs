using Cysharp.Threading.Tasks;
using System.Threading;

public static class UIManagerExtension
{
    public static async UniTask OpenOverlayUIAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.Overlay, cancellationToken);
    }

    public static void CloseOverlayUI(this UIManager uiManager)
    {
        uiManager.Close(UIType.Overlay);
    }

    public static async UniTask OpenLoadingUIAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        await uiManager.OpenTestRootAsync(UIType.Loading, cancellationToken);
    }

    public static void CloseLoadingUI(this UIManager uiManager)
    {
        uiManager.Close(UIType.Loading);
    }
}
