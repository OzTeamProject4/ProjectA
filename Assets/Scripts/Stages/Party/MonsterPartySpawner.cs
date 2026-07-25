using UnityEngine;

public class MonsterPartySpawner : MonoBehaviour
{
    [SerializeField] private string _stageId;

    public string StageId
    {
        get { return _stageId; }
    }

    public Vector3 SpawnPosition
    {
        get { return transform.position; }
    }
}
