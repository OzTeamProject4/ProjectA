public class SignatureData : BaseData
{
    public int EnchantLevel { get; init; }
    public float MaxHp { get; init; }
    public float Attack { get; init; }
    public float Defence { get; init; }
    public float MoveSpeed { get; init; }
    public string RequiredItemId { get; init; }
    public string RequiredItemCount { get; init; }

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

    public bool TryGetRequiredMaterials(out (string ItemId, int Count)[] materials)
    {
        if (!Util.TryPairMaterials(RequiredItemIds, RequiredItemCounts, out materials))
        {
            return false;
        }

        return true;
    }
}