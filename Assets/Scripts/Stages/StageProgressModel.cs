using System;
using System.Collections.Generic;
using UnityEngine;

public class StageProgressModel
{
    private readonly HashSet<string> _clearedSet = new HashSet<string>();

    public event Action<string> OnStageCleared;

    public string SelectedStageId { get; private set; }
    public Vector3 PlayerPosition { get; private set; }

    private readonly List<string> _selectedPartyIds = new List<string>();

    public IReadOnlyList<string> SelectedPartyIds
    {
        get { return _selectedPartyIds; }
    }

    public void SelectStage(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning("[StageProgressModel] SelectStage: stageId 가 비어 있습니다.");
            return;
        }

        SelectedStageId = stageId;
    }

    public void SetSelectedPartyIds(IReadOnlyList<string> partyIds)
    {
        _selectedPartyIds.Clear();

        if (null == partyIds)
        {
            return;
        }

        foreach (string partyId in partyIds)
        {
            if (string.IsNullOrEmpty(partyId))
            {
                continue;
            }

            _selectedPartyIds.Add(partyId);
        }
    }

    public void SetPlayerPosition(Vector3 position)
    {
        PlayerPosition = position;
    }

    public bool IsCleared(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            return false;
        }

        return _clearedSet.Contains(stageId);
    }

    public void AddCleared(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning("[StageProgressModel] AddCleared: stageId 가 비어 있습니다.");
            return;
        }

        if (!_clearedSet.Add(stageId))
        {
            return;
        }

        OnStageCleared?.Invoke(stageId);
    }
}
