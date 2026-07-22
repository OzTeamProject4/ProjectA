using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class BattleMap : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private CinemachineCamera _battleCamera;
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private WaveTriggerZone[] _waveTriggerZones;

    public Transform PlayerSpawnPoint
    {
        get { return _playerSpawnPoint; }
    }

    public BattleManager BattleManager
    {
        get { return _battleManager; }
    }

    public CinemachineCamera BattleCamera
    {
        get { return _battleCamera; }
    }

    public IReadOnlyList<WaveTriggerZone> WaveTriggerZones
    {
        get { return _waveTriggerZones; }
    }
}