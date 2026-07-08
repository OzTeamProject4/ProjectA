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

    /// <summary>
    /// 경험치 추가 메서드. 필요 경험치를 넘기면 레벨업하며 초과분은 이월.
    /// 만렙(현재 성급 상한) 도달 시 추가 경험치와 초과분은 삭제.
    /// </summary>
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

    /// <summary>
    /// 보유한 경험치 아이템 하나를 사용해 grantExp 만큼 경험치를 가산.
    /// 만렙에서는 아이템을 소비하지 않고 차단.
    /// </summary>
    public void UseExpItem(string itemId, int value)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("itemId 가 비어 있습니다.");
            return;
        }

        if (GetItemCount(itemId) <= 0)
        {
            Debug.LogWarning($"보유 수량 없음. ItemId={itemId}");
            return;
        }

        if (IsMaxLevel)
        {
            Debug.Log($"만렙이라 경험치 아이템 사용 불가. ItemId={itemId}");
            return;
        }

        _expItemCounts[itemId] = GetItemCount(itemId) - 1;
        OnItemCountChanged?.Invoke();

        AddExp(value);
    }

    /// <summary>
    /// 승급 가능 여부를 검사.
    /// 최대 성급이 아니고, 현재 성급 만렙에 도달했으며, 중복본이 충분한지.
    /// </summary>
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

    /// <summary>
    /// 성급을 1단계 올리기(승성)
    /// 보유한 캐릭터 중복본을 소비. 승급 후 만렙 상한이 상승.
    /// </summary>
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

    /// <summary> 경험치 아이템 획득(테스트/실제 지급 공용). </summary>
    public void AddExpItem(string itemId, int count)
    {
        if (string.IsNullOrEmpty(itemId) || count <= 0)
        {
            Debug.LogWarning($"유효하지 않은 입력. ItemId={itemId}, Count={count}");
            return;
        }

        _expItemCounts[itemId] = GetItemCount(itemId) + count;
        OnItemCountChanged?.Invoke();
    }

    /// <summary> 중복본 획득(테스트/실제 지급 공용). </summary>
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

    public int GetItemCount(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return 0;
        }

        return _expItemCounts.TryGetValue(itemId, out int count) ? count : 0;
    }
}