using System.Collections.Generic;
using UnityEngine;

public class BattleMap : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private WaveTriggerZone[] _waveTriggerZones;

    public Transform PlayerSpawnPoint
    {
        get { return _playerSpawnPoint; }
    }

    public IReadOnlyList<WaveTriggerZone> WaveTriggerZones
    {
        get { return _waveTriggerZones; }
    }
}