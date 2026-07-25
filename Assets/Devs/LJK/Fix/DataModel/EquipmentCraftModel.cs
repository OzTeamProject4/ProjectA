using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentCraftModel
{
    private string _dataId;
    private string _name;
    private EquipType _equipType;
    private string _iconKey;
    private int _requiredGold; //TODO 리퀘스트 아이템으로 들어가도 되지않을까
    private List<string> _requiredItemIds;
    private List<int> _requiredItemCounts;
    private List<StatInfo> _statInfos;

    public string DataId
    {
        get
        { 
            return _dataId; 
        }
    }

    public string Name
    {
        get
        { 
            return _name; 
        }
    }

    public EquipType EquipType
    {
        get { return _equipType; }
    }

    public string IconKey
    {
        get 
        { 
            return _iconKey;
        }
    }

    public int RequiredGold
    {
        get 
        { 
            return _requiredGold;
        }
    }

    public IReadOnlyList<string> RequiredItemIds
    {
        get { return _requiredItemIds; }
    }

    public IReadOnlyList<int> RequiredItemCounts
    {
        get { return _requiredItemCounts; }
    }

    public IReadOnlyList<StatInfo> StatInfos
    {
        get { return _statInfos; }
    }

    public EquipmentCraftModel(ItemData itemData)
    {
        if(!GameManager.Instance.DataManager.TryGetData(itemData.TypeDataId, out EquipmentData equipmentData))
        {
            Debug.Log($"{itemData.TypeDataId}의 EquipmentData가 없습니다.");
            return;
        }

        _dataId = itemData.DataId;
        _name = itemData.Name;
        _iconKey = itemData.IconKey;
        _equipType = equipmentData.EquipType;
        _requiredGold = equipmentData.RequiredGold;
        _requiredItemIds = equipmentData.RequiredItemIds.ToList();
        _requiredItemCounts = equipmentData.RequiredItemCounts.ToList();
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