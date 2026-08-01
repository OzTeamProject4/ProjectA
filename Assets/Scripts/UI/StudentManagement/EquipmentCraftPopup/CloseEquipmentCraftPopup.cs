public class CloseEquipmentCraftPopup : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.CloseEquipmentCraftPopup();
    }
}