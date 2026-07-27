using System;
using System.ComponentModel;

public class ExperienceItemSlotViewModel
{
    private MaterialModel _materialModel;
   
    public MaterialModel MaterialModel
    {
        get
        { 
            return _materialModel;
        }
    }

    public string Name
    {
        get 
        { 
            return _materialModel.Name;
        }
    }

    public string IconKey
    {
        get
        {
            return _materialModel.IconKey;
        }

    }

    public int Count
    {
        get 
        { 
            return _materialModel.Count ;
        }
    }

    public event Action<string> PropertyChanged;

    public void SetModel(MaterialModel materialModel)
    {
        if (_materialModel != null)
        {
            _materialModel.PropertyChanged -= OnPropertyChanged;
        }

        _materialModel = materialModel;
        _materialModel.PropertyChanged += OnPropertyChanged;
    }

    public void Refresh()
    {
        _materialModel.NotifyAllProperties();
    }

    public void Dispose()
    {
        if (_materialModel == null)
        {
            return;
        }

        _materialModel.PropertyChanged -= OnPropertyChanged;
        _materialModel = null;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(e.PropertyName);
    }
}
