public class RewardSlotViewModel
{
    private readonly string _itemId;
    private readonly int _count;

    public int Count
    {
        get { return _count; }
    }

    public RewardSlotViewModel(string itemId, int count)
    {
        _itemId = itemId;
        _count = count;
    }

    public string GetIconPath()
    {
        if (string.IsNullOrEmpty(_itemId))
        {
            return null;
        }

        if (!GameManager.Instance.DataManager.TryGetData(_itemId, out ItemData item))
        {
            return null;
        }

        if (null == item || string.IsNullOrEmpty(item.IconKey))
        {
            return null;
        }

        return item.IconKey;
    }
}
