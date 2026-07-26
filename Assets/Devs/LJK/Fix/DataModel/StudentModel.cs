using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public static class StudentDataId
{
    public static string CreateGradeId(int grade)
    {
        return $"Star_{grade}";
    }

    public static string CreateLevelId(int level)
    {
        return $"Level_{level}";
    }

    public static string CreateShardItemId(string studentDataId)
    {
        int separatorIndex = studentDataId.LastIndexOf('_');

        if (separatorIndex < 0 || separatorIndex == studentDataId.Length - 1)
        {
            Debug.LogError($"학생 DataId 형식이 예상과 다릅니다. DataId={studentDataId}");
            return string.Empty;
        }

        string serial = studentDataId.Substring(separatorIndex + 1);

        return $"Item_Mat_Shard_{serial}";
    }
}
public class StudentGradeData : BaseData
{
    public int Star { get; init; } //TODO 사용처 없음 확인바람
    public int MaxLevel { get; init; }
    public int RequiredToNext { get; init; }
    public float HpGrow { get; init; }
    public float AtkGrow { get; init; }
    public float DefGrow { get; init; }
    public float MoveSpeedGrow { get; init; }
}

public class StudentLevelData : BaseData
{
    public int Level { get; init; } //TODO 사용처 없음 확인바람
    public int RequiredExp { get; init; }
}

public enum StatType
{
    Hp,
    Attack,
    Defense,
    MoveSpeed
}


public struct StatData
{
    public float Hp { get; private set; }
    public float Attack { get; private set; }
    public float Defense { get; private set; }
    public float MoveSpeed { get; private set; }

    public StatData(float hp, float attack, float defense, float moveSpeed)
    {
        Hp = hp;
        Attack = attack;
        Defense = defense;
        MoveSpeed = moveSpeed;
    }

    public void AddStat(StatData statData)
    {
        Hp += statData.Hp;
        Attack += statData.Attack;
        Defense += statData.Defense;
        MoveSpeed += statData.MoveSpeed;
    }
}

public class StudentModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs NameChanged = new PropertyChangedEventArgs(nameof(Name));
    private static readonly PropertyChangedEventArgs StarChanged = new PropertyChangedEventArgs(nameof(Star));
    private static readonly PropertyChangedEventArgs ElementTypeChanged = new PropertyChangedEventArgs(nameof(ElementType));
    private static readonly PropertyChangedEventArgs CurrentExperienceChanged = new PropertyChangedEventArgs(nameof(CurrentExperience));
    private static readonly PropertyChangedEventArgs LevelChanged = new PropertyChangedEventArgs(nameof(Level));
    private static readonly PropertyChangedEventArgs IsMaxLevelChanged = new PropertyChangedEventArgs(nameof(IsMaxLevel));
    private static readonly PropertyChangedEventArgs HpChanged = new PropertyChangedEventArgs(nameof(TotalHp));
    private static readonly PropertyChangedEventArgs AttackChanged = new PropertyChangedEventArgs(nameof(TotalAttack));
    private static readonly PropertyChangedEventArgs DefenseChanged = new PropertyChangedEventArgs(nameof(TotalDefense));
    private static readonly PropertyChangedEventArgs MoveSpeedChanged = new PropertyChangedEventArgs(nameof(TotalMoveSpeed));
    private static readonly PropertyChangedEventArgs FullBodyKeyChanged = new PropertyChangedEventArgs(nameof(FullBodyKey));
    private static readonly PropertyChangedEventArgs PortraitKeyChanged = new PropertyChangedEventArgs(nameof(PortraitKey));
    private static readonly PropertyChangedEventArgs EquippedItemIdsChanged = new PropertyChangedEventArgs(nameof(EquippedItemIds));
    private static readonly PropertyChangedEventArgs RequiredGradeUpItemIdChanged = new PropertyChangedEventArgs(nameof(RequiredGradeUpItemId));


    private string _dataId;
    private string _name;
    private int _star;
    private ElementType _elementType;
    private int _currentExperience;
    private int _level;
    private string _fullBodyKey;
    private string _portraitKey;

    private StudentGradeData _currentGradeData;
    private StudentLevelData _currentLevelData;

    private readonly Dictionary<EquipType, string> _equippedItemIds;
    private readonly StatData _growthPerLevel;
    private readonly Dictionary<StatType, float> _baseStats;
    private readonly Dictionary<StatType, float> _levelStats;
    private readonly Dictionary<StatType, float> _gradeStats;
    private readonly Dictionary<StatType, float> _equipmentStats;

    public string DataId
    {
        get { return _dataId; }
    }

    public string Name
    {
        get { return _name; }
    }

    public int Star
    {
        get { return _star; }
        private set
        {
            if (IsMaxStar)
            {
                return;
            }

            if (_star != value)
            {
                _star = value;

                TryUpdateCurrentGradeData();

                OnPropertyChanged(StarChanged);

                RecalculateStats();
                OnPropertyChanged(IsMaxLevelChanged);
            }
        }
    }

    public bool IsMaxStar
    {
        get
        {
            return _currentGradeData.RequiredToNext <= 0;
        }
    }

    public string RequiredGradeUpItemId
    {
        get
        {
            return StudentDataId.CreateShardItemId(_dataId);
        }
    }

    public int RequiredGradeUpItemCount
    {
        get
        {
            return _currentGradeData.RequiredToNext;
        }
    }

    public ElementType ElementType
    {
        get { return _elementType; }
    }

    public int CurrentExperience
    {
        get { return _currentExperience; }
        private set
        {
            if (_currentExperience == value)
            {
                return;
            }

            _currentExperience = value;
            OnPropertyChanged(CurrentExperienceChanged);
        }
    }

    public int RequiredExp
    {
        get
        {
            return _currentLevelData.RequiredExp;
        }
    }

    public int Level
    {
        get { return _level; }
        private set
        {
            if (_level != value)
            {
                _level = value;
                TryUpdateCurrentLevelData();
                OnPropertyChanged(LevelChanged);
                OnPropertyChanged(IsMaxLevelChanged);

                RecalculateStats();
            }
        }
    }

    public bool IsMaxLevel
    {
        get
        {
            return _level >= _currentGradeData.MaxLevel;
        }
    }

    public float TotalHp
    {
        get { return _baseStats[StatType.Hp] + _levelStats[StatType.Hp] + _gradeStats[StatType.Hp] + _equipmentStats[StatType.Hp]; }
    }

    public float TotalAttack
    {
        get { return _baseStats[StatType.Attack] + _levelStats[StatType.Attack] + _gradeStats[StatType.Attack] + _equipmentStats[StatType.Attack]; }
    }

    public float TotalDefense
    {
        get { return _baseStats[StatType.Defense] + _levelStats[StatType.Defense] + _gradeStats[StatType.Defense] + _equipmentStats[StatType.Defense]; }
    }

    public float TotalMoveSpeed
    {
        get { return _baseStats[StatType.MoveSpeed] + _levelStats[StatType.MoveSpeed] + _gradeStats[StatType.MoveSpeed] + _equipmentStats[StatType.MoveSpeed]; }
    }

    public string FullBodyKey
    {
        get
        {
            return _fullBodyKey;
        }
    }

    public string PortraitKey
    {
        get
        {
            return _portraitKey;
        }
    }

    public IReadOnlyDictionary<EquipType, string> EquippedItemIds
    {
        get
        {
            return _equippedItemIds;
        }
    }

    public bool TryGetEquippedItemId(EquipType equipType, out string instanceId)
    {
        return _equippedItemIds.TryGetValue(equipType, out instanceId);
    }

    public void Equip(EquipType equipType, string instanceId)
    {
        _equippedItemIds[equipType] = instanceId;

        RecalculateStats();
        OnPropertyChanged(EquippedItemIdsChanged);
    }

    public void Unequip(EquipType equipType)
    {
        if (!_equippedItemIds.Remove(equipType))
        {
            return;
        }

        RecalculateStats();
        OnPropertyChanged(EquippedItemIdsChanged);
    }


    public event PropertyChangedEventHandler PropertyChanged;

    public StudentModel(StudentData studentData)
    {
        _dataId = studentData.DataId;
        _name = studentData.Name;
        _star = studentData.Star;
        _elementType = studentData.Type;
        _portraitKey = studentData.CharacterIconPath;
        _fullBodyKey = CreateFullBodyKey(studentData.CharacterIconPath);
        _currentExperience = 0;
        _level = 1;

        TryUpdateCurrentGradeData();
        TryUpdateCurrentLevelData();

        _equippedItemIds = new Dictionary<EquipType, string>();

        _growthPerLevel = new StatData(studentData.HpGrow, studentData.AtkGrow, studentData.DefGrow, studentData.MoveSpeedGrow);

        _baseStats = new Dictionary<StatType, float>();
        _levelStats = new Dictionary<StatType, float>();
        _gradeStats = new Dictionary<StatType, float>();
        _equipmentStats = new Dictionary<StatType, float>();

        FillStatDictionary(_baseStats, new StatData(studentData.MaxHp, studentData.Attack, studentData.Defence, studentData.MoveSpeed));
        RecalculateStats();
    }

    //TODO StudentData에 전신 이미지 컬럼이 없어 초상화 키에서 파생시킨다. 컬럼이 생기면 그 값을 쓸 것
    // Icon/Lumi → StandImage/Lumi
    private static string CreateFullBodyKey(string portraitKey)
    {
        if (string.IsNullOrWhiteSpace(portraitKey))
        {
            return string.Empty;
        }

        int separatorIndex = portraitKey.LastIndexOf('/');

        if (separatorIndex < 0 || separatorIndex == portraitKey.Length - 1)
        {
            return string.Empty;
        }

        string characterName = portraitKey.Substring(separatorIndex + 1);

        return $"StandImage/{characterName}";
    }

    public void NotifyAllProperties()
    {
        OnPropertyChanged(NameChanged);
        OnPropertyChanged(StarChanged);
        OnPropertyChanged(ElementTypeChanged);
        OnPropertyChanged(CurrentExperienceChanged);
        OnPropertyChanged(LevelChanged);
        OnPropertyChanged(IsMaxLevelChanged);
        OnPropertyChanged(HpChanged);
        OnPropertyChanged(AttackChanged);
        OnPropertyChanged(DefenseChanged);
        OnPropertyChanged(MoveSpeedChanged);
        OnPropertyChanged(FullBodyKeyChanged);
        OnPropertyChanged(PortraitKeyChanged);
        OnPropertyChanged(RequiredGradeUpItemIdChanged);
    }

    private bool TryUpdateCurrentGradeData()
    {
        string gradeId = StudentDataId.CreateGradeId(_star);

        if (!GameManager.Instance.DataManager.TryGetData(gradeId, out _currentGradeData))
        {
            Debug.LogError($"{gradeId} StudentGradeData를 찾을 수 없습니다.");
            return false;
        }

        return true;
    }

    private bool TryUpdateCurrentLevelData()
    {
        string levelId = StudentDataId.CreateLevelId(_level);

        if (!GameManager.Instance.DataManager.TryGetData(levelId, out _currentLevelData))
        {
            Debug.LogError($"{levelId} StudentLevelData를 찾을 수 없습니다.");
            return false;
        }

        return true;
    }

    private static void FillStatDictionary(Dictionary<StatType, float> statDictionary, StatData statData)
    {
        statDictionary[StatType.Hp] = statData.Hp;
        statDictionary[StatType.Attack] = statData.Attack;
        statDictionary[StatType.Defense] = statData.Defense;
        statDictionary[StatType.MoveSpeed] = statData.MoveSpeed;
    }

    private void RecalculateStats()
    {
        UpdateLevelStats();
        UpdateGradeStats();
        UpdateEquipmentStats();
        NotifyStatsChanged();
    }

    private void UpdateLevelStats()
    {
        int growthCount = _level - 1;

        if (growthCount < 0)
        {
            growthCount = 0;
        }

        StatData levelStats = new StatData(
            _growthPerLevel.Hp * growthCount,
            _growthPerLevel.Attack * growthCount,
            _growthPerLevel.Defense * growthCount,
            _growthPerLevel.MoveSpeed * growthCount);

        FillStatDictionary(_levelStats, levelStats);
    }

    private void UpdateGradeStats()
    {
        StatData gradeStats = new StatData(_currentGradeData.HpGrow, _currentGradeData.AtkGrow, _currentGradeData.DefGrow, _currentGradeData.MoveSpeedGrow);

        FillStatDictionary(_gradeStats, gradeStats);
    }

    private void UpdateEquipmentStats()
    {
        StatData equipmentStats = new StatData();
        InventoryModel inventoryModel = NetworkManagerTemp.Instance.InventoryModel;

        // 담긴 값은 InstanceId다. 마스터 데이터를 다시 조회하지 않고 인스턴스가 들고 있는 StatInfos를 합산한다
        foreach (string instanceId in _equippedItemIds.Values)
        {
            if (!inventoryModel.TryGetEquipment(instanceId, out EquipmentModel equipmentModel))
            {
                Debug.LogWarning($"{instanceId} 장비를 인벤토리에서 찾을 수 없습니다.");
                continue;
            }

            equipmentStats.AddStat(CreateStatData(equipmentModel.StatInfos));
        }

        FillStatDictionary(_equipmentStats, equipmentStats);
    }

    private static StatData CreateStatData(IReadOnlyList<StatInfo> statInfos)
    {
        float hp = 0f;
        float attack = 0f;
        float defense = 0f;
        float moveSpeed = 0f;

        if (statInfos == null)
        {
            return new StatData(hp, attack, defense, moveSpeed);
        }

        foreach (StatInfo statInfo in statInfos)
        {
            switch (statInfo.Type)
            {
                case StatType.Hp:
                    hp += statInfo.Value;
                    break;
                case StatType.Attack:
                    attack += statInfo.Value;
                    break;
                case StatType.Defense:
                    defense += statInfo.Value;
                    break;
                case StatType.MoveSpeed:
                    moveSpeed += statInfo.Value;
                    break;
            }
        }

        return new StatData(hp, attack, defense, moveSpeed);
    }

    private void NotifyStatsChanged()
    {
        OnPropertyChanged(HpChanged);
        OnPropertyChanged(AttackChanged);
        OnPropertyChanged(DefenseChanged);
        OnPropertyChanged(MoveSpeedChanged);
    }

    public bool TryAddExp(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError($"AddExp: 유효하지 않은 경험치({amount}).");
            return false;
        }

        if (IsMaxLevel)
        {
            return false;
        }

        //TODO 여러 레벨이 한 번에 오르면 Level 세터가 매번 RecalculateStats를 불러 스탯 통지가 레벨 수만큼 나간다.
        //     루프가 끝난 뒤 한 번만 재계산/통지하도록 묶는 것을 고려.
        int gainedExperience = _currentExperience + amount;

        while (!IsMaxLevel)
        {
            int requiredExperience = _currentLevelData.RequiredExp;

            if (requiredExperience <= 0)
            {
                break;
            }

            if (gainedExperience < requiredExperience)
            {
                break;
            }

            gainedExperience -= requiredExperience;
            Level++;
        }

        if (IsMaxLevel && gainedExperience > _currentLevelData.RequiredExp)
        {
            gainedExperience = _currentLevelData.RequiredExp;
        }

        CurrentExperience = gainedExperience;

        return true;
    }

    public bool TryGradeUp()
    {
        if (IsMaxStar)
        {
            return false;
        }

        if (!IsMaxLevel)
        {
            return false;
        }

        if (!NetworkManagerTemp.Instance.InventoryModel.TryGetMaterial(RequiredGradeUpItemId, out MaterialModel materialModel))
        {
            return false;
        }

        if (!materialModel.TryConsume(RequiredGradeUpItemCount))
        {
            return false;
        }

        Star++;

        return true;
    }

    private void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, propertyChangedEventArgs);
    }

}