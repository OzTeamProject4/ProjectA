using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/// <summary>
/// IGrowthDataProvider 테스트용 구현체.
/// DataManager / JSON 로드가 구현되기 전까지, 엑셀 스키마 값을 하드코딩으로 제공
/// </summary>
public class MockGrowthDataProvider : IGrowthDataProvider
{
    private readonly Dictionary<string, CharacterStatData> _statTable = new();
    private readonly Dictionary<int, CharacterGradeData> _gradeTable = new();
    private readonly Dictionary<int, int> _levelExpTable = new();
    private readonly Dictionary<string, ItemData> _itemTable = new();
    private readonly List<string> _characterIds = new();

    private bool _isInitialized;

    public UniTask InitializeAsync(CancellationToken token)
    {
        if (_isInitialized)
        {
            return UniTask.CompletedTask;
        }

        token.ThrowIfCancellationRequested();

        BuildStatTable();
        BuildGradeTable();
        BuildLevelExpTable();
        BuildItemTable();

        _isInitialized = true;

        Debug.Log($"초기화 완료. 캐릭터 {_statTable.Count}종 / 성급 {_gradeTable.Count}단계 / 레벨 {_levelExpTable.Count}개 / 아이템 {_itemTable.Count}종");
        return UniTask.CompletedTask;
    }

    public CharacterStatData GetStat(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            Debug.LogWarning("GetStat: characterId 가 비어 있습니다.");
            return null;
        }

        if (_statTable.TryGetValue(characterId, out var data))
        {
            return data;
        }

        Debug.LogWarning($"StatData 를 찾을 수 없습니다. CharacterId={characterId}");
        return null;
    }

    public CharacterGradeData GetGrade(int star)
    {
        if (_gradeTable.TryGetValue(star, out var data))
        {
            return data;
        }

        Debug.LogWarning($"GradeData 를 찾을 수 없습니다. Star={star}");
        return null;
    }

    public int GetRequiredExp(int level)
    {
        if (_levelExpTable.TryGetValue(level, out var requiredExp))
        {
            return requiredExp;
        }

        Debug.LogWarning($"LevelExp 를 찾을 수 없습니다. Level={level}");
        return 0;
    }

    public ItemData GetItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("GetItem: itemId 가 비어 있습니다.");
            return null;
        }

        if (_itemTable.TryGetValue(itemId, out var data))
        {
            return data;
        }

        Debug.LogWarning($"ItemData 를 찾을 수 없습니다. ItemId={itemId}");
        return null;
    }

    public IReadOnlyList<string> GetAllCharacterIds()
    {
        return _characterIds;
    }

    public IReadOnlyList<string> GetAllExpItemIds()
    {
        return _itemTable.Values
            .Where(item => item.Type == ItemType.ExpBook)
            .Select(item => item.ItemId)
            .ToList();
    }

    // ======================================================================
    // 하드코딩 데이터
    // ======================================================================

    private void BuildStatTable()
    {
        AddStat(new CharacterStatData
        {
            CharacterId = "Character_001",
            Hp = 100,
            Atk = 20,
            Def = 10,
            AtkSpeed = 1f,
            MoveSpeed = 3.5f,
            HpGrow = 18.6f,
            AtkGrow = 2.7f,
            DefGrow = 1.55f,
            AtkSpeedGrow = 0.01f,
            MoveSpeedGrow = 0.017f
        });

        AddStat(new CharacterStatData
        {
            CharacterId = "Character_002",
            Hp = 140,
            Atk = 15,
            Def = 16,
            AtkSpeed = 0.85f,
            MoveSpeed = 3.2f,
            HpGrow = 24f,
            AtkGrow = 1.9f,
            DefGrow = 2.4f,
            AtkSpeedGrow = 0.008f,
            MoveSpeedGrow = 0.012f
        });

        AddStat(new CharacterStatData
        {
            CharacterId = "Character_003",
            Hp = 80,
            Atk = 26,
            Def = 7,
            AtkSpeed = 1.25f,
            MoveSpeed = 4f,
            HpGrow = 14.4f,
            AtkGrow = 3.6f,
            DefGrow = 1.05f,
            AtkSpeedGrow = 0.013f,
            MoveSpeedGrow = 0.02f
        });

        AddStat(new CharacterStatData
        {
            CharacterId = "Character_004",
            Hp = 95,
            Atk = 17,
            Def = 9,
            AtkSpeed = 1.4f,
            MoveSpeed = 4.2f,
            HpGrow = 17.1f,
            AtkGrow = 2.2f,
            DefGrow = 1.35f,
            AtkSpeedGrow = 0.015f,
            MoveSpeedGrow = 0.022f
        });

        AddStat(new CharacterStatData
        {
            CharacterId = "Character_005",
            Hp = 160,
            Atk = 22,
            Def = 13,
            AtkSpeed = 0.8f,
            MoveSpeed = 3f,
            HpGrow = 27.2f,
            AtkGrow = 2.9f,
            DefGrow = 1.95f,
            AtkSpeedGrow = 0.007f,
            MoveSpeedGrow = 0.011f
        });
    }

    private void BuildGradeTable()
    {
        AddGrade(new CharacterGradeData
        {
            Star = 3,
            MaxLevel = 20,
            RequiredToNext = 5,
            HpGrow = 0f,
            AtkGrow = 0f,
            DefGrow = 0f,
            AtkSpeedGrow = 0f,
            MoveSpeedGrow = 0f
        });

        AddGrade(new CharacterGradeData
        {
            Star = 4,
            MaxLevel = 25,
            RequiredToNext = 10,
            HpGrow = 10f,
            AtkGrow = 5f,
            DefGrow = 2f,
            AtkSpeedGrow = 0.05f,
            MoveSpeedGrow = 0.02f
        });

        AddGrade(new CharacterGradeData
        {
            Star = 5,
            MaxLevel = 30,
            RequiredToNext = 0,
            HpGrow = 20f,
            AtkGrow = 10f,
            DefGrow = 4f,
            AtkSpeedGrow = 0.1f,
            MoveSpeedGrow = 0.04f
        });
    }

    private void BuildLevelExpTable()
    {
        var entries = new (int level, int requiredExp)[]
        {
                (1, 100),   (2, 150),   (3, 250),   (4, 300),   (5, 400),
                (6, 500),   (7, 650),   (8, 750),   (9, 900),   (10, 1050),
                (11, 1250), (12, 1400), (13, 1600), (14, 1800), (15, 2000),
                (16, 2250), (17, 2450), (18, 2700), (19, 3000), (20, 3250),
                (21, 3550), (22, 3850), (23, 4150), (24, 4500), (25, 4800),
                (26, 5150), (27, 5500), (28, 5900), (29, 6250), (30, 0)
        };

        foreach (var entry in entries)
        {
            _levelExpTable[entry.level] = entry.requiredExp;
        }
    }

    private void BuildItemTable()
    {
        AddItem(new ItemData { ItemId = "ExpBook_Small", Name = "경험치북(소)", Type = ItemType.ExpBook, Gold = 100, Value = 100 });
        AddItem(new ItemData { ItemId = "ExpBook_Medium", Name = "경험치북(중)", Type = ItemType.ExpBook, Gold = 200, Value = 250 });
        AddItem(new ItemData { ItemId = "ExpBook_Large", Name = "경험치북(대)", Type = ItemType.ExpBook, Gold = 400, Value = 500 });
    }

    private void AddStat(CharacterStatData stat)
    {
        _statTable[stat.CharacterId] = stat;
        _characterIds.Add(stat.CharacterId);
    }

    private void AddGrade(CharacterGradeData grade)
    {
        _gradeTable[grade.Star] = grade;
    }

    private void AddItem(ItemData item)
    {
        _itemTable[item.ItemId] = item;
    }
}