using System;
using System.Collections.Generic;
using System.ComponentModel;

public class EquipmentInfoPopupViewModel
{
    private StudentModel _studentModel;
    private EquipmentModel _equipmentModel;

    public string IconKey
    {
        get 
        {
            return _equipmentModel.IconKey;
        }
    }

    public IReadOnlyList<StatInfo> StatInfo
    {
        get 
        { 
            return _equipmentModel.StatInfos;
        }
    }

    public event Action<string> PropertyChanged;

    public void SetModel(EquipmentModel equipmentModel, StudentModel studentModel)
    {
        if (_equipmentModel != null)
        {
            _equipmentModel.PropertyChanged -= OnModelPropertyChanged;
        }

        _equipmentModel = equipmentModel;
        _equipmentModel.PropertyChanged += OnModelPropertyChanged;
    }

    public void Dispose()
    {
        if (_equipmentModel == null)
        {
            return;
        }

        _equipmentModel.PropertyChanged -= OnModelPropertyChanged;
        _equipmentModel = null;
    }

    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(e.PropertyName);
    }

    public void RequestEquip()
    {
        //_characterModel.Equip(_itemModel);
    }

    public void RequestUnequip()
    {
       // _characterModel.Unequip(_itemModel);
    }
}
