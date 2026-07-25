using Cysharp.Threading.Tasks;

public class LoadingButton : BaseButton
{
    protected override void OnButtonClick()
    {
        CompleteLoadingAsync().Forget();
    }

    private async UniTask CompleteLoadingAsync()
    {
        await GameManager.Instance.UIManager.OpenOverlayAsync();
        GameManager.Instance.UIManager.CloseLoading();
        await GameManager.Instance.UIManager.OpenLobbyAsync();
        GameManager.Instance.UIManager.CloseOverlay();
    }
}