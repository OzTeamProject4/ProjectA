using System.Collections.Generic;

public class EquipmentCraftListModel
{
    private List<EquipmentCraftModel> _equipmentCraftModels;

    public IReadOnlyList<EquipmentCraftModel> EquipmentCraftModels
    {
        get { return _equipmentCraftModels; }
    }

    public EquipmentCraftListModel(List<EquipmentCraftModel> equipmentCraftModels)
    {
        _equipmentCraftModels = equipmentCraftModels;
    }

    public IReadOnlyList<EquipmentCraftModel> GetEquipmentCraftsByEquipType(EquipType equipType)
    {
        return null;
    }
}