using System;
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

public class StudentModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs NameChanged = new PropertyChangedEventArgs(nameof(Name));
    private static readonly PropertyChangedEventArgs StarChanged = new PropertyChangedEventArgs(nameof(Star));
    private static readonly PropertyChangedEventArgs PortraitKeyChanged = new PropertyChangedEventArgs(nameof(PortraitKey));


    private static readonly PropertyChangedEventArgs ExpChanged = new PropertyChangedEventArgs(nameof(Exp));
    private static readonly PropertyChangedEventArgs LevelChanged = new PropertyChangedEventArgs(nameof(Level));
    private static readonly PropertyChangedEventArgs IsMaxLevelChanged = new PropertyChangedEventArgs(nameof(IsMaxLevel));

    private static readonly PropertyChangedEventArgs HpChanged = new PropertyChangedEventArgs(nameof(FinalHp));
    private static readonly PropertyChangedEventArgs AttackChanged = new PropertyChangedEventArgs(nameof(FinalAttack));
    private static readonly PropertyChangedEventArgs DefenseChanged = new PropertyChangedEventArgs(nameof(FinalDefense));
    private static readonly PropertyChangedEventArgs MoveSpeedChanged = new PropertyChangedEventArgs(nameof(FinalMoveSpeed));

    //private static readonly PropertyChangedEventArgs EquipChanged = new PropertyChangedEventArgs(nameof(EquipItem));

    private string _dataId;
    private string _name;
    private int _star;
    private string _portraitKey;

    private int _exp;
    private int _level;

    private float _baseHp;
    private float _baseAttack;
    private float _baseDefense;
    private float _baseMoveSpeed;

    private float _addHp;
    private float _addAttack;
    private float _addDefense;
    private float _addMoveSpeed;

    private readonly ElementType _elementType;

    private bool _isMaxLevel;

    private CharacterGradeData _currentGradeData;
    private LevelExpData _currentLevelData;

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
                TryUpdateCurrentGradeData(_star);

                OnPropertyChanged(StarChanged);
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

    public string PortraitKey
    {
        get
        {
            return _portraitKey;
        }
    }


    public StudentModel(CharacterData characterData)
    {
        _dataId = characterData.DataId;
        _name = characterData.Name;
        _star = characterData.Star;
        _portraitKey = characterData.PortraitKey;


        _exp = characterData.Exp;
        _level = CalculateLevel(_exp);

        _baseHp = characterData.Hp;
        _baseAttack = characterData.Atk;
        _baseDefense = characterData.Def;
        _baseMoveSpeed = characterData.MoveSpeed;


        TryUpdateCurrentGradeData(_star);
        TryUpdateCurrentLevelData(_level);

        _isMaxLevel = _level >= _currentGradeData.MaxLevel;
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

    public float FinalHp
    {
        get { return _baseHp + _addHp; }
    }

    public float FinalAttack
    {
        get { return _baseAttack + _addAttack; }
    }

    public float FinalDefense
    {
        get { return _baseDefense + _addDefense; }
    }

    public float FinalMoveSpeed
    {
        get { return _baseMoveSpeed + _addMoveSpeed; }
    }

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



    public int RequiredToNext
    {
        get
        {
            return _currentGradeData.RequiredToNext;
        }
    }

    public void NotifyAllProperties()
    {
        OnPropertyChanged(NameChanged);
        OnPropertyChanged(StarChanged);
        OnPropertyChanged(PortraitKeyChanged);


        OnPropertyChanged(ExpChanged);
        OnPropertyChanged(LevelChanged);
        OnPropertyChanged(HpChanged);
        OnPropertyChanged(AttackChanged);
        OnPropertyChanged(DefenseChanged);
        OnPropertyChanged(MoveSpeedChanged);
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