//TODO 해당 뷰 닫기하면 다같이 열린 팝업 닫히도록 기능 추가
public class CloseStudentManagement : BaseButton
{
    protected override void OnButtonClick()
    {
        GameManager.Instance.UIManager.CloseStudentManagement();
    }
}