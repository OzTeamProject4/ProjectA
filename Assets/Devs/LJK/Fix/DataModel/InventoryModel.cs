using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using static UnityEngine.CullingGroup;
using static UnityEngine.Rendering.DebugUI;

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

        if (!GameManager.Instance.DataManager.TryGetData(itemData.TypeDataId, out CurrencyData currencyData))
        {
            Debug.LogError($"'{itemData.TypeDataId}'를 가진 CurrencyData가 없습니다. DataId={itemData.DataId}");
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
    public EquipType _equipType;
    private List<StatInfo> _statInfos;

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

    public EquipmentModel(ItemData itemData) : base(itemData)
    {
        if (!GameManager.Instance.DataManager.TryGetData(itemData.TypeDataId, out EquipmentData equipmentData))
        {
            Debug.LogError($"{itemData.TypeDataId}를 가진 EquipmentData가 없습니다");
            return;
        }

        _equipType = equipmentData.EquipType;
        _statInfos = CreateStatInfos(equipmentData);
    }

    private static List<StatInfo> CreateStatInfos(EquipmentData equipmentData)
    {
        List<StatInfo> statInfos = new List<StatInfo>()
        {
            new StatInfo(StatType.Hp, equipmentData.Hp),
            new StatInfo(StatType.Attack, equipmentData.Attack),
            new StatInfo(StatType.Defense, equipmentData.Defense),
            new StatInfo(StatType.MoveSpeed, equipmentData.MoveSpeed)
        };

        return statInfos;
    }
}

public class InventoryModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs InventoryChanged = new PropertyChangedEventArgs(nameof(Inventory));

    private readonly Dictionary<string, ItemModel> _inventory;

    public IReadOnlyDictionary<string, ItemModel> Inventory
    {
        get { return _inventory; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public InventoryModel()
    {
        _inventory = new Dictionary<string, ItemModel>();
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
    public IReadOnlyDictionary<string, EquipmentModel> GetItemsByEquipType(EquipType equipType)
    {
        Dictionary<string, EquipmentModel> filteredItems = new Dictionary<string, EquipmentModel>();

        foreach (ItemModel item in _inventory.Values)
        {
            if (item is not EquipmentModel equipmentModel)
            {
                continue;
            }

            if (equipmentModel.EquipType != equipType)
            {
                continue;
            }

            filteredItems.Add(equipmentModel.DataId, equipmentModel);
        }

        return filteredItems;
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