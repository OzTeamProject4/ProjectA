using System;
using System.Collections.Generic;

public class ExperienceInventoryPopupViewModel
{
    private StudentModel _studentModel;
    private InventoryModel _inventoryModel;

    public IReadOnlyDictionary<string, MaterialModel> ExperienceItems { get; private set; }   

    public event Action<string> PropertyChanged;

    public ExperienceInventoryPopupViewModel()
    {
        _inventoryModel = NetworkManagerTemp.Instance.InventoryModel;
    }

    public void SetModel(StudentModel characterModel)
    {
        _studentModel = characterModel;
        ExperienceItems = _inventoryModel.GetItemsByMaterialType(MaterialType.Exp);
    }

    public void UseExpItem(MaterialModel materialModel)
    {
        materialModel.UseExpItem(_studentModel);
    }

    public void Dispose()
    {
        _inventoryModel = null;

        if (_studentModel == null)
        {
            return;
        }

        _studentModel = null;
    }
}