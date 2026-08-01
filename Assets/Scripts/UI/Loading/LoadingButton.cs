using Cysharp.Threading.Tasks;

public class LoadingButton : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.OpenStudentGachaAsync().Forget();
    }
}