using System;
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
}
public class StudentGradeData : BaseData
{
    public int Star { get; init; } //TODO 사용처 없음 확인바람
    public int MaxLevel { get; init; }
    public string RequiredGradeUpItemId { get; init; }
    public int RequiredGradeUpItemCount { get; init; }
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

    }
}

public class StudentModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs NameChanged = new PropertyChangedEventArgs(nameof(Name));
    private static readonly PropertyChangedEventArgs StarChanged = new PropertyChangedEventArgs(nameof(Star));
    private static readonly PropertyChangedEventArgs TotalExperienceChanged = new PropertyChangedEventArgs(nameof(TotalExperience));
    private static readonly PropertyChangedEventArgs LevelChanged = new PropertyChangedEventArgs(nameof(Level));
    private static readonly PropertyChangedEventArgs IsMaxLevelChanged = new PropertyChangedEventArgs(nameof(IsMaxLevel));
    private static readonly PropertyChangedEventArgs HpChanged = new PropertyChangedEventArgs(nameof(TotalHp));
    private static readonly PropertyChangedEventArgs AttackChanged = new PropertyChangedEventArgs(nameof(TotalAttack));
    private static readonly PropertyChangedEventArgs DefenseChanged = new PropertyChangedEventArgs(nameof(TotalDefense));
    private static readonly PropertyChangedEventArgs MoveSpeedChanged = new PropertyChangedEventArgs(nameof(TotalMoveSpeed));
    private static readonly PropertyChangedEventArgs FullBodyKeyChanged = new PropertyChangedEventArgs(nameof(FullBodyKey));
    private static readonly PropertyChangedEventArgs PortraitKeyChanged = new PropertyChangedEventArgs(nameof(PortraitKey));


    //private static readonly PropertyChangedEventArgs EquipChanged = new PropertyChangedEventArgs(nameof(EquipItem));

    private string _dataId;
    private string _name;
    private int _star;
    private ElementType _elementType;
    private int _totalExperience;
    private int _level;
    private string _fullBodyKey;
    private string _portraitKey;

    private StudentGradeData _currentGradeData;
    private StudentLevelData _currentLevelData;

    private readonly Dictionary<EquipType, string> _equippedItemIds;
    private readonly Dictionary<StatType, float> _baseStats;
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

                //TODO 승급시 캐릭터 스텟 반영 메서드 호출
                TryUpdateCurrentGradeData();

                OnPropertyChanged(StarChanged);
            }
        }
    }

    public bool IsMaxStar
    {
        get
        {
            return _currentGradeData.RequiredGradeUpItemCount <= 0;
        }
    }

    public string RequiredGradeUpItemId
    {
        get
        {
            return _currentGradeData.RequiredGradeUpItemId;
        }
    }

    public int RequiredGradeUpItemCount
    {
        get
        {
            return _currentGradeData.RequiredGradeUpItemCount;
        }
    }

    public ElementType ElementType
    {
        get { return _elementType; }
    }

    public int TotalExperience
    {
        get { return _totalExperience; }
        private set
        {
            if (_totalExperience == value)
            {
                return;
            }

            _totalExperience = value;
            OnPropertyChanged(TotalExperienceChanged);
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
                //TODO레벨업 시 스탯 반영 메서드 추가

                _level = value;
                TryUpdateCurrentLevelData();
                OnPropertyChanged(LevelChanged);
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
        get { return _baseStats[StatType.Hp] + _equipmentStats[StatType.Hp]; }
    }

    public float TotalAttack
    {
        get { return _baseStats[StatType.Attack] + _equipmentStats[StatType.Attack]; }
    }

    public float TotalDefense
    {
        get { return _baseStats[StatType.Defense] + _equipmentStats[StatType.Defense]; }
    }

    public float TotalMoveSpeed
    {
        get { return _baseStats[StatType.MoveSpeed] + _equipmentStats[StatType.MoveSpeed]; }
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


    public event PropertyChangedEventHandler PropertyChanged;

    public StudentModel(StudentData studentData)
    {
        _dataId = studentData.DataId;
        _name = studentData.Name;
        _star = studentData.Star;
        _elementType = studentData.ElementType;
        _fullBodyKey = studentData.FullBodyKey;
        _portraitKey = studentData.PortraitKey;
        _totalExperience = 0;
        _level = CalculateLevel();

        TryUpdateCurrentGradeData();
        TryUpdateCurrentLevelData();

        _equippedItemIds = new Dictionary<EquipType, string>();
        _baseStats = CreateStatDictionary(new StatData(studentData.BaseHp, studentData.BaseAttack, studentData.BaseDefense, studentData.BaseMoveSpeed));
        _equipmentStats = GetEquipmentStats();
    }

    public void NotifyAllProperties()
    {
        OnPropertyChanged(NameChanged);
        OnPropertyChanged(StarChanged);
        OnPropertyChanged(TotalExperienceChanged);
        OnPropertyChanged(LevelChanged);
        OnPropertyChanged(IsMaxLevelChanged);
        OnPropertyChanged(HpChanged);
        OnPropertyChanged(AttackChanged);
        OnPropertyChanged(DefenseChanged);
        OnPropertyChanged(MoveSpeedChanged);
        OnPropertyChanged(FullBodyKeyChanged);
        OnPropertyChanged(PortraitKeyChanged);
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

    private int CalculateLevel()
    {
        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, StudentLevelData> levelTable))
        {
            Debug.LogError("레벨 경험치 데이터를 찾을 수 없습니다.");
            return -1;
        }

        int maxLevel = 1;

        foreach (StudentLevelData levelData in levelTable.Values)
        {
            if (_totalExperience < levelData.RequiredExp)
            {
                return levelData.Level;
            }

            maxLevel = Math.Max(maxLevel, levelData.Level);
        }

        return maxLevel;
    }

    private Dictionary<StatType, float> CreateStatDictionary(StatData statData)
    {
        Dictionary<StatType, float> statDictionary = new Dictionary<StatType, float>();

        statDictionary[StatType.Hp] = statData.Hp;
        statDictionary[StatType.Attack] = statData.Attack;
        statDictionary[StatType.Defense] = statData.Defense;
        statDictionary[StatType.MoveSpeed] = statData.MoveSpeed;

        return statDictionary;
    }

    private Dictionary<StatType, float> GetEquipmentStats()
    {
        StatData equipmentStats = new StatData();

        foreach (string itemId in _equippedItemIds.Values)
        {
            if (!GameManager.Instance.DataManager.TryGetData(itemId, out EquipmentData equipmentData))
            {
                Debug.LogWarning($"{itemId} 장비 데이터가 없습니다.");
                continue;
            }

            StatData itemStats = new StatData(equipmentData.Hp, equipmentData.Attack, equipmentData.Defense, equipmentData.MoveSpeed);
            equipmentStats.AddStat(itemStats);
        }

        Dictionary<StatType, float> equipmentStatsDictionary = CreateStatDictionary(equipmentStats);
        return equipmentStatsDictionary;
    }

    //TODO 레벨업 로직 수정
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

        TotalExperience += amount;

        while (TotalExperience >= _currentLevelData.RequiredExp)
        {
            Level++;
        }

        return true;
    }

    //TODO 승급 로직 수정
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

        //TODO 아이템 가져오기
        if(!NetworkManagerTemp.Instance.InventoryModel.TryGetItem(_currentGradeData.RequiredGradeUpItemId, out ItemModel itemModel))
        {
            return false;
        }

        if (itemModel is not MaterialModel materialModel)
        {
            return false;
        }

        if (materialModel.Count < _currentGradeData.RequiredGradeUpItemCount)
        {
            return false;
        }

        //TODO 아이템 소비
        //materialModel.Count -= _currentGradeData.RequiredGradeUpItemCount;

        Star++;

        return true;
    }

    //private List<ItemModel> equipItem = new List<ItemModel>();

    //public IReadOnlyList<ItemModel> EquipItem
    //{
    //    get { return equipItem; }
    //}

    //public void Equip(ItemModel itemModel)
    //{
    //    //TODO 여러 검사
    //    if (itemModel.EquipId != null)
    //    {
    //        CharacterListModel characterListModel = NetworkManagerTemp.Instance.GetcharacterListModel();
    //        CharacterModel character = characterListModel.GetCharacter(itemModel.EquipId);

    //        if (character == null)
    //        {
    //            Debug.LogError("");
    //            return;
    //        }

    //        character.Unequip(itemModel);
    //    }

    //    itemModel.Equip(Id);
    //    equipItem.Add(itemModel);
    //    CalculateItemStat(itemModel.StatInfo, true);
    //    OnPropertyChanged(EquipChanged);
    //}

    //public void Unequip(ItemModel itemModel)
    //{
    //    //TODO 여러검사

    //    itemModel.UnEquip();
    //    equipItem.Remove(itemModel);
    //    CalculateItemStat(itemModel.StatInfo, false);
    //    OnPropertyChanged(EquipChanged);
    //}

    private void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, propertyChangedEventArgs);
    }

    //public void CalculateItemStat(IReadOnlyList<StatInfo> statInfos, bool isAdd)
    //{
    //    float value = isAdd ? 1f : -1f;

    //    foreach (StatInfo statInfo in statInfos)
    //    {
    //        float delta = statInfo.Value * value;

    //        switch (statInfo.Type)
    //        {
    //            case StatType.MaxHp:
    //                _addHp += delta;
    //                OnPropertyChanged(HpChanged);
    //                break;
    //            case StatType.Atk:
    //                _addAttack += delta;
    //                OnPropertyChanged(AttackChanged);
    //                break;
    //            case StatType.Def:
    //                _addDefense += delta;
    //                OnPropertyChanged(DefenseChanged);
    //                break;
    //            case StatType.MoveSpeed:
    //                _addMoveSpeed += delta;
    //                OnPropertyChanged(MoveSpeedChanged);
    //                break;
    //        }
    //    }
    //}
}