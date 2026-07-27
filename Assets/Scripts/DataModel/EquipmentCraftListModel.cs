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
        List<EquipmentCraftModel> filteredCraftModels = new();

        foreach (EquipmentCraftModel craftModel in _equipmentCraftModels)
        {
            if (craftModel.EquipType != equipType)
            {
                continue;
            }

            filteredCraftModels.Add(craftModel);
        }

        return filteredCraftModels;
    }
}