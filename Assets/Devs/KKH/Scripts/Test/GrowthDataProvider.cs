using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public static class GrowthDataKeys
{
    public const string CharacterStat = "Data/CharacterStat";
    public const string CharacterGrade = "Data/CharacterGrade";
    public const string LevelExp = "Data/LevelExp";
    public const string Item = "Data/Item";
}

public class GrowthDataProvider : IGrowthDataProvider
{
    private bool _isInitialized;

    public UniTask InitializeAsync(CancellationToken token)
    {
        if (_isInitialized)
        {
            return UniTask.CompletedTask;
        }

        token.ThrowIfCancellationRequested();

        bool hasStat = GameManager.Instance.DataManager.TryGetDataTable<CharacterStatData>(out _);
        bool hasGrade = GameManager.Instance.DataManager.TryGetDataTable<CharacterGradeData>(out _);
        bool hasLevelExp = GameManager.Instance.DataManager.TryGetDataTable<LevelExpData>(out _);
        bool hasItem = GameManager.Instance.DataManager.TryGetDataTable<ItemData>(out _);

        if (!hasStat || !hasGrade || !hasLevelExp || !hasItem)
        {
            Debug.LogError("[GrowthDataProvider:InitializeAsync] 성장 데이터 테이블이 아직 로드되지 않았습니다.");
            return UniTask.CompletedTask;
        }

        _isInitialized = true;
        return UniTask.CompletedTask;
    }

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
        string dataId = $"Star_{star}";

        GameManager.Instance.DataManager.TryGetData(dataId, out CharacterGradeData data);
        return data;
    }

    public int GetRequiredExp(int level)
    {
        string dataId = $"Level_{level}";

        if (!GameManager.Instance.DataManager.TryGetData(dataId, out LevelExpData data))
        {
            return 0;
        }

        return data.RequiredExp;
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

        return table.Values
            .Where(item => item.Type == ItemType.ExpBook)
            .Select(item => item.DataId)
            .ToList();
    }
}