public class ExpItemSlotViewModel
{
    private readonly CharacterModel _model;
    private readonly ItemData _itemData;

    public string DataId
    {
        get { return _itemData.DataId; }
    }

    public string Name
    {
        get { return _itemData.Name; }
    }

    public int OwnedCount
    {
        get { return _model.GetItemCount(DataId); }
    }

    public bool IsUsable
    {
        get { return OwnedCount > 0; }
    }

    public ExpItemSlotViewModel(CharacterModel model, ItemData itemData)
    {
        _model = model;
        _itemData = itemData;
    }
}
