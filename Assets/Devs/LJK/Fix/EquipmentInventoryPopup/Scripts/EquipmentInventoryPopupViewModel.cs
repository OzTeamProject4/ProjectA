using System;
using System.Collections.Generic;
using System.ComponentModel;

public class EquipmentInventoryPopupViewModel
{
    private StudentModel _studentModel;
    private InventoryModel _inventoryModel;
    private EquipType _equipType;

    public StudentModel StudentModel
    { 
        get
        { 
            return _studentModel;
        }
    }

    // 같은 장비를 여러 개 가질 수 있어 InstanceId 리스트로
    public IReadOnlyList<EquipmentModel> FilteredEquipmentItems { get; private set; }

    public event Action EquipmentsChanged;

    public void SetModel(EquipType equipType, StudentModel studentModel)
    {
        _studentModel = studentModel;
        _equipType = equipType;

        if (_inventoryModel == null)
        {
            _inventoryModel = NetworkManagerTemp.Instance.InventoryModel;
            _inventoryModel.PropertyChanged += OnInventoryPropertyChanged;
        }

        RefreshFilteredItems();
    }

    private void RefreshFilteredItems()
    {
        FilteredEquipmentItems = _inventoryModel.GetEquipmentsByEquipType(_equipType);
    }

    private void OnInventoryPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(InventoryModel.Equipments))
        {
            return;
        }

        RefreshFilteredItems();

        if (EquipmentsChanged == null)
        {
            return;
        }

        EquipmentsChanged.Invoke();
    }

    public void RequestEquip(EquipmentModel equipmentModel)
    {
        if (_studentModel == null || equipmentModel == null)
        {
            return;
        }

        NetworkManagerTemp.Instance.InventoryModel.TryEquip(_studentModel, equipmentModel.InstanceId);
    }

    public void Dispose()
    {
        FilteredEquipmentItems = null;

        if (_inventoryModel != null)
        {
            _inventoryModel.PropertyChanged -= OnInventoryPropertyChanged;
            _inventoryModel = null;
        }

        _studentModel = null;
    }
}    