public class CloseExperienceInventoryPopup : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.CloseExperienceInventoryPopup();
    }
}
