public class CloseGacha : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.CloseStudentGacha();
    }
}
