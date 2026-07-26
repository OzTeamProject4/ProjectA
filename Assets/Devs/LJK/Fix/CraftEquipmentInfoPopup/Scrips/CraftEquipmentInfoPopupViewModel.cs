using System.Collections.Generic;
using UnityEngine;

public class CraftEquipmentInfoPopupViewModel
{
    private EquipmentCraftModel _equipmentCraftModel;

    public string Name
    {
        get { return _equipmentCraftModel.Name; }
    }

    public string IconKey
    {
        get { return _equipmentCraftModel.IconKey; }
    }

    public IReadOnlyList<StatDelta> CreateStatDeltas()
    {
        List<StatDelta> statDeltas = new List<StatDelta>();

        if (_equipmentCraftModel == null || _equipmentCraftModel.StatInfos == null)
        {
            return statDeltas;
        }

        foreach (StatInfo statInfo in _equipmentCraftModel.StatInfos)
        {
            if (Mathf.Approximately(statInfo.Value, 0f))
            {
                continue;
            }

            statDeltas.Add(new StatDelta(statInfo.Type, statInfo.Value, 0f, false));
        }

        return statDeltas;
    }

    public void SetModel(EquipmentCraftModel equipmentCraftModel)
    {
        _equipmentCraftModel = equipmentCraftModel;
    }

    public void Dispose()
    {
        _equipmentCraftModel = null;
    }
}