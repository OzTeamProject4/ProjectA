using Cysharp.Threading.Tasks;
using System.Threading;

public static class UIManagerExtension
{
    public static async UniTask OpenTestUIAsync(this UIManager uiManager, CancellationToken cancellationToken = default)
    {
        BaseUI baseUI = await uiManager.OpenTestRootAsync(UIType.Test, cancellationToken);
    }

    public static void CloseTestUI(this UIManager uiManager)
    {
        uiManager.Close(UIType.Test);
    }
}
