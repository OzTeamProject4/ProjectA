using System.Collections.Generic;

public class EquipmentInventoryPopupViewModel
{
    private StudentModel _studentModel;

    public StudentModel StudentModel
    { 
        get
        { 
            return _studentModel;
        }
    }

    public IReadOnlyDictionary<string, EquipmentModel> FilteredEquipmentItems { get; private set; }

    public void SetModel(EquipType equipType, StudentModel studentModel)
    {
        _studentModel = studentModel;

        InventoryModel inventoryModel = NetworkManagerTemp.Instance.InventoryModel;
        FilteredEquipmentItems = inventoryModel.GetItemsByEquipType(equipType);
    }

    public void Dispose()
    {
        FilteredEquipmentItems = null;

        if (_studentModel == null)
        {
            return;
        }

        _studentModel = null;
    }
}    