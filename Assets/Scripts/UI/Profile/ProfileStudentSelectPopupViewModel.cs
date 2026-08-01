using System.Collections.Generic;

public class ProfileStudentSelectPopupViewModel
{
    private StudentListModel _studentListModel;

    public IReadOnlyList<StudentModel> StudentList
    {
        get
        {
            return _studentListModel.StudentList;
        }
    }

    public ProfileStudentSelectPopupViewModel()
    {
        _studentListModel = NetworkManager.Instance.StudentListModel;
    }

    public void Dispose()
    {
        _studentListModel = null;
    }
}
