using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ExperienceInventoryPopupViewModel
{
    private StudentModel _studentModel;

    public IReadOnlyDictionary<string, MaterialModel> ExperienceItems { get; private set; }

    public void SetModel(StudentModel characterModel)
    {
        _studentModel = characterModel;

        InventoryModel inventoryModel = NetworkManagerTemp.Instance.InventoryModel;
        ExperienceItems = inventoryModel.GetItemsByMaterialType(MaterialType.Exp);
    }

    public void UseExpItem(MaterialModel materialModel)
    {
        materialModel.UseExpItem(_studentModel);
    }

    public void Dispose()
    {
        ExperienceItems = null;

        if (_studentModel == null)
        {
            return;
        }

        _studentModel = null;
    }
}