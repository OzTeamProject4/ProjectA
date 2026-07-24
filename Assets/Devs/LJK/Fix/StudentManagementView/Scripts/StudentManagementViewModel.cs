using System;
using System.ComponentModel;

public class StudentManagementViewModel
{
    private StudentModel _studentModel;
    private InventoryModel _inventoryModel;

    public StudentModel StudentModel
    {
        get 
        {
            return _studentModel; 
        }
    }

    public int OwnedGradeUpItemCount
    {
        get
        {
            return GetItemCount(_studentModel.RequiredGradeUpItemId);
        }
    }

    public string Name
    {
        get
        {
            return _studentModel.Name;
        }
    }

    public int Star
    {
        get
        {
            return _studentModel.Star;
        }
    }

    public bool IsMaxStar
    {
        get
        {
            return _studentModel.IsMaxStar;
        }
    }

    public ElementType ElementType
    {
        get
        {
            return _studentModel.ElementType;
        }
    }

    public int RequiredGradeUpItemCount
    {
        get
        {
            return _studentModel.RequiredGradeUpItemCount;
        }
    }

    public string FullBodyKey
    {
        get
        {
            return _studentModel.FullBodyKey;
        }
    }

    public int TotalExperience
    {
        get
        {
            return _studentModel.TotalExperience;
        }
    }

    public int RequiredExp
    {
        get
        {
            return _studentModel.RequiredExp;
        }
    }

    public int Level
    {
        get
        {
            return _studentModel.Level;
        }
    }

    public bool IsMaxLevel
    {
        get
        {
            return _studentModel.IsMaxLevel;
        }
    }

    //public IReadOnlyList<ItemModel> EquipItem
    //{
    //    get { return _characterModel.EquipItem; }
    //}

    public float TotalHp
    {
        get
        {
            return _studentModel.TotalHp;
        }
    }

    public float TotalAttack
    {
        get
        {
            return _studentModel.TotalAttack;
        }
    }

    public float TotalDefense
    {
        get
        {
            return _studentModel.TotalDefense;
        }
    }

    public float TotalMoveSpeed
    {
        get
        {
            return _studentModel.TotalMoveSpeed;
        }
    }

    public StudentManagementViewModel()
    {
        _inventoryModel = NetworkManagerTemp.Instance.InventoryModel;
        _inventoryModel.PropertyChanged += OnInventoryChanged;
    }

    public event Action<string> PropertyChanged;

    public void SetModel(StudentModel studentModel)
    {
        if (_studentModel != null)
        {
            _studentModel.PropertyChanged -= OnPropertyChanged;
        }

        _studentModel = studentModel;
        _studentModel.PropertyChanged += OnPropertyChanged;
    }

    public void Refresh()
    {
        _studentModel.NotifyAllProperties();
    }

    public void Dispose()
    {
        _inventoryModel.PropertyChanged -= OnInventoryChanged;

        if (_studentModel == null)
        {
            return;
        }

        _studentModel.PropertyChanged -= OnPropertyChanged;
        _studentModel = null;
    }

    private int GetItemCount(string itemId)
    {
        if (!_inventoryModel.TryGetItem(itemId, out ItemModel item))
        {
            return 0;
        }

        if (item is not MaterialModel materialModel)
        {
            return 0;
        }

        return materialModel.Count;
    }

    private void OnInventoryChanged(object sender, PropertyChangedEventArgs e)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(nameof(OwnedGradeUpItemCount));
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(e.PropertyName);
    }

    //public void RequestAddExp(int exp)
    //{
    //    _characterModel.TryAddExp(exp);
    //}

    //public void RequestGradeUp()
    //{
    //    _characterModel.TryGradeUp();
    //}
}