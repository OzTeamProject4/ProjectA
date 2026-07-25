using System.Collections.Generic;

public class CraftEquipmentInfoPopupViewModel
{
    private EquipmentCraftModel _equipmentCraftModel;

    public string Name
    {
        get { return _equipmentCraftModel.Name; }
    }

    public string IconKey
    {
        get { return _equipmentCraftModel.IconKey; }
    }

    public IReadOnlyList<StatInfo> StatInfos
    {
        get { return _equipmentCraftModel.StatInfos; }
    }

    public void SetModel(EquipmentCraftModel equipmentCraftModel)
    {
        _equipmentCraftModel = equipmentCraftModel;
    }

    public void Dispose()
    {
        _equipmentCraftModel = null;
    }
}