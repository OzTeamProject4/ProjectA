using System.Buffers.Text;
using UnityEngine;

public class EnemyViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(EnemyViewModel));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(TotalExp));
        OnPropertyChanged(nameof(CurrentLevel));
        OnPropertyChanged(nameof(ElementalType));
        OnPropertyChanged(nameof(BaseHp));
        OnPropertyChanged(nameof(BaseDamage));
        OnPropertyChanged(nameof(PrefabAddress));

    }

    private int _instanceId;
    private string _enemyDataId;
    private string _name;
    private int _totalExp;
    private int _currentLevel;
    private string _elementalType;
    private float _baseHp;
    private float _baseDamage;
    private string _prefabAddress;

    public int EnemyInstanceId
    {
        get => _instanceId;
        set
        {
            if (_instanceId != value)
            {
                _instanceId = value;
                OnPropertyChanged(nameof(EnemyInstanceId));
            }
        }
    }

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

    public string ElementalType
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

    public float BaseHp
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
    public float BaseDamage
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

}
