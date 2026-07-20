using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private readonly Dictionary<string, int> _itemCounts = new();
    private readonly Dictionary<string, EquipmentInstance> _equipment = new();

    private int _instanceCounter;

    // TODO: PlayerModel(유저 계정 상태) 도입 시 골드를 그쪽으로 이관.
    public int Gold { get; private set; }

    public event Action OnItemChanged;
    public event Action OnEquipmentChanged;
    public event Action<EquipmentInstance> OnEquipmentCreated;
    public event Action OnGoldChanged;

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[Inventory:AddGold] 유효하지 않은 수량({amount}).");
            return;
        }

        Gold += amount;
        OnGoldChanged?.Invoke();
    }

    public bool TryConsumeGold(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"[Inventory:TryConsumeGold] 유효하지 않은 수량({amount}).");
            return false;
        }

        if (Gold < amount)
        {
            return false;
        }

        Gold -= amount;
        OnGoldChanged?.Invoke();
        return true;
    }

    public int GetItemCount(string dataId)
    {
        if (string.IsNullOrEmpty(dataId))
        {
            return 0;
        }

        return _itemCounts.TryGetValue(dataId, out int count) ? count : 0;
    }

    public void AddItem(string dataId, int count)
    {
        if (string.IsNullOrEmpty(dataId) || count <= 0)
        {
            Debug.LogWarning($"[Inventory:AddItem] 유효하지 않은 입력. dataId={dataId}, count={count}");
            return;
        }

        _itemCounts[dataId] = GetItemCount(dataId) + count;
        OnItemChanged?.Invoke();
    }

    public bool TryConsumeItem(string dataId, int count = 1)
    {
        if (count <= 0)
        {
            Debug.LogWarning($"[Inventory:TryConsumeItem] 유효하지 않은 수량({count}). dataId={dataId}");
            return false;
        }

        if (GetItemCount(dataId) < count)
        {
            return false;
        }

        _itemCounts[dataId] = GetItemCount(dataId) - count;
        OnItemChanged?.Invoke();
        return true;
    }

    public bool HasEquipment(string dataId)
    {
        if (string.IsNullOrEmpty(dataId))
        {
            return false;
        }

        foreach (EquipmentInstance instance in _equipment.Values)
        {
            if (instance.Data.DataId == dataId)
            {
                return true;
            }
        }

        return false;
    }

    public EquipmentInstance CreateEquipment(EquipmentData data, RolledStats rolledStat)
    {
        if (null == data)
        {
            Debug.LogError("[Inventory:CreateEquipment] data 가 null 입니다.");
            return null;
        }

        _instanceCounter++;
        string instanceId = $"equip_{_instanceCounter:D4}";

        EquipmentInstance instance = new EquipmentInstance(instanceId, data, rolledStat);

        _equipment[instanceId] = instance;
        OnEquipmentCreated?.Invoke(instance);
        OnEquipmentChanged?.Invoke();
        return instance;
    }

    public EquipmentInstance GetEquipment(string instanceId)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            return null;
        }

        return _equipment.TryGetValue(instanceId, out EquipmentInstance instance) ? instance : null;
    }

    public IReadOnlyList<EquipmentInstance> GetEquipmentByType(EquipType type)
    {
        List<EquipmentInstance> result = new();

        foreach (EquipmentInstance instance in _equipment.Values)
        {
            if (instance.Type == type)
            {
                result.Add(instance);
            }
        }

        return result;
    }

    public IReadOnlyList<EquipmentInstance> GetEquippedItems(string characterId)
    {
        List<EquipmentInstance> result = new();

        if (string.IsNullOrEmpty(characterId))
        {
            return result;
        }

        foreach (EquipmentInstance instance in _equipment.Values)
        {
            if (instance.EquippedBy == characterId)
            {
                result.Add(instance);
            }
        }

        return result;
    }

    public EquipmentInstance GetEquippedItem(string characterId, EquipType type)
    {
        if (string.IsNullOrEmpty(characterId))
        {
            return null;
        }

        foreach (EquipmentInstance instance in _equipment.Values)
        {
            if (instance.Type == type && instance.EquippedBy == characterId)
            {
                return instance;
            }
        }

        return null;
    }

    // ===== 세이브 로드 전용 =====

    public void RestoreGold(int amount)
    {
        Gold = amount;
    }

    public void RestoreItemCount(string dataId, int count)
    {
        if (string.IsNullOrEmpty(dataId) || count < 0)
        {
            Debug.LogWarning($"[Inventory:RestoreItemCount] 유효하지 않은 입력. dataId={dataId}, count={count}");
            return;
        }

        _itemCounts[dataId] = count;
    }

    public void RestoreEquipment(string instanceId, EquipmentData data, RolledStats rolledStat, string equippedBy)
    {
        if (string.IsNullOrEmpty(instanceId) || null == data)
        {
            Debug.LogWarning($"[Inventory:RestoreEquipment] 유효하지 않은 입력. instanceId={instanceId}");
            return;
        }

        EquipmentInstance instance = new EquipmentInstance(instanceId, data, rolledStat);

        if (!string.IsNullOrEmpty(equippedBy))
        {
            instance.SetEquippedBy(equippedBy);
        }

        _equipment[instanceId] = instance;

        SyncInstanceCounter(instanceId);
    }

    // 인스턴스 Id 충돌 방지용 Sync
    private void SyncInstanceCounter(string instanceId)
    {
        const string prefix = "equip_";

        if (!instanceId.StartsWith(prefix))
        {
            return;
        }

        string numberPart = instanceId.Substring(prefix.Length);

        if (int.TryParse(numberPart, out int number) && number > _instanceCounter)
        {
            _instanceCounter = number;
        }
    }

    public IReadOnlyList<EquipmentInstance> GetAllEquipment()
    {
        return new List<EquipmentInstance>(_equipment.Values);
    }

    public IReadOnlyList<string> GetAllItemDataIds()
    {
        return new List<string>(_itemCounts.Keys);
    }
}