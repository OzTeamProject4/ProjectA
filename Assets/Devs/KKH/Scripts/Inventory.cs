using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private readonly Dictionary<string, int> _itemCounts = new();
    private readonly Dictionary<string, EquipmentInstance> _equipment = new();

    private int _instanceCounter;

    public event Action OnItemChanged;
    public event Action OnEquipmentChanged;

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

    public EquipmentInstance CreateEquipment(EquipmentData data,
        float bonusHp, float bonusAtk, float bonusDef, float bonusAtkSpeed, float bonusMovement)
    {
        if (null == data)
        {
            Debug.LogError("[Inventory:CreateEquipment] data 가 null 입니다.");
            return null;
        }

        _instanceCounter++;
        string instanceId = $"equip_{_instanceCounter:D4}";

        EquipmentInstance instance = new EquipmentInstance(
            instanceId, data, bonusHp, bonusAtk, bonusDef, bonusAtkSpeed, bonusMovement);

        _equipment[instanceId] = instance;
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
}