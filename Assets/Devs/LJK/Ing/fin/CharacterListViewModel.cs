using System;
using System.Collections.Generic;
using System.ComponentModel;

public class CharacterListViewModel
{
    private CharacterListModel _characterListModel;

    public IReadOnlyList<string> CharacterIdList
    {
        get { return _characterListModel.CharacterIdList; }
    }

    public event Action<string> ModelPropertyChanged;

    public CharacterListViewModel()
    {
        //Test
        IReadOnlyList<string> strings = new List<string>() { "1", "2" };

        _characterListModel = new CharacterListModel(strings);
        _characterListModel.PropertyChanged += OnModelPropertyChanged;
    }

    public void Dispose()
    {
        _characterListModel.PropertyChanged -= OnModelPropertyChanged;
        _characterListModel = null;
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