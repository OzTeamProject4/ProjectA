using UnityEngine;

public class StageMonsterParty : MonoBehaviour
{
    [SerializeField] private string _stageId;

    public string StageId
    {
        get { return _stageId; }
    }
}