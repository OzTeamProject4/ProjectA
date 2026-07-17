using Cysharp.Threading.Tasks;

public class LoadingButton : BaseButton
{
    protected override void OnButtonClick()
    {
        CompleteLoadingAsync().Forget();
    }

    private async UniTask CompleteLoadingAsync()
    {
        await GameManager.Instance.UIManager.OpenOverlayUIAsync();
        GameManager.Instance.UIManager.CloseLoadingUI();
        //오픈 로비
    }
}