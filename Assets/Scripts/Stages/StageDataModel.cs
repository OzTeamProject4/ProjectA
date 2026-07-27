using System;
using System.Collections.Generic;
using UnityEngine;

public class StageDataModel
{
    private readonly Dictionary<string, List<StageWaveData>> _wavesByStageId = new Dictionary<string, List<StageWaveData>>();

    public StageDataModel()
    {
        BuildWaveTable();
    }

    public StageData GetStage(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning("[StageDataModel] GetStage: stageId 가 비어 있습니다.");
            return null;
        }

        GameManager.Instance.DataManager.TryGetData(stageId, out StageData data);

        return data;
    }

    public IReadOnlyList<StageWaveData> GetStageWaves(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning("[StageDataModel] GetStageWaves: stageId 가 비어 있습니다.");
            return Array.Empty<StageWaveData>();
        }

        if (!_wavesByStageId.TryGetValue(stageId, out List<StageWaveData> waves))
        {
            return Array.Empty<StageWaveData>();
        }

        return waves;
    }

    // ===== 웨이브 목록 구성 =====

    private void BuildWaveTable()
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, StageWaveData> table))
        {
            Debug.LogError("[StageDataModel] StageWaveData 테이블을 가져오지 못했습니다.");
            return;
        }

        foreach (StageWaveData wave in table.Values)
        {
            if (null == wave)
            {
                continue;
            }

            if (string.IsNullOrEmpty(wave.StageId))
            {
                continue;
            }

            AddWave(wave);
        }

        SortWaves();
    }

    private void AddWave(StageWaveData wave)
    {
        if (!_wavesByStageId.TryGetValue(wave.StageId, out List<StageWaveData> waves))
        {
            waves = new List<StageWaveData>();
            _wavesByStageId.Add(wave.StageId, waves);
        }

        waves.Add(wave);
    }

    private void SortWaves()
    {
        foreach (List<StageWaveData> waves in _wavesByStageId.Values)
        {
            waves.Sort(CompareByWaveNumber);
        }
    }

    private static int CompareByWaveNumber(StageWaveData a, StageWaveData b)
    {
        return a.WaveNumber.CompareTo(b.WaveNumber);
    }
}
