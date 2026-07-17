using System.Collections.Generic;
using UnityEngine;

public class StageSelectMap : MonoBehaviour
{
    [SerializeField] private MonsterPartySpawner[] _spawners;

    public IReadOnlyList<MonsterPartySpawner> Spawners
    {
        get { return _spawners; }
    }
}