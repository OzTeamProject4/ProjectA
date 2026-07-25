using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public static class CharId
{
    public static string GetCharGradeId(int grade)
    {
        string gradeId = $"Star_{grade}";
        return gradeId;
    }

    public static string GetCharLevelId(int level)
    {
        string levelId = $"Level_{level}";
        return levelId;
    }
}

public class CharacterModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs IdChanged = new PropertyChangedEventArgs(nameof(Id));
    private static readonly PropertyChangedEventArgs NameChanged = new PropertyChangedEventArgs(nameof(Name));
    private static readonly PropertyChangedEventArgs StarChanged = new PropertyChangedEventArgs(nameof(Star));
    private static readonly PropertyChangedEventArgs ExpChanged = new PropertyChangedEventArgs(nameof(Exp));
    private static readonly PropertyChangedEventArgs LevelChanged = new PropertyChangedEventArgs(nameof(Level));
    private static readonly PropertyChangedEventArgs IsMaxLevelChanged = new PropertyChangedEventArgs(nameof(IsMaxLevel));

    //private static readonly PropertyChangedEventArgs HpChanged = new PropertyChangedEventArgs(nameof(Hp));
    //private static readonly PropertyChangedEventArgs AttakChanged = new PropertyChangedEventArgs(nameof(Attack));
    //private static readonly PropertyChangedEventArgs DefenseChanged = new PropertyChangedEventArgs(nameof(Defense));
    //private static readonly PropertyChangedEventArgs MoveSpeedChanged = new PropertyChangedEventArgs(nameof(MoveSpeed));

    private string _id;
    private string _name;
    private int _star;
    private int _exp;
    private int _level;

    private float _baseHp;
    private float _baseAttack;
    private float _baseDefense;
    private float _baseMoveSpeed;

    private readonly ElementType _elementType;
    private readonly string _iconPath;
    private bool _isMaxLevel;
    private bool _isMaxStar;

    private CharacterGradeData _currentGradeData;
    private LevelExpData _currentLevelData;

    public CharacterModel(CharacterData characterData)
    {
        _id = characterData.DataId;
        _name = characterData.Name;
        _star = characterData.Star;
        _exp = characterData.Exp;
        _level = CalculateLevel(_exp);

        _baseHp = characterData.Hp;
        _baseAttack = characterData.Attack;
        _baseDefense = characterData.Defence;
        _baseMoveSpeed = characterData.MoveSpeed;

        _iconPath = characterData.CharacterIconPath;

        TryUpdateCurrentGradeData(_star);
        TryUpdateCurrentLevelData(_level);

        _isMaxLevel = _level >= _currentGradeData.MaxLevel;
        _isMaxStar = _currentGradeData.RequiredToNext <= 0;
    }

    private bool TryUpdateCurrentGradeData(int star)
    {
        string dataId = CharId.GetCharGradeId(star);

        if (!GameManager.Instance.DataManager.TryGetData(dataId, out _currentGradeData))
        {
            Debug.LogError($"CharacterGradeData를 찾을 수 없습니다. DataId: {dataId}");
            return false;
        }

        return true;
    }

    private bool TryUpdateCurrentLevelData(int exp)
    {
        string dataId = CharId.GetCharLevelId(exp);

        if (!GameManager.Instance.DataManager.TryGetData(dataId, out _currentLevelData))
        {
            Debug.LogError($"CharacterLevelData를 찾을 수 없습니다. DataId: {dataId}");
            return false;
        }

        return true;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public string Id
    {
        get { return _id; }
        private set
        {
            if (_id != value)
            {
                _id = value;
                OnPropertyChanged(IdChanged);
            }
        }
    }

    public string Name
    {
        get { return _name; }
        private set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(NameChanged);
            }
        }
    }

    public int Star
    {
        get { return _star; }
        private set
        {
            if (_star != value)
            {
                _star = value;
                TryUpdateCurrentGradeData(_star);
                _isMaxStar = _currentGradeData.RequiredToNext <= 0;
                OnPropertyChanged(StarChanged);
            }
        }
    }

    public int Exp
    {
        get { return _exp; }
        private set
        {
            if (_exp != value)
            {
                _exp = value;
                OnPropertyChanged(ExpChanged);
            }
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
                IsMaxLevel = _level >= _currentGradeData.MaxLevel;
                TryUpdateCurrentLevelData(_level);
                OnPropertyChanged(LevelChanged);
            }
        }
    }

    //public float Hp
    //{
    //    get { return _hp; }
    //    private set
    //    {
    //        if (_hp != value)
    //        {
    //            _hp = value;
    //            OnPropertyChanged(HpChanged);
    //        }
    //    }
    //}

    //public float Attack
    //{
    //    get { return _attack; }
    //    private set
    //    {
    //        if (_attack != value)
    //        {
    //            _attack = value;
    //            OnPropertyChanged(AttakChanged);
    //        }
    //    }
    //}

    //public float Defense
    //{
    //    get { return _defense; }
    //    private set
    //    {
    //        if (_defense != value)
    //        {
    //            _defense = value;
    //            OnPropertyChanged(DefenseChanged);
    //        }
    //    }
    //}

    //public float MoveSpeed
    //{
    //    get { return _moveSpeed; }
    //    private set
    //    {
    //        if (_moveSpeed != value)
    //        {
    //            _moveSpeed = value;
    //            OnPropertyChanged(MoveSpeedChanged);
    //        }
    //    }
    //}

    public ElementType ElementType
    {
        get { return _elementType; }
    }

    public bool IsMaxLevel
    {
        get
        {
            return _isMaxLevel;
        }
        private set
        {
            if (_isMaxLevel != value)
            {
                _isMaxLevel = value;
                OnPropertyChanged(IsMaxLevelChanged);
            }
        }
    }

    public int RequiredExp
    {
        get
        {
            return _currentLevelData.RequiredExp;
        }
    }

    public bool IsMaxStar
    {
        get
        {
            return _isMaxStar;
        }
    }

    public int RequiredToNext
    {
        get
        {
            return _currentGradeData.RequiredToNext;
        }
    }

    public string IconPath
    {
        get { return _iconPath; }
    }

    public void InitProperty()
    {
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(IconPath)));
        OnPropertyChanged(IdChanged);
        OnPropertyChanged(NameChanged);
        OnPropertyChanged(StarChanged);
        OnPropertyChanged(ExpChanged);
        OnPropertyChanged(LevelChanged);
        OnPropertyChanged(IsMaxLevelChanged);
    }

    private int CalculateLevel(int exp)
    {
        Dictionary<string, LevelExpData> levelTable = new Dictionary<string, LevelExpData>();

        if (!GameManager.Instance.DataManager.TryGetDataTable(out levelTable))
        {
            Debug.LogError($"LevelExpTable를 찾을 수 없습니다.");
            return -1;
        }

        foreach (LevelExpData levelExp in levelTable.Values)
        {
            if (exp >= levelExp.RequiredExp)
            {
                continue;
            }

            return levelExp.Level;
        }

        Debug.LogError($"");
        return 1;
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

        Exp += amount;

        while (Exp >= _currentLevelData.RequiredExp)
        {
            Level++;
        }

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

        //TODO 아이템 가져오기
        int gradeItem = 100;

        if (gradeItem < _currentGradeData.RequiredToNext)
        {
            return false;
        }

        //TODO 아이템 소비
        gradeItem -= _currentGradeData.RequiredToNext;

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
// private static readonly PropertyChangedEventArgs SkillListChanged = new PropertyChangedEventArgs(nameof(SkillList));
// private string _skillList;

//public string SkillList
//{
//    get => _skillList;
//    private set
//    {
//        if (_skillList != value)
//        {
//            _skillList = value;
//            OnPropertyChanged(SkillListChanged);
//        }
//    }
//}



//?? 중복 승급 재화
//public int OwnedDuplicates { get; private set; }


//public StatData GetFinalStats()
//{
//    CharacterData stat = _dataProvider.GetStat(CharacterId);
//    CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
//    StatData equipmentBonus = GetEquipmentBonus();

//    return StatCalculator.Calculate(stat, grade, CurrentLevel, equipmentBonus);
//}

//public StatData GetEquipmentBonus()
//{
//    return StatCalculator.SumEquipmentStats(_inventory.GetEquippedItems(CharacterId));
//}

//public EquipmentInstance GetEquippedItem(EquipType slot)
//{
//    return _inventory.GetEquippedItem(CharacterId, slot);
//}

//public bool CanEquip(EquipmentInstance instance)
//{
//    if (null == instance)
//    {
//        return false;
//    }

//    if (!string.IsNullOrEmpty(instance.Data.AllowedId) && instance.Data.AllowedId != CharacterId)
//    {
//        return false;
//    }

//    return true;
//}

//public void Equip(EquipmentInstance instance)
//{
//    if (!CanEquip(instance))
//    {
//        Debug.LogWarning($"Equip: 장착할 수 없는 장비입니다. CharacterId={CharacterId}");
//        return;
//    }

//    if (instance.EquippedBy == CharacterId)
//    {
//        Debug.Log($"Equip: 이미 장착 중인 장비입니다. InstanceId={instance.InstanceId}");
//        return;
//    }

//    EquipmentInstance previousInSlot = _inventory.GetEquippedItem(CharacterId, instance.Type);
//    if (null != previousInSlot)
//    {
//        previousInSlot.ClearEquipped();
//    }

//    instance.SetEquippedBy(CharacterId);

//    OnEquipmentChanged?.Invoke();
//}

//public void Unequip(EquipType slot)
//{
//    EquipmentInstance instance = _inventory.GetEquippedItem(CharacterId, slot);
//    if (null == instance)
//    {
//        Debug.LogWarning($"Unequip: 장착된 장비가 없습니다. Slot={slot}, CharacterId={CharacterId}");
//        return;
//    }

//    instance.ClearEquipped();

//    OnEquipmentChanged?.Invoke();
//}

//public int GetRequiredExpForNextLevel()
//{
//    return _dataProvider.GetRequiredExp(CurrentLevel);
//}

//public int GetRequiredDuplicatesForPromotion()
//{
//    CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
//    if (null == grade)
//    {
//        return 0;
//    }

//    return grade.RequiredToNext > 0 ? grade.RequiredToNext : 0;
//}