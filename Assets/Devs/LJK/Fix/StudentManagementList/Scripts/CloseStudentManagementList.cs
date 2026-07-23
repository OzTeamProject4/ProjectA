public class CloseStudentManagementList : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.CloseStudentManagementList();
    }
}