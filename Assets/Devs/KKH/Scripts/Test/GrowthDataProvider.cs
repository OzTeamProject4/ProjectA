using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GrowthDataProvider : IGrowthDataProvider
{
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
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, CharacterGradeData> table))
        {
            return null;
        }

        foreach (CharacterGradeData grade in table.Values)
        {
            if (grade.Star == star)
            {
                return grade;
            }
        }

        return null;
    }

    public int GetRequiredExp(int level)
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, LevelExpData> table))
        {
            return 0;
        }

        foreach (LevelExpData exp in table.Values)
        {
            if (exp.Level == level)
            {
                return exp.RequiredExp;
            }
        }

        return 0;
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
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, ItemData> table))
        {
            return Array.Empty<string>();
        }

        List<string> expItemIds = new();
        foreach (ItemData item in table.Values)
        {
            if (item.Type == ItemType.ExpBook)
            {
                expItemIds.Add(item.DataId);
            }
        }

        return expItemIds;
    }
}