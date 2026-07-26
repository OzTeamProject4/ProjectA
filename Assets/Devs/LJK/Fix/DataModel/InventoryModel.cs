using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ItemModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs NameChanged = new PropertyChangedEventArgs(nameof(Name));
    private static readonly PropertyChangedEventArgs IconKeyChanged = new PropertyChangedEventArgs(nameof(IconKey));

    private string _dataId;
    private string _name;
    private string _description;
    private ItemType _itemType;
    private string _iconKey;

    public string DataId 
    {
        get
        {
            return _dataId; 
        }
    }

    public ItemType ItemType
    {
        get 
        {
            return _itemType;
        }
    }

    public string Name 
    {
        get 
        {
            return _name;
        } 
    }

    public string Description
    {
        get 
        {
            return _description;
        } 
    }

    public string IconKey 
    {
        get 
        {
            return _iconKey;
        } 
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ItemModel(ItemData itemData)
    {
        _dataId = itemData.DataId;
        _name = itemData.Name;
        _description = itemData.Description;
        _itemType = itemData.ItemType;
        _iconKey = itemData.IconKey;
    }

    public virtual void NotifyAllProperties()
    {
        OnPropertyChanged(NameChanged);
        OnPropertyChanged(IconKeyChanged);
    }

    protected void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, propertyChangedEventArgs);
    }
}

public class MaterialModel : ItemModel
{
    private static readonly PropertyChangedEventArgs CountChanged = new PropertyChangedEventArgs(nameof(Count));

    private CurrencyType _materialType;
    private string _tier;
    private int _value;
    private int _count;

    public CurrencyType MaterialType
    {
        get
        {
            return _materialType;
        }
    }

    public string Tier
    {
        get
        {
            return _tier;
        }
    }

    public int Count
    {
        get
        {
            return _count;
        }
        private set
        {
            if (_count != value)
            {
                _count = value;
                OnPropertyChanged(CountChanged);
            }
        }
    }

    public MaterialModel(ItemData itemData, int count) : base(itemData)
    {
        _count = count;

        if (!GameManager.Instance.DataManager.TryGetData(itemData.ForeignKey, out CurrencyData currencyData))
        {
            Debug.LogError($"'{itemData.ForeignKey}'를 가진 CurrencyData가 없습니다. DataId={itemData.DataId}");
            return;
        }

        _materialType = currencyData.CurrencyType;
        _tier = currencyData.Tier;
        _value = currencyData.Value;
    }

    public override void NotifyAllProperties()
    {
        base.NotifyAllProperties();
        OnPropertyChanged(CountChanged);
    }

    // 스택형 재료의 소비
    public bool TryConsume(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError($"[MaterialModel:TryConsume] 유효하지 않은 수량({amount}). DataId={DataId}");
            return false;
        }

        if (_count < amount)
        {
            return false;
        }

        Count -= amount;
        return true;
    }

    public void UseExpItem(StudentModel studentModel)
    {
        if (_count <= 0)
        {
            return;
        }

        if (!studentModel.TryAddExp(_value))
        {
            return;
        }

        TryConsume(1);
    }
}

public class EquipmentModel : ItemModel
{
    private static readonly PropertyChangedEventArgs EquippedByChanged = new PropertyChangedEventArgs(nameof(EquippedBy));

    private readonly string _instanceId;
    private EquipType _equipType;
    private List<StatInfo> _statInfos;
    private string _equippedBy;

    public string InstanceId
    {
        get
        {
            return _instanceId;
        }
    }

    public EquipType EquipType
    {
        get
        {
            return _equipType;
        }
    }

    public IReadOnlyList<StatInfo> StatInfos
    {
        get { return _statInfos; }
    }

    public string EquippedBy
    {
        get
        {
            return _equippedBy;
        }
        private set
        {
            if (_equippedBy == value)
            {
                return;
            }

            _equippedBy = value;
            OnPropertyChanged(EquippedByChanged);
        }
    }

    public bool IsEquipped
    {
        get
        {
            return !string.IsNullOrEmpty(_equippedBy);
        }
    }

    public EquipmentModel(string instanceId, ItemData itemData) : base(itemData)
    {
        _instanceId = instanceId;

        if (!GameManager.Instance.DataManager.TryGetData(itemData.ForeignKey, out EquipmentData equipmentData))
        {
            Debug.LogError($"'{itemData.ForeignKey}'를 가진 EquipmentData가 없습니다. DataId={itemData.DataId}");
            return;
        }

        _equipType = equipmentData.EquipmentType;
        _statInfos = CreateStatInfos(equipmentData);
    }

    public void SetEquippedBy(string studentDataId)
    {
        EquippedBy = studentDataId;
    }

    public void ClearEquippedBy()
    {
        EquippedBy = null;
    }

    private static List<StatInfo> CreateStatInfos(EquipmentData equipmentData)
    {
        List<StatInfo> statInfos = new List<StatInfo>()
        {
            new StatInfo(StatType.Hp, equipmentData.MaxHp),
            new StatInfo(StatType.Attack, equipmentData.Attack),
            new StatInfo(StatType.Defense, equipmentData.Defence),
            new StatInfo(StatType.MoveSpeed, equipmentData.MoveSpeed)
        };

        return statInfos;
    }
}

public class InventoryModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs InventoryChanged = new PropertyChangedEventArgs(nameof(Inventory));

    // 재료는 스택형, 장비는 인스턴스형
    private readonly Dictionary<string, ItemModel> _inventory;
    private readonly Dictionary<string, EquipmentModel> _equipments;

    public IReadOnlyDictionary<string, ItemModel> Inventory
    {
        get { return _inventory; }
    }

    public IReadOnlyDictionary<string, EquipmentModel> Equipments
    {
        get { return _equipments; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public InventoryModel()
    {
        _inventory = new Dictionary<string, ItemModel>();
        _equipments = new Dictionary<string, EquipmentModel>();
    }

    public void NotifyAllProperties()
    {
        OnPropertyChanged(InventoryChanged);
    }

    //TODO 로직 수정
    public bool TryGetItem(string itemId, out ItemModel item)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            item = null;
            return false;
        }

        if (!_inventory.TryGetValue(itemId, out item))
        {
            return false;
        }

        return true;
    }

    public IReadOnlyDictionary<string, ItemModel> GetItemsByItemType(ItemType itemType)
    {
        Dictionary<string, ItemModel> filteredItems = new Dictionary<string, ItemModel>();

        foreach (ItemModel item in _inventory.Values)
        {
            if (item.ItemType != itemType)
            {
                continue;
            }

            filteredItems.Add(item.DataId, item);
        }

        return filteredItems;
    }

    public IReadOnlyList<EquipmentModel> GetEquipmentsByEquipType(EquipType equipType)
    {
        List<EquipmentModel> filteredEquipments = new List<EquipmentModel>();

        foreach (EquipmentModel equipmentModel in _equipments.Values)
        {
            if (equipmentModel.EquipType != equipType)
            {
                continue;
            }

            filteredEquipments.Add(equipmentModel);
        }

        return filteredEquipments;
    }

    public bool TryGetEquipment(string instanceId, out EquipmentModel equipmentModel)
    {
        if (string.IsNullOrWhiteSpace(instanceId))
        {
            equipmentModel = null;
            return false;
        }

        return _equipments.TryGetValue(instanceId, out equipmentModel);
    }

    public void AddEquipment(EquipmentModel equipmentModel)
    {
        if (equipmentModel == null || string.IsNullOrWhiteSpace(equipmentModel.InstanceId))
        {
            Debug.LogError("[InventoryModel:AddEquipment] InstanceId가 없는 장비는 추가할 수 없습니다.");
            return;
        }

        if (!_equipments.TryAdd(equipmentModel.InstanceId, equipmentModel))
        {
            Debug.LogError($"[InventoryModel:AddEquipment] InstanceId가 중복됩니다. InstanceId={equipmentModel.InstanceId}");
        }
    }

    public bool TryEquip(StudentModel studentModel, string instanceId)
    {
        if (studentModel == null)
        {
            Debug.LogError("[InventoryModel:TryEquip] studentModel이 null입니다.");
            return false;
        }

        if (!TryGetEquipment(instanceId, out EquipmentModel equipmentModel))
        {
            Debug.LogError($"[InventoryModel:TryEquip] 장비를 찾을 수 없습니다. InstanceId={instanceId}");
            return false;
        }

        if (equipmentModel.IsEquipped)
        {
            if (equipmentModel.EquippedBy == studentModel.DataId)
            {
                return false;
            }

            if (!TryUnequipFrom(equipmentModel.EquippedBy, equipmentModel.EquipType))
            {
                return false;
            }
        }

        TryUnequip(studentModel, equipmentModel.EquipType);

        studentModel.Equip(equipmentModel.EquipType, equipmentModel.InstanceId);
        equipmentModel.SetEquippedBy(studentModel.DataId);

        return true;
    }

    public bool TryUnequip(StudentModel studentModel, EquipType equipType)
    {
        if (studentModel == null)
        {
            Debug.LogError("[InventoryModel:TryUnequip] studentModel이 null입니다.");
            return false;
        }

        if (!studentModel.TryGetEquippedItemId(equipType, out string instanceId))
        {
            return false;
        }

        studentModel.Unequip(equipType);

        if (TryGetEquipment(instanceId, out EquipmentModel equipmentModel))
        {
            equipmentModel.ClearEquippedBy();
        }

        return true;
    }

    private bool TryUnequipFrom(string studentDataId, EquipType equipType)
    {
        StudentModel owner = NetworkManagerTemp.Instance.StudentListModel.GetCharacter(studentDataId);

        if (owner == null)
        {
            Debug.LogWarning($"[InventoryModel:TryUnequipFrom] 장착자를 찾을 수 없습니다. DataId={studentDataId}");
            return false;
        }

        return TryUnequip(owner, equipType);
    }

    public IReadOnlyDictionary<string, MaterialModel> GetItemsByMaterialType(CurrencyType materialType)
    {
        Dictionary<string, MaterialModel> filteredItems = new Dictionary<string, MaterialModel>();

        foreach (ItemModel item in _inventory.Values)
        {
            if (item is not MaterialModel materialModel)
            {
                continue;
            }

            if (materialModel.MaterialType != materialType)
            {
                continue;
            }

            filteredItems.Add(materialModel.DataId, materialModel);
        }

        return filteredItems;
    }

    //TODO Test용 추후 삭제바람
    public void AddExpItem(MaterialModel materialModel)
    {
        _inventory.Add(materialModel.DataId, materialModel);
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