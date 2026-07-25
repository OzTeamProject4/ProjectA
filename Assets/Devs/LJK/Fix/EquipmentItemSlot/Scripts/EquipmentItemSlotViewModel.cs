using System;
using System.ComponentModel;

public class EquipmentItemSlotViewModel
{
    private EquipmentModel _equipmentModel;

    public EquipmentModel EquipmentModel
    {
        get
        {
            return _equipmentModel;
        }
    }

    public string IconKey
    {
        get
        {
            return _equipmentModel.IconKey;
        }
    }

    public event Action<string> PropertyChanged;

    public void SetModel(EquipmentModel equipmentModel)
    {
        if (_equipmentModel != null)
        {
            _equipmentModel.PropertyChanged -= OnPropertyChanged;
        }

        _equipmentModel = equipmentModel;
        _equipmentModel.PropertyChanged += OnPropertyChanged;
    }

    public void Refresh()
    {
        _equipmentModel.NotifyAllProperties();
    }

    public void Dispose()
    {
        if (_equipmentModel != null)
        {
            _equipmentModel.PropertyChanged -= OnPropertyChanged;
        }

        _equipmentModel = null;
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