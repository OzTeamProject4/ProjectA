public class CloseCraftEquipmentInfoPopup : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.CraftEquipmentInfoPopup();
    }
}