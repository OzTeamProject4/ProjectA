using System.Collections.Generic;
using System.ComponentModel;

public class StudentListModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs StudentListChanged = new PropertyChangedEventArgs(nameof(StudentList));

    private readonly List<StudentModel> _characterList = new List<StudentModel>();

    public IReadOnlyList<StudentModel> StudentList
    {
        get { return _characterList; }
    }

    public StudentListModel(IReadOnlyList<StudentModel> characterModels)
    {
        _characterList = new List<StudentModel>(characterModels);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void AddCharacter(StudentModel characterModel)
    {
        _characterList.Add(characterModel);
        OnPropertyChanged(StudentListChanged);
    }

    public StudentModel GetCharacter(string id)
    {
        foreach (var character in _characterList)
        {
            if (character.DataId == id)
            {
                return character;
            }
        }

        return null;
    }

    public void RemoveCharacter(StudentModel characterModel)
    {
        if (_characterList.Remove(characterModel))
        {
            OnPropertyChanged(StudentListChanged);
        }
    }

    private void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, propertyChangedEventArgs);
    }
}