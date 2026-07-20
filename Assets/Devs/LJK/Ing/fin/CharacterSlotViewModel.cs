using System;
using System.ComponentModel;

public class CharacterSlotViewModel
{
    private CharacterModel _characterModel;
    
    public string Id
    {
        get
        { 
            return _characterModel.Id; 
        }
    }

    public string Name
    {
        get
        { 
            return _characterModel.Name; 
        }
    }

    public int Star
    {
        get 
        { 
            return _characterModel.Star; 
        }
    }

    public string IconPath
    {
        get 
        { 
            return _characterModel.IconPath; 
        }
    }

    public event Action<string> ModelPropertyChanged;

    public CharacterSlotViewModel(string characterId)
    {
        //Test
        switch (characterId)
        {
            case "1":
                _characterModel = new CharacterModel(new CharacterData("1", "One", 3, "Test"));
                break;
            case "2":
                _characterModel = new CharacterModel(new CharacterData("2", "Two", 4, "Test"));
                break;
        }

        _characterModel.PropertyChanged += OnModelPropertyChanged;
    }

    public void Init()
    {
        _characterModel.InitProperty();
    }

    public void Dispose()
    {
        _characterModel.PropertyChanged -= OnModelPropertyChanged;
        _characterModel = null;
    }

    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ModelPropertyChanged == null)
        {
            return;
        }

        ModelPropertyChanged.Invoke(e.PropertyName);
    }
}