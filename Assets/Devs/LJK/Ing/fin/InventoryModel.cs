using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ItemModel : INotifyPropertyChanged
{ 
    private string _name;
    private string _description;
    private string _iconKey;

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
        _name = itemData.Name;
        _description = itemData.Description;
        _iconKey = itemData.IconKey;
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

    private int _count;

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
    }
}

public class EquipmentModel : ItemModel
{
    public EquipType _equipType;

    public EquipType EquipType
    { 
        get 
        {
            return _equipType; 
        }
    }

    public EquipmentModel(ItemData itemData) : base(itemData)
    {
        if (!GameManager.Instance.DataManager.TryGetData(itemData.TypeDataId, out EquipmentData equipmentData))
        {
            Debug.LogError($"{itemData.TypeDataId}를 가진 EquipmentData가 없습니다");
            return;
        }

        _equipType = equipmentData.EquipType;
    }
}


//public class ItemModel : INotifyPropertyChanged
//{

//    private static readonly PropertyChangedEventArgs EquipChanged = new PropertyChangedEventArgs(nameof(EquipId));




//    private string _itemId;




//    private ItemType itemType;
//    private int _tier;

//    //private List<StatInfo> _statInfo;
//    private string _equipId;
//    private int _value;

//    public string ItemId { get { return _itemId; } }
//    public string ItemName { get { return "Test"; } }
//    public ItemType ItemType { get { return itemType; } }

//    public EquipType EquipType { get { return _equipType; } }
//    public int Tier { get { return _tier; } }
//    public int Value { get { return _value; } }



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
//    _itemId = itemId;
//    _iconPath = path;
//    _count = count;
//    _equipType = equipType;
//    _tier = tier;
//    //_statInfo = new List<StatInfo>() { new StatInfo(StatType.Atk, 10) };
//    itemType = itemTypes;
//    _value = value;
//}


//    public void AddCount(int amount)
//    {
//        Count += amount;
//    }

//    public void RemoveCount(int amount)
//    {
//        Count -= amount;
//    }

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



//    internal void UseItem(StudentModel characterModel)
//    {
//        if (characterModel.TryAddExp(Value))
//        {
//            RemoveCount(1);
//        }
//    }
//}

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

    //수정
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

    public void AddItem(string itemId, string path, int amount, EquipType equipType, int tier, ItemType itemType = ItemType.Material, int Value = 0)
    {
        //if (_itemList.TryGetValue(itemId, out ItemModel item))
        //{
        //    item.AddCount(amount);
        //}
        //else
        //{
        //    _itemList.Add(itemId, new ItemModel(itemId, path, amount, equipType, tier, itemType, Value));
        //}

        OnPropertyChanged(InventoryChanged);
    }

    public bool RemoveItem(string itemId, int amount)
    {
        if (!_inventory.TryGetValue(itemId, out ItemModel item))
        {
            return false;
        }

        //if (item.Count < amount)
        //{
        //    return false;
        //}

        //item.RemoveCount(amount);

        //if (item.Count == 0)
        //{
        //    _itemList.Remove(itemId);
        //}

        OnPropertyChanged(InventoryChanged);

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