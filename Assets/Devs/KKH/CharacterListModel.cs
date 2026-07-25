using System.Collections.Generic;
using System.ComponentModel;

public class CharacterListModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs ListChanged = new PropertyChangedEventArgs(nameof(CharacterIdList));

    private readonly List<CharacterModel> _characterList = new List<CharacterModel>();

    public IReadOnlyList<CharacterModel> CharacterIdList
    {
        get { return _characterList; }
    }

    public CharacterListModel(IReadOnlyList<CharacterModel> characterModels)
    {
        _characterList = new List<CharacterModel>(characterModels);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void AddCharacter(CharacterModel characterModel)
    {
        _characterList.Add(characterModel);
        OnPropertyChanged(ListChanged);
    }

    public void RemoveCharacter(CharacterModel characterModel)
    {
        if (_characterList.Remove(characterModel))
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