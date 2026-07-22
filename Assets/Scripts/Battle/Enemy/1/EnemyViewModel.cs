using System.Buffers.Text;
using UnityEngine;
public class EnemyViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(EnemyViewModel));
        OnPropertyChanged(nameof(EnemyDataId));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(TotalExp));
        OnPropertyChanged(nameof(CurrentLevel));
        OnPropertyChanged(nameof(ElementalType));
        OnPropertyChanged(nameof(BaseHp));
        OnPropertyChanged(nameof(CurrentHp));
        OnPropertyChanged(nameof(BaseDamage));
        OnPropertyChanged(nameof(CurrentDamage));
        OnPropertyChanged(nameof(PrefabAddress));

    }

    private string _enemyDataId;
    private string _name;
    private int _totalExp;
    private int _currentLevel;
    private ElementType _elementalType;
    private int _baseHp;
    private int _maxHp;
    private int _currentHp;
    private int _baseDamage;
    private int _currentDamage;
    private string _prefabAddress;
    private string _skillPrefabAddress;



    public string EnemyDataId
    {
        get => _enemyDataId;
        set
        {
            if (_enemyDataId != value)
            {
                _enemyDataId = value;
                OnPropertyChanged(nameof(EnemyDataId));
            }
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public int TotalExp
    {
        get => _totalExp;
        set
        {
            if (_totalExp != value)
            {
                _totalExp = value;
                CurrentLevel = (int)_totalExp / 100;
                OnPropertyChanged(nameof(TotalExp));
            }
        }
    }

    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            if (_currentLevel != value)
            {
                _currentLevel = value;
                OnPropertyChanged(nameof(CurrentLevel));
            }
        }
    }

    public ElementType ElementalType
    {
        get => _elementalType;
        set
        {
            if (_elementalType != value)
            {
                _elementalType = value;
                OnPropertyChanged(nameof(ElementalType));
            }
        }
    }

    public int BaseHp
    {
        get => _baseHp;
        set
        {
            if (_baseHp != value)
            {
                _baseHp = value;
                OnPropertyChanged(nameof(BaseHp));
            }
        }
    }

    public int CurrentHp
    {
        get => _currentHp;
        set
        {
            if (_currentHp != value)
            {
                _currentHp = value;
                OnPropertyChanged(nameof(CurrentHp));
            }
        }
    }

    public int MaxHp
    {
        get => _maxHp;
        set
        {
            if (_maxHp != value)
            {
                _maxHp = value;
                OnPropertyChanged(nameof(MaxHp));
            }
        }
    }

    public int BaseDamage
    {
        get => _baseDamage;
        set
        {
            if (_baseDamage != value)
            {
                _baseDamage = value;
                OnPropertyChanged(nameof(BaseDamage));
            }
        }
    }
    public int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            if (_currentDamage != value)
            {
                _currentDamage = value;
                OnPropertyChanged(nameof(CurrentDamage));
            }
        }
    }

    public string PrefabAddress
    {
        get => _prefabAddress;
        set
        {
            if (_prefabAddress != value)
            {
                _prefabAddress = value;
                OnPropertyChanged(nameof(PrefabAddress));
            }
        }
    }

    public string SkillPrefabAddress
    {
        get => _skillPrefabAddress;
        set
        {
            if (_skillPrefabAddress != value)
            {
                _skillPrefabAddress = value;
                OnPropertyChanged(nameof(SkillPrefabAddress));
            }
        }
    }


}
