using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel
{
    private readonly IGrowthDataProvider _dataProvider;
    private readonly Dictionary<string, int> _expItemCounts = new();

    public string CharacterId { get; private set; }
    public int CurrentStar { get; private set; }
    public int CurrentLevel { get; private set; }
    public int CurrentExp { get; private set; }
    public int OwnedDuplicates { get; private set; }

    public bool IsMaxLevel
    {
        get
        {
            CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
            if (null == grade)
            {
                return false;
            }

            return CurrentLevel >= grade.MaxLevel;
        }
    }

    public bool IsMaxStar
    {
        get
        {
            CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
            if (null == grade)
            {
                return true;
            }

            return grade.RequiredToNext <= 0;
        }
    }

    public event Action OnExpChanged;
    public event Action OnLevelChanged;
    public event Action OnStarChanged;
    public event Action OnDuplicatesChanged;
    public event Action OnItemCountChanged;

    public CharacterModel(string characterId, int startStar, IGrowthDataProvider dataProvider)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            Debug.LogError("characterId 가 비어 있습니다.");
        }

        if (null == dataProvider)
        {
            Debug.LogError("IGrowthDataProvider 가 null 입니다.");
        }

        _dataProvider = dataProvider;
        CharacterId = characterId;
        CurrentStar = startStar;
        CurrentLevel = 1;
        CurrentExp = 0;
        OwnedDuplicates = 0;

        if (null != dataProvider && null == dataProvider.GetGrade(startStar))
        {
            Debug.LogWarning($"시작 성급({startStar})에 해당하는 CharacterGradeData 가 없습니다. CharacterId={characterId}");
        }
    }

    public FinalStats GetFinalStats()
    {
        CharacterStatData stat = _dataProvider.GetStat(CharacterId);
        CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);

        return StatCalculator.Calculate(stat, grade, CurrentLevel);
    }

    public int GetRequiredExpForNextLevel()
    {
        return _dataProvider.GetRequiredExp(CurrentLevel);
    }

    public int GetRequiredDuplicatesForPromotion()
    {
        CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
        return grade.RequiredToNext > 0 ? grade.RequiredToNext : 0;
    }

    public void AddExp(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"AddExp: 유효하지 않은 경험치({amount}).");
            return;
        }

        CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
        if (null == grade)
        {
            Debug.LogWarning($"AddExp: GradeData 없음. Star={CurrentStar}");
            return;
        }

        int maxLevel = grade.MaxLevel;
        if (CurrentLevel >= maxLevel)
        {
            Debug.Log($"이미 만렙(Lv{CurrentLevel}). 경험치 {amount} 무시. CharacterId={CharacterId}");
            return;
        }

        CurrentExp += amount;

        bool leveledUp = false;
        while (CurrentLevel < maxLevel)
        {
            int required = _dataProvider.GetRequiredExp(CurrentLevel);
            if (required <= 0)
            {
                break;
            }

            if (CurrentExp < required)
            {
                break;
            }

            CurrentExp -= required;
            CurrentLevel++;
            leveledUp = true;
        }

        if (CurrentLevel >= maxLevel)
        {
            CurrentExp = 0;
        }

        OnExpChanged?.Invoke();
        if (leveledUp)
        {
            OnLevelChanged?.Invoke();
        }
    }
    public void UseExpItem(string dataId)
    {
        if (string.IsNullOrEmpty(dataId))
        {
            Debug.LogWarning("UseExpItem: dataId 가 비어 있습니다.");
            return;
        }

        ItemData item = _dataProvider.GetItem(dataId);
        if (null == item)
        {
            Debug.LogWarning($"UseExpItem: ItemData 를 찾을 수 없습니다. dataId={dataId}");
            return;
        }

        UseExpItem(dataId, item.Value);
    }

    public void UseExpItem(string dataId, int value)
    {
        if (string.IsNullOrEmpty(dataId))
        {
            Debug.LogWarning("dataId 가 비어 있습니다.");
            return;
        }

        if (GetItemCount(dataId) <= 0)
        {
            Debug.LogWarning($"보유 수량 없음. dataId={dataId}");
            return;
        }

        if (IsMaxLevel)
        {
            Debug.Log($"만렙이라 경험치 아이템 사용 불가. dataId={dataId}");
            return;
        }

        _expItemCounts[dataId] = GetItemCount(dataId) - 1;
        OnItemCountChanged?.Invoke();

        AddExp(value);
    }

    public bool CanPromote()
    {
        CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
        if (null == grade || grade.RequiredToNext <= 0)
        {
            return false;
        }

        if (CurrentLevel < grade.MaxLevel)
        {
            return false;
        }

        return OwnedDuplicates >= grade.RequiredToNext;
    }

    public void Promote()
    {
        CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
        if (null == grade)
        {
            Debug.LogWarning($"CharacterGradeData 없음. Star={CurrentStar}");
            return;
        }

        if (grade.RequiredToNext <= 0)
        {
            Debug.LogWarning($"이미 최대 성급({CurrentStar}) 입니다.");
            return;
        }

        if (CurrentLevel < grade.MaxLevel)
        {
            Debug.LogWarning($"만렙(Lv{grade.MaxLevel}) 도달 후 승급 가능. 현재 Lv{CurrentLevel}");
            return;
        }

        if (OwnedDuplicates < grade.RequiredToNext)
        {
            Debug.LogWarning($"캐릭터 보유 수량 부족. 필요={grade.RequiredToNext}, 보유={OwnedDuplicates}");
            return;
        }

        OwnedDuplicates -= grade.RequiredToNext;
        CurrentStar++;

        OnDuplicatesChanged?.Invoke();
        OnStarChanged?.Invoke();
    }

    public void AddExpItem(string dataId, int count)
    {
        if (string.IsNullOrEmpty(dataId) || count <= 0)
        {
            Debug.LogWarning($"유효하지 않은 입력. dataId={dataId}, Count={count}");
            return;
        }

        _expItemCounts[dataId] = GetItemCount(dataId) + count;
        OnItemCountChanged?.Invoke();
    }

    public void AddDuplicate(int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning($"유효하지 않은 수량({count}).");
            return;
        }

        OwnedDuplicates += count;
        OnDuplicatesChanged?.Invoke();
    }

    public bool CanUseExpItem(string dataId)
    {
        if (IsMaxLevel)
        {
            return false;
        }

        return GetItemCount(dataId) > 0;
    }

    public IReadOnlyList<ItemData> GetExpItems()
    {
        List<ItemData> items = new();

        foreach (string dataId in _dataProvider.GetAllExpItemIds())
        {
            ItemData item = _dataProvider.GetItem(dataId);
            if (null != item)
            {
                items.Add(item);
            }
        }

        return items;
    }

    public int GetItemCount(string dataId)
    {
        if (string.IsNullOrEmpty(dataId))
        {
            return 0;
        }

        return _expItemCounts.TryGetValue(dataId, out int count) ? count : 0;
    }
}