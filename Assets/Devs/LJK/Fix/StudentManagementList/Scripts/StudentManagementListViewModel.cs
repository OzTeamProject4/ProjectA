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
        _studentListModel = NetworkManagerTemp.Instance.StudentListModel;
    }

    public void Dispose()
    {
        _studentListModel = null;
    }
}