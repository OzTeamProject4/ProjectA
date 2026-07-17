using System;
using UnityEngine;

public class StageMonsterParty : MonoBehaviour
{
    private string _stageId;

    public string StageId
    {
        get { return _stageId; }
    }

    public event Action<string> OnPlayerReached;

    public void SetStageId(string stageId)
    {
        _stageId = stageId;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out StagePlayerParty player))
        {
            return;
        }

        if (string.IsNullOrEmpty(_stageId))
        {
            Debug.LogWarning($"[StageMonsterParty] _stageId 가 비어 있습니다. name={name}");
            return;
        }

        OnPlayerReached?.Invoke(_stageId);
    }
}