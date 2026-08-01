using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class StageClearModel : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs ClearedStageCountChanged = new PropertyChangedEventArgs(nameof(ClearedStageCount));

    private readonly HashSet<string> _clearedStageIdSet = new HashSet<string>();

    public int ClearedStageCount
    {
        get
        {
            return _clearedStageIdSet.Count;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public StageClearModel(IReadOnlyList<string> clearedStageIds)
    {
        if (clearedStageIds == null)
        {
            return;
        }

        foreach (string clearedStageId in clearedStageIds)
        {
            if (string.IsNullOrEmpty(clearedStageId))
            {
                continue;
            }

            _clearedStageIdSet.Add(clearedStageId);
        }
    }

    public bool IsCleared(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            return false;
        }

        return _clearedStageIdSet.Contains(stageId);
    }

    public void AddClearedStage(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning($"[{nameof(StageClearModel)}:{nameof(AddClearedStage)}] stageId가 비어 있습니다.");
            return;
        }

        if (!_clearedStageIdSet.Add(stageId))
        {
            return;
        }

        OnPropertyChanged(ClearedStageCountChanged);
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
