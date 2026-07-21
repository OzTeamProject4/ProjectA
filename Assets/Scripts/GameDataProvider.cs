using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameDataProvider : IGameDataProvider
{
    private Dictionary<int, CharacterGradeData> _gradeByStar;
    private Dictionary<int, LevelExpData> _expByLevel;
    private List<string> _expItemId;
    private List<EquipmentData> _allEquipment;
    private List<ItemData> _allItems;
    private List<StageData> _allStages;
    private Dictionary<string, List<StageWaveData>> _wavesByStageId;

    public CharacterData GetStat(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            Debug.LogWarning("[GrowthDataProvider:GetStat] characterId 가 비어 있습니다.");
            return null;
        }

        GameManager.Instance.DataManager.TryGetData(characterId, out CharacterData data);
        return data;
    }

    public CharacterGradeData GetGrade(int star)
    {
        if (null == _gradeByStar && !TryBuildGradeLookup())
        {
            return null;
        }

        if (!_gradeByStar.TryGetValue(star, out CharacterGradeData grade))
        {
            return null;
        }

        return grade;
    }

    public int GetRequiredExp(int level)
    {
        if (null == _expByLevel && !TryBuildExpLookup())
        {
            return 0;
        }

        if (!_expByLevel.TryGetValue(level, out LevelExpData exp))
        {
            return 0;
        }

        return exp.RequiredExp;
    }

    public ItemData GetItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[GrowthDataProvider:GetItem] itemId 가 비어 있습니다.");
            return null;
        }

        GameManager.Instance.DataManager.TryGetData(itemId, out ItemData data);
        return data;
    }

    public IReadOnlyList<string> GetAllCharacterIds()
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, CharacterData> table))
        {
            return Array.Empty<string>();
        }

        return table.Keys.ToList();
    }

    public IReadOnlyList<string> GetAllExpItemIds()
    {
        if (null != _expItemId)
        {
            return _expItemId;
        }

        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, ItemData> table))
        {
            return Array.Empty<string>();
        }

        _expItemId = new List<string>();
        foreach (ItemData item in table.Values)
        {
            if (item.Type == ItemType.ExpBook)
            {
                _expItemId.Add(item.DataId);
            }
        }

        return _expItemId;
    }

    public EquipmentData GetEquipment(string dataId)
    {
        if (string.IsNullOrEmpty(dataId))
        {
            Debug.LogWarning("[GameDataProvider:GetEquipment] dataId 가 비어 있습니다.");
            return null;
        }

        GameManager.Instance.DataManager.TryGetData(dataId, out EquipmentData data);
        return data;
    }

    public IReadOnlyList<EquipmentData> GetAllEquipment()
    {
        if (null != _allEquipment)
        {
            return _allEquipment;
        }

        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, EquipmentData> table))
        {
            return Array.Empty<EquipmentData>();
        }

        _allEquipment = new List<EquipmentData>(table.Values);
        return _allEquipment;
    }

    public IReadOnlyList<ItemData> GetAllItems()
    {
        if (null != _allItems)
        {
            return _allItems;
        }

        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, ItemData> table))
        {
            return Array.Empty<ItemData>();
        }

        _allItems = new List<ItemData>(table.Values);
        return _allItems;
    }

    public StageData GetStage(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning("[GameDataProvider:GetStage] stageId 가 비어 있습니다.");
            return null;
        }

        GameManager.Instance.DataManager.TryGetData(stageId, out StageData data);
        return data;
    }

    public IReadOnlyList<StageData> GetAllStages()
    {
        if (null != _allStages)
        {
            return _allStages;
        }

        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, StageData> table))
        {
            return Array.Empty<StageData>();
        }

        _allStages = new List<StageData>(table.Values);
        return _allStages;
    }

    public IReadOnlyList<StageWaveData> GetStageWaves(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning("[GameDataProvider:GetStageWaves] stageId 가 비어 있습니다.");
            return Array.Empty<StageWaveData>();
        }

        if (null == _wavesByStageId && !TryBuildWaveLookup())
        {
            return Array.Empty<StageWaveData>();
        }

        if (!_wavesByStageId.TryGetValue(stageId, out List<StageWaveData> waves))
        {
            return Array.Empty<StageWaveData>();
        }

        return waves;
    }

    private bool TryBuildGradeLookup()
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, CharacterGradeData> table))
        {
            return false;
        }

        _gradeByStar = new Dictionary<int, CharacterGradeData>();
        foreach (CharacterGradeData grade in table.Values)
        {
            _gradeByStar[grade.Star] = grade;
        }

        return true;
    }

    private bool TryBuildExpLookup()
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, LevelExpData> table))
        {
            return false;
        }

        _expByLevel = new Dictionary<int, LevelExpData>();
        foreach (LevelExpData exp in table.Values)
        {
            _expByLevel[exp.Level] = exp;
        }

        return true;
    }

    private bool TryBuildWaveLookup()
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, StageWaveData> table))
        {
            return false;
        }

        _wavesByStageId = new Dictionary<string, List<StageWaveData>>();
        foreach (StageWaveData wave in table.Values)
        {
            if (!_wavesByStageId.TryGetValue(wave.StageId, out List<StageWaveData> list))
            {
                list = new List<StageWaveData>();
                _wavesByStageId[wave.StageId] = list;
            }

            list.Add(wave);
        }

        foreach (List<StageWaveData> list in _wavesByStageId.Values)
        {
            list.Sort(CompareByWaveNumber);
        }

        return true;
    }

    private int CompareByWaveNumber(StageWaveData a, StageWaveData b)
    {
        return a.WaveNumber.CompareTo(b.WaveNumber);
    }
}