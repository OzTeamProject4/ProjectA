public class StageData : BaseData
{
    public string StageName {  get; init; }
    public string MapPrefabKey { get; init; }
    public int WaveCount { get; init; }
    public string RewardItemId { get; init; }
    public string RewardItemCount { get; init; }

    private string[] _rewardItemIds;
    private int[] _rewardItemCounts;

    public string[] RewardItemIds
    {
        get
        {
            if (_rewardItemIds == null)
            {
                _rewardItemIds = Util.ParseIds(RewardItemId);
            }

            return _rewardItemIds;
        }
    }

    public int[] RewardItemCounts
    {
        get
        {
            if (_rewardItemCounts == null)
            {
                _rewardItemCounts = Util.ParseCounts(RewardItemCount);
            }

            return _rewardItemCounts;
        }
    }

    public bool TryGetRewards(out (string ItemId, int Count)[] rewards)
    {
        return Util.TryPairMaterials(RewardItemIds, RewardItemCounts, out rewards);
    }
}
