using System.Collections.Generic;
using System.ComponentModel;

public class CharacterListModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs ListChanged = new PropertyChangedEventArgs(nameof(CharacterIdList));

    private readonly List<string> _characterIdList = new List<string>();

    public IReadOnlyList<string> CharacterIdList
    {
        get { return _characterIdList; }
    }

    public CharacterListModel(IReadOnlyList<string> strings)
    {
        _characterIdList = new List<string>(strings);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void AddCharacter(string characterId)
    {
        _characterIdList.Add(characterId);
        OnPropertyChanged(ListChanged);
    }

    public void RemoveCharacter(string characterId)
    {
        if (_characterIdList.Remove(characterId))
        {
            OnPropertyChanged(ListChanged);
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