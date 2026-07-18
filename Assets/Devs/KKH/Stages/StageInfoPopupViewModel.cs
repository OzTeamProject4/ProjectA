using System.Collections.Generic;
using UnityEngine;

public class StageInfoPopupViewModel
{
    private readonly StageData _stageData;
    private readonly IReadOnlyList<StageWaveData> _waves;

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

    public StageInfoPopupViewModel(StageData stageData, IReadOnlyList<StageWaveData> waves)
    {
        if (null == stageData)
        {
            Debug.LogError("[StageInfoPopupViewModel] stageData 가 null 입니다.");
        }

        _stageData = stageData;
        _waves = waves;
    }

    public IReadOnlyList<string> GetMonsterIds()
    {
        List<string> result = new List<string>();

        if (null == _waves)
        {
            return result;
        }

        foreach (StageWaveData wave in _waves)
        {
            if (null == wave)
            {
                continue;
            }

            foreach (string monsterId in wave.MonsterIds)
            {
                if (string.IsNullOrEmpty(monsterId))
                {
                    continue;
                }

                if (result.Contains(monsterId))
                {
                    continue;
                }

                result.Add(monsterId);
            }
        }

        return result;
    }
}