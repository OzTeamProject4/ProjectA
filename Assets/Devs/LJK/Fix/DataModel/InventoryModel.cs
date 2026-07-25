using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using static UnityEngine.CullingGroup;
using static UnityEngine.Rendering.DebugUI;

public enum ItemType
{
    Equipment,
    Material
}

public enum MaterialType
{
    Exp,
}

public enum EquipType
{
    Weapon,
    Helmet,
    Armor,
    Greeve,
    Accessory,
    Signature
}

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

    private MaterialType _materialType;
    private int _tier;
    private int _value;
    private int _count;

    public MaterialType MaterialType
    {
        get
        {
            return _materialType;
        }
    }

    public int Tier
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

    public MaterialModel(ItemData itemData, int tier,  int value, int count) : base(itemData)
    {
        _materialType = MaterialType.Exp;
        _tier = tier;
        _value = value;
        _count = count;
    }

    public MaterialModel(ItemData itemData, int count) : base(itemData)
    {
        if (!GameManager.Instance.DataManager.TryGetData(itemData.TypeDataId, out MaterialData materialData))
        {
            Debug.LogError($"{itemData.TypeDataId}를 가진 MaterialData가 없습니다");
            return;
        }

        _materialType = materialData.MaterialType;
        _tier = materialData.Tier;
        _value = materialData.Value;
        _count = count;
    }

    public override void NotifyAllProperties()
    {
        base.NotifyAllProperties();
        OnPropertyChanged(CountChanged);
    }

    public void UseExpItem(StudentModel studentModel)
    {
        if (studentModel.TryAddExp(_value))
        {
            Count -= 1;
        }
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


//public class ItemModel : INotifyPropertyChanged
//{

//    private static readonly PropertyChangedEventArgs EquipChanged = new PropertyChangedEventArgs(nameof(EquipId));

//    //private List<StatInfo> _statInfo;
//    private string _equipId;


//    public string EquipId
//    {
//        get
//        {
//            return _equipId;
//        }
//        private set
//        {
//            if (_equipId != value)
//            {
//                _equipId = value;
//                OnPropertyChanged(EquipChanged);
//            }
//        }
//    }

//    //public IReadOnlyList<StatInfo> StatInfo
//    //{
//    //    get { return _statInfo; }
//    //}

//  
//public ItemModel(ItemData itemData)
//{
//   
//    //_statInfo = new List<StatInfo>() { new StatInfo(StatType.Atk, 10) };
//}

//    public void Equip(string id)
//    {
//        if (EquipId != null)
//        {
//            return;
//        }

//        EquipId = id;
//    }

//    public void UnEquip()
//    {
//        if (EquipId == null)
//        {
//            return;
//        }

//        EquipId = null;
//    }


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

    public IReadOnlyDictionary<string, MaterialModel> GetItemsByMaterialType(MaterialType materialType)
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

    //public void AddItem()
    //{
    //    if (_itemList.TryGetValue(itemId, out ItemModel item))
    //    {
    //        item.AddCount(amount);
    //    }
    //    else
    //    {
    //        _itemList.Add(itemId, new ItemModel(itemId, path, amount, equipType, tier, itemType, Value));
    //    }

    //    OnPropertyChanged(InventoryChanged);
    //}
    
    //TODO Test용 추후 삭제바람
    public void AddExpItem(MaterialModel materialModel)
    {
        _inventory.Add(materialModel.DataId, materialModel);
    }

    //public bool RemoveItem(string itemId, int amount)
    //{
    //    if (!_inventory.TryGetValue(itemId, out ItemModel item))
    //    {
    //        return false;
    //    }

    //    if (item.Count < amount)
    //    {
    //        return false;
    //    }

    //    item.RemoveCount(amount);

    //    if (item.Count == 0)
    //    {
    //        _itemList.Remove(itemId);
    //    }

    //    OnPropertyChanged(InventoryChanged);

    //    return true;
    //}

    private void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, propertyChangedEventArgs);
    }
}