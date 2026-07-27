using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EquipmentInfoPopupViewModel
{
    private StudentModel _studentModel;
    private EquipmentModel _equipmentModel;

    public string Name
    {
        get
        {
            return _equipmentModel.Name;
        }
    }

    public string Description
    {
        get
        {
            return _equipmentModel.Description;
        }
    }

    public string IconKey
    {
        get
        {
            return _equipmentModel.IconKey;
        }
    }

    public event Action<string> PropertyChanged;

    private bool HasStatComparison
    {
        get
        {
            if (_studentModel == null || _equipmentModel == null)
            {
                return false;
            }

            return !IsEquippedByCurrentStudent;
        }
    }

    public bool IsEquippedByCurrentStudent
    {
        get
        {
            if (_studentModel == null || _equipmentModel == null)
            {
                return false;
            }

            return _equipmentModel.EquippedBy == _studentModel.DataId;
        }
    }

    public IReadOnlyList<StatDelta> CreateStatDeltas()
    {
        List<StatDelta> statDeltas = new List<StatDelta>();

        if (_equipmentModel == null || _equipmentModel.StatInfos == null)
        {
            return statDeltas;
        }

        bool hasComparison = HasStatComparison;
        IReadOnlyList<StatInfo> equippedStatInfos = GetEquippedStatInfos();

        foreach (StatInfo statInfo in _equipmentModel.StatInfos)
        {
            float equippedValue = FindStatValue(equippedStatInfos, statInfo.Type);
            float delta = statInfo.Value - equippedValue;

            if (Mathf.Approximately(statInfo.Value, 0f) && Mathf.Approximately(delta, 0f))
            {
                continue;
            }

            statDeltas.Add(new StatDelta(statInfo.Type, statInfo.Value, delta, hasComparison));
        }

        return statDeltas;
    }

    private IReadOnlyList<StatInfo> GetEquippedStatInfos()
    {
        if (!HasStatComparison)
        {
            return null;
        }

        if (!_studentModel.TryGetEquippedItemId(_equipmentModel.EquipType, out string instanceId))
        {
            return null;
        }

        if (!NetworkManagerTemp.Instance.InventoryModel.TryGetEquipment(instanceId, out EquipmentModel equippedModel))
        {
            return null;
        }

        return equippedModel.StatInfos;
    }

    private static float FindStatValue(IReadOnlyList<StatInfo> statInfos, StatType statType)
    {
        if (statInfos == null)
        {
            return 0f;
        }

        foreach (StatInfo statInfo in statInfos)
        {
            if (statInfo.Type == statType)
            {
                return statInfo.Value;
            }
        }

        return 0f;
    }

    public void SetModel(EquipmentModel equipmentModel, StudentModel studentModel)
    {
        if (_equipmentModel != null)
        {
            _equipmentModel.PropertyChanged -= OnModelPropertyChanged;
        }

        _studentModel = studentModel;
        _equipmentModel = equipmentModel;
        _equipmentModel.PropertyChanged += OnModelPropertyChanged;
    }

    public void Dispose()
    {
        if (_equipmentModel == null)
        {
            return;
        }

        _equipmentModel.PropertyChanged -= OnModelPropertyChanged;
        _equipmentModel = null;
    }

    private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(e.PropertyName);
    }

    public void RequestEquip()
    {
        if (_studentModel == null || _equipmentModel == null)
        {
            return;
        }

        NetworkManagerTemp.Instance.InventoryModel.TryEquip(_studentModel, _equipmentModel.InstanceId);
    }

    public void RequestUnequip()
    {
        if (_studentModel == null || _equipmentModel == null)
        {
            return;
        }

        if (!IsEquippedByCurrentStudent)
        {
            return;
        }

        NetworkManagerTemp.Instance.InventoryModel.TryUnequip(_studentModel, _equipmentModel.EquipType);
    }
}
