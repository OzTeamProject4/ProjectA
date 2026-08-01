using Cysharp.Threading.Tasks;

public class LoadingButton : BaseButton
{
    protected override void OnButtonClick()
    {
        OpenLobbyAsync().Forget();
    }

    private async UniTask OpenLobbyAsync()
    {
        await GameManager.Instance.UIManager.OpenOverlayAsync();
        GameManager.Instance.UIManager.CloseLoading();
        await GameManager.Instance.UIManager.OpenLobbyAsync();
        GameManager.Instance.UIManager.CloseOverlay();
    }
}