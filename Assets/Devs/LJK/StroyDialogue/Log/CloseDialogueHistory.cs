public class CloseDialogueHistory : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.CloseDialogueHistory();
    }
}
