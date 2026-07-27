public class CloseEquipmentInventoryPopup : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.CloseEquipmentInventoryPopup();
    }
}
