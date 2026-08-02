using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class BattleMap : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private CinemachineCamera _battleCamera;
    [SerializeField] private WaveTriggerZone[] _waveTriggerZones;
    private bool _victoryRequested;

    private void Awake()
    {
        _waveTriggerZones =
            GetComponentsInChildren<WaveTriggerZone>(true);
    }
    private void Update()
    {
        if (_victoryRequested)
        {
            return;
        }

        if (!AreAllWavesSpawnCompleted)
        {
            return;
        }

        if (GameObject.FindGameObjectWithTag("Enemy") != null)
        {
            return;
        }

        _victoryRequested = true;

        GameManager.Instance.BattleManager.EndBattle(true);
    }

    private bool AreAllWavesSpawnCompleted
    {
        get
        {
            if (_waveTriggerZones == null ||
                _waveTriggerZones.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < _waveTriggerZones.Length; i++)
            {
                WaveTriggerZone wave = _waveTriggerZones[i];

                if (wave == null || !wave.IsSpawnCompleted)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public Transform PlayerSpawnPoint
    {
        get { return _playerSpawnPoint; }
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