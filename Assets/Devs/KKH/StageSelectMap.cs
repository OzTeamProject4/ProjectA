using System.Collections.Generic;
using UnityEngine;

public class StageSelectMap : MonoBehaviour
{
    [SerializeField] private MonsterPartySpawner[] _spawners;
    [SerializeField] private Transform _playerSpawnPoint;

    public IReadOnlyList<MonsterPartySpawner> Spawners
    {
        get { return _spawners; }
    }

    public Transform PlayerSpawnPoint
    {
        get { return _playerSpawnPoint; }
    }
}