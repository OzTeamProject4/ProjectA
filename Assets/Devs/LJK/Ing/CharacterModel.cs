using System.ComponentModel;

public class CharacterModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs IdChanged = new PropertyChangedEventArgs(nameof(Id));
    private static readonly PropertyChangedEventArgs NameChanged = new PropertyChangedEventArgs(nameof(Name));
    private static readonly PropertyChangedEventArgs StarChanged = new PropertyChangedEventArgs(nameof(Star));
    private static readonly PropertyChangedEventArgs ExpChanged = new PropertyChangedEventArgs(nameof(Exp));
    private static readonly PropertyChangedEventArgs HpChanged = new PropertyChangedEventArgs(nameof(Hp));
    private static readonly PropertyChangedEventArgs AttakChanged = new PropertyChangedEventArgs(nameof(Attack));
    private static readonly PropertyChangedEventArgs DefenseChanged = new PropertyChangedEventArgs(nameof(Defense));
    private static readonly PropertyChangedEventArgs MoveSpeedChanged = new PropertyChangedEventArgs(nameof(MoveSpeed));

    private string _id;
    private string _name;
    private int _star;
    private int _exp;
    private float _hp;
    private float _attack;
    private float _defense;
    private float _moveSpeed;
    private readonly ElementType _elementType;
    private readonly string _iconPath;

    public CharacterModel(CharacterData characterData)
    {
        _id = characterData.DataId;
        _name = characterData.Name;
        _star = characterData.Star;
        _exp = characterData.Exp;
        _iconPath = characterData.CharacterIconPath;
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

    public float Hp
    {
        get { return _hp; }
        private set
        {
            if (_hp != value)
            {
                _hp = value;
                OnPropertyChanged(HpChanged);
            }
        }
    }

    public float Attack
    {
        get { return _attack; }
        private set
        {
            if (_attack != value)
            {
                _attack = value;
                OnPropertyChanged(AttakChanged);
            }
        }
    }

    public float Defense
    {
        get { return _defense; }
        private set
        {
            if (_defense != value)
            {
                _defense = value;
                OnPropertyChanged(DefenseChanged);
            }
        }
    }

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        private set
        {
            if (_moveSpeed != value)
            {
                _moveSpeed = value;
                OnPropertyChanged(MoveSpeedChanged);
            }
        }
    }

    public ElementType ElementType
    {
        get { return _elementType; }
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
    //private readonly Inventory _inventory;
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

    //public bool IsMaxLevel
    //{
    //    get
    //    {
    //        CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
    //        if (null == grade)
    //        {
    //            return false;
    //        }

    //        return CurrentLevel >= grade.MaxLevel;
    //    }
    //}

    //public bool IsMaxStar
    //{
    //    get
    //    {
    //        CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
    //        if (null == grade)
    //        {
    //            return true;
    //        }

    //        return grade.RequiredToNext <= 0;
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

    //public void AddExp(int amount)
    //{
    //    if (amount <= 0)
    //    {
    //        Debug.LogWarning($"AddExp: 유효하지 않은 경험치({amount}).");
    //        return;
    //    }

    //    CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
    //    if (null == grade)
    //    {
    //        Debug.LogWarning($"AddExp: GradeData 없음. Star={CurrentStar}");
    //        return;
    //    }

    //    int maxLevel = grade.MaxLevel;
    //    if (CurrentLevel >= maxLevel)
    //    {
    //        Debug.Log($"이미 만렙(Lv{CurrentLevel}). 경험치 {amount} 무시. CharacterId={CharacterId}");
    //        return;
    //    }

    //    CurrentExp += amount;

    //    bool leveledUp = false;
    //    while (CurrentLevel < maxLevel)
    //    {
    //        int required = _dataProvider.GetRequiredExp(CurrentLevel);
    //        if (required <= 0)
    //        {
    //            break;
    //        }

    //        if (CurrentExp < required)
    //        {
    //            break;
    //        }

    //        CurrentExp -= required;
    //        CurrentLevel++;
    //        leveledUp = true;
    //    }

    //    if (CurrentLevel >= maxLevel)
    //    {
    //        CurrentExp = 0;
    //    }

    //    OnExpChanged?.Invoke();
    //    if (leveledUp)
    //    {
    //        OnLevelChanged?.Invoke();
    //    }
    //}

    //public bool CanPromote()
    //{
    //    CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
    //    if (null == grade || grade.RequiredToNext <= 0)
    //    {
    //        return false;
    //    }

    //    if (CurrentLevel < grade.MaxLevel)
    //    {
    //        return false;
    //    }

    //    return OwnedDuplicates >= grade.RequiredToNext;
    //}

    //public void Promote()
    //{
    //    CharacterGradeData grade = _dataProvider.GetGrade(CurrentStar);
    //    if (null == grade)
    //    {
    //        Debug.LogWarning($"CharacterGradeData 없음. Star={CurrentStar}");
    //        return;
    //    }

    //    if (grade.RequiredToNext <= 0)
    //    {
    //        Debug.LogWarning($"이미 최대 성급({CurrentStar}) 입니다.");
    //        return;
    //    }

    //    if (CurrentLevel < grade.MaxLevel)
    //    {
    //        Debug.LogWarning($"만렙(Lv{grade.MaxLevel}) 도달 후 승급 가능. 현재 Lv{CurrentLevel}");
    //        return;
    //    }

    //    if (OwnedDuplicates < grade.RequiredToNext)
    //    {
    //        Debug.LogWarning($"캐릭터 보유 수량 부족. 필요={grade.RequiredToNext}, 보유={OwnedDuplicates}");
    //        return;
    //    }

    //    OwnedDuplicates -= grade.RequiredToNext;
    //    CurrentStar++;

    //    OnDuplicatesChanged?.Invoke();
    //    OnStarChanged?.Invoke();
    //}

    //public void AddDuplicate(int count)
    //{
    //    if (count <= 0)
    //    {
    //        Debug.LogWarning($"유효하지 않은 수량({count}).");
    //        return;
    //    }

    //    OwnedDuplicates += count;
    //    OnDuplicatesChanged?.Invoke();
    //}

    //public bool CanUseExpItem(string dataId)
    //{
    //    if (IsMaxLevel)
    //    {
    //        return false;
    //    }

    //    return _inventory.GetItemCount(dataId) > 0;
    //}

    //public void UseExpItem(string dataId)
    //{
    //    if (string.IsNullOrEmpty(dataId))
    //    {
    //        Debug.LogWarning("UseExpItem: dataId 가 비어 있습니다.");
    //        return;
    //    }

    //    ItemData item = _dataProvider.GetItem(dataId);
    //    if (null == item)
    //    {
    //        Debug.LogWarning($"UseExpItem: ItemData 를 찾을 수 없습니다. dataId={dataId}");
    //        return;
    //    }

    //    if (IsMaxLevel)
    //    {
    //        Debug.Log($"만렙이라 경험치 아이템 사용 불가. dataId={dataId}");
    //        return;
    //    }

    //    if (!_inventory.TryConsumeItem(dataId))
    //    {
    //        Debug.LogWarning($"보유 수량 없음. dataId={dataId}");
    //        return;
    //    }

    //    AddExp(item.Value);
    //}

    //public IReadOnlyList<ItemData> GetExpItems()
    //{
    //    List<ItemData> items = new();

    //    foreach (string dataId in _dataProvider.GetAllExpItemIds())
    //    {
    //        ItemData item = _dataProvider.GetItem(dataId);
    //        if (null != item)
    //        {
    //            items.Add(item);
    //        }
    //    }

    //    return items;
    //}

    //public string GetCharacterIconPath(string characterId)
    //{
    //    if (string.IsNullOrEmpty(characterId))
    //    {
    //        return null;
    //    }

    //    CharacterData characterData = _dataProvider.GetStat(characterId);

    //    if (null == characterData)
    //    {
    //        return null;
    //    }

    //    return characterData.CharacterIconPath;
    //}