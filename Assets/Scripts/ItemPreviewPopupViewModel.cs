using System.Collections.Generic;
using UnityEngine;

public class ItemPreviewPopupViewModel
{
    private readonly EquipmentData _data;

    public string Name
    {
        get { return _data.Name; }
    }

    public string Description
    {
        get { return _data.Description; }
    }

    public string SpritePath
    {
        get { return _data.SpritePath; }
    }

    public ItemPreviewPopupViewModel(EquipmentData data)
    {
        if (null == data)
        {
            Debug.LogError("ItemPreviewPopupViewModel: data 가 null 입니다.");
        }

        _data = data;
    }

    public IReadOnlyList<StatValue> GetStatValues()
    {
        List<StatValue> values = new();

        AddStat(values, StatType.MaxHp, _data.MaxHp, true);
        AddStat(values, StatType.Atk, _data.Atk, true);
        AddStat(values, StatType.Def, _data.Def, true);
        AddStat(values, StatType.MoveSpeed, _data.MoveSpeed, false);

        return values;
    }

    private void AddStat(List<StatValue> values, StatType type, float value, bool isInteger)
    {
        if (Mathf.Approximately(value, 0f))
        {
            return;
        }

        values.Add(new StatValue
        {
            Type = type,
            Value = value,
            IsInteger = isInteger
        });
    }
}