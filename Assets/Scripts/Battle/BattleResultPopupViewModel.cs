using System.Collections.Generic;

public class BattleResultPopupViewModel
{
    private readonly bool _isVictory;
    private readonly StageData _stageData;

    public bool IsVictory
    {
        get { return _isVictory; }
    }

    public string StageName
    {
        get
        {
            if (null == _stageData)
            {
                return string.Empty;
            }

            return _stageData.StageName;
        }
    }

    public BattleResultPopupViewModel(bool isVictory, string stageId)
    {
        _isVictory = isVictory;
        _stageData = GetStage(stageId);
    }

    private StageData GetStage(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            return null;
        }

        GameManager.Instance.DataManager.TryGetData(stageId, out StageData data);
        return data;
    }

    public IReadOnlyList<RewardSlotViewModel> GetRewards()
    {
        List<RewardSlotViewModel> result = new List<RewardSlotViewModel>();

        // 보상은 승리 시에만
        if (!_isVictory || null == _stageData)
        {
            return result;
        }

        if (!_stageData.TryGetRewards(out (string ItemId, int Count)[] rewards))
        {
            return result;
        }

        foreach ((string ItemId, int Count) reward in rewards)
        {
            result.Add(new RewardSlotViewModel(reward.ItemId, reward.Count));
        }

        return result;
    }
}
