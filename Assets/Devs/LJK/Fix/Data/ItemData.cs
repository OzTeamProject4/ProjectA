public class ItemData : BaseData
{
    public string Name { get; init; }
    public string Description { get; init; }
    public ItemType ItemType { get; init; }
    public string IconKey { get; init; }
    public string ForeignKey { get; init; }
}

public class EquipmentData : BaseData
{
    public EquipType EquipmentType { get; init; }
    public string AllowedId { get; init; }
    public string RequiredItemId { get; init; }
    public string RequiredItemCount { get; init; }
    public float BonusRate { get; init; }
    public float MaxHp { get; init; }
    public float Attack { get; init; }
    public float Defence { get; init; }
    public float MoveSpeed { get; init; }
    public string SpritePath { get; init; }
    public string Description { get; init; }

    private string[] _requiredItemIds;
    private int[] _requiredItemCounts;

    public string[] RequiredItemIds
    {
        get
        {
            if (_requiredItemIds == null)
            {
                _requiredItemIds = Util.ParseIds(RequiredItemId);
            }

            return _requiredItemIds;
        }
    }

    public int[] RequiredItemCounts
    {
        get
        {
            if (_requiredItemCounts == null)
            {
                _requiredItemCounts = Util.ParseCounts(RequiredItemCount);
            }

            return _requiredItemCounts;
        }
    }
}

