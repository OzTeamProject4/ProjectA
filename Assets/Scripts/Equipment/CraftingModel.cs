using System.Collections.Generic;
using UnityEngine;

public class CraftingModel
{
    private readonly Inventory _inventory;
    private readonly IGameDataProvider _dataProvider;

    public CraftingModel(Inventory inventory, IGameDataProvider dataProvider)
    {
        if (null == inventory)
        {
            Debug.LogError("CraftingModel: Inventory 가 null 입니다.");
        }

        if (null == dataProvider)
        {
            Debug.LogError("CraftingModel: IGameDataProvider 가 null 입니다.");
        }

        _inventory = inventory;
        _dataProvider = dataProvider;
    }

    public IReadOnlyList<EquipmentData> GetCraftableEquipment(EquipType type, string characterId)
    {
        List<EquipmentData> result = new();

        foreach (EquipmentData data in _dataProvider.GetAllEquipment())
        {
            if (data.Type != type)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(data.AllowedId) && data.AllowedId != characterId)
            {
                continue;
            }

            result.Add(data);
        }

        return result;
    }

    public ItemData GetMaterialData(string itemId)
    {
        return _dataProvider.GetItem(itemId);
    }

    public bool CanCraft(EquipmentData data)
    {
        if (null == data)
        {
            return false;
        }

        if (data.Type == EquipType.Signature && _inventory.HasEquipment(data.DataId))
        {
            return false;
        }

        if (_inventory.Gold < data.Gold)
        {
            return false;
        }

        if (!data.TryGetRequiredMaterials(out (string ItemId, int Count)[] materials))
        {
            return true;
        }

        foreach ((string itemId, int count) in materials)
        {
            if (_inventory.GetItemCount(itemId) < count)
            {
                return false;
            }
        }

        return true;
    }

    public EquipmentData GetEquipmentData(string dataId)
    {
        return _dataProvider.GetEquipment(dataId);
    }

    public EquipmentInstance Craft(string equipmentDataId)
    {
        EquipmentData data = _dataProvider.GetEquipment(equipmentDataId);
        if (null == data)
        {
            Debug.LogWarning($"Craft: EquipmentData 를 찾을 수 없습니다. dataId={equipmentDataId}");
            return null;
        }

        if (!CanCraft(data))
        {
            Debug.LogWarning($"Craft: 제작 조건을 만족하지 않습니다. dataId={equipmentDataId}");
            return null;
        }

        if (data.TryGetRequiredMaterials(out (string ItemId, int Count)[] materials))
        {
            foreach ((string itemId, int count) in materials)
            {
                _inventory.TryConsumeItem(itemId, count);
            }
        }

        _inventory.TryConsumeGold(data.Gold);

        RolledStats rolled = EquipmentCalculator.Roll(data);
        return _inventory.CreateEquipment(data, rolled);
    }
}