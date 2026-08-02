using System.Collections.Generic;

public class StudentManagementListViewModel
{
    private StudentListModel _studentListModel;

    public IReadOnlyList<StudentModel> StudentList
    {
        get { return _studentListModel.StudentList; }
    }

    public StudentManagementListViewModel()
    {
        _studentListModel = NetworkManager.Instance.StudentListModel;
    }

    public void Dispose()
    {
        _studentListModel = null;
    }
}