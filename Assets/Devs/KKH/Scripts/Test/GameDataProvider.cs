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

    public CharacterStatData GetStat(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            Debug.LogWarning("[GrowthDataProvider:GetStat] characterId 가 비어 있습니다.");
            return null;
        }

        GameManager.Instance.DataManager.TryGetData(characterId, out CharacterStatData data);
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
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, CharacterStatData> table))
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
}