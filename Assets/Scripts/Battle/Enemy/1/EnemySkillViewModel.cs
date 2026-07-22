using System.Buffers.Text;
using UnityEngine;
public class EnemySkillViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(EnemyViewModel));
        OnPropertyChanged(nameof(SkillDataId));
        OnPropertyChanged(nameof(InstanceId));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(User));
        OnPropertyChanged(nameof(ElementalType));
        OnPropertyChanged(nameof(BaseDamage));
        OnPropertyChanged(nameof(PrefabAddress));

    }

    private string _skillDataId;
    private int _instanceId;
    private string _name;
    private GameObject _user;
    private string _elementalType;
    private float _baseDamage;
    private string _prefabAddress;



    public string SkillDataId
    {
        get => _skillDataId;
        set
        {
            if (_skillDataId != value)
            {
                _skillDataId = value;
                OnPropertyChanged(nameof(SkillDataId));
            }
        }
    }

    public int InstanceId
    {
        get => _instanceId;
        set
        {
            if (_instanceId != value)
            {
                _instanceId = value;
                OnPropertyChanged(nameof(InstanceId));
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

    public GameObject User
    {
        get => _user;
        set
        {
            if (_user != value)
            {
                _user = value;
                OnPropertyChanged(nameof(User));
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
