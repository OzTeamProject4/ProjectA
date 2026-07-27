public class CloseEquipmentInfoPopup : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.CloseEquipmentInfoPopup();
    }
}