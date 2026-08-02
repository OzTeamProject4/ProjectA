using System.Collections.Generic;

public class ExperienceInventoryPopupViewModel
{
    private StudentModel _studentModel;

    public IReadOnlyDictionary<string, MaterialModel> ExperienceItems { get; private set; }

    public bool CanUseExpItem
    {
        get
        {
            if (null == _studentModel)
            {
                return false;
            }

            return !_studentModel.IsMaxLevel;
        }
    }

    public void SetModel(StudentModel characterModel)
    {
        _studentModel = characterModel;

        InventoryModel inventoryModel = NetworkManager.Instance.InventoryModel;
        ExperienceItems = inventoryModel.GetItemsByMaterialType(CurrencyType.ExpBook);
    }

    public void UseExpItem(MaterialModel materialModel)
    {
        if (!CanUseExpItem)
        {
            return;
        }

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