using System.Collections.Generic;
using UnityEngine;

public class BattleMap : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private WaveTriggerZone[] _waveTriggerZones;
    [SerializeField] private Transform _cameraAnchor;

    public Transform PlayerSpawnPoint
    {
        get { return _playerSpawnPoint; }
    }

    public IReadOnlyList<WaveTriggerZone> WaveTriggerZones
    {
        get { return _waveTriggerZones; }
    }

    public Transform CameraAnchor
    {
        get { return _cameraAnchor; }
    }
}