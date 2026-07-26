using System;
using System.Collections.Generic;
using System.ComponentModel;

public class EquipmentInfoPopupViewModel
{
    private StudentModel _studentModel;
    private EquipmentModel _equipmentModel;

    public string Name
    {
        get
        {
            return _equipmentModel.Name;
        }
    }

    public string Description
    {
        get
        {
            return _equipmentModel.Description;
        }
    }

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

    public bool IsEquippedByCurrentStudent
    {
        get
        {
            if (_studentModel == null || _equipmentModel == null)
            {
                return false;
            }

            return _equipmentModel.EquippedBy == _studentModel.DataId;
        }
    }

    public void SetModel(EquipmentModel equipmentModel, StudentModel studentModel)
    {
        if (_equipmentModel != null)
        {
            _equipmentModel.PropertyChanged -= OnModelPropertyChanged;
        }

        _studentModel = studentModel;
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
        if (_studentModel == null || _equipmentModel == null)
        {
            return;
        }

        NetworkManagerTemp.Instance.InventoryModel.TryEquip(_studentModel, _equipmentModel.InstanceId);
    }

    public void RequestUnequip()
    {
        if (_studentModel == null || _equipmentModel == null)
        {
            return;
        }

        if (!IsEquippedByCurrentStudent)
        {
            return;
        }

        NetworkManagerTemp.Instance.InventoryModel.TryUnequip(_studentModel, _equipmentModel.EquipType);
    }
}
