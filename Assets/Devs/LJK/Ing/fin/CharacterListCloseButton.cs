public class CharacterListCloseButton : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.CloseCharacterList();
    }
}
