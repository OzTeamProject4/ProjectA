using System.Collections.Generic;

public class EquipmentCraftPopupViewModel
{
    private EquipmentCraftListModel _equipmentCraftListModel;

    private IReadOnlyList<EquipmentCraftModel> _filteredEquipmentCraftModels;

    public IReadOnlyList<EquipmentCraftModel> FilteredEquipmentCraftModels
    {
        get { return _filteredEquipmentCraftModels; }
    }

    public EquipmentCraftPopupViewModel()
    {
        _equipmentCraftListModel = NetworkManager.Instance.EquipmentCraftListModel;
    }

    public void UpdateEquipmentCraftModels(EquipType equipType)
    {
        _filteredEquipmentCraftModels = _equipmentCraftListModel.GetEquipmentCraftsByEquipType(equipType);
    }

    public void Dispose()
    {
        _filteredEquipmentCraftModels = null;
    }
}