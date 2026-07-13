using Cysharp.Threading.Tasks;

public static class UIManagerExtension
{
    public static async UniTask OpenTestUIAsync(this UIManager uiManager)
    {
        await uiManager.OpenTestRootAsync(UIType.Test);
    }

    public static void CloseTestUI(this UIManager uiManager)
    {
        uiManager.Close(UIType.Test);
    }
}
