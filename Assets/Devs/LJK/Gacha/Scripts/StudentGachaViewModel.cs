using System;
using System.ComponentModel;

public class StudentGachaViewModel
{
    private StudentGachaModel _studentGachaModel;

    public string Name
    {
        get
        {
            return _studentGachaModel.Name;
        }
    }

    public string MainKey
    {
        get
        {
            return _studentGachaModel.MainKey;
        }
    }

    public string PortraitKey
    {
        get
        {
            return _studentGachaModel.PortraitKey;
        }
    }

    public event Action<string> ModelPropertyChanged;

    public StudentGachaViewModel()
    {
        _studentGachaModel = NetworkManager.Instance.StudentGachaModel;
    }

    public void OnEnable()
    {
        _studentGachaModel.PropertyChanged += OnModelPropertyChanged;
    }

    public void OnDisable()
    {
        _studentGachaModel.PropertyChanged -= OnModelPropertyChanged;
    }

    public void Dispose()
    {
        _studentGachaModel = null;
    }

    public void RequestUpdateGacha(string id)
    {
        _studentGachaModel.UpdateGachaData(id);
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