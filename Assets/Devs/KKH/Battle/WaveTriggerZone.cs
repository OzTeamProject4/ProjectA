using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WaveTriggerZone : MonoBehaviour
{
    [SerializeField] private int _waveIndex;
    [SerializeField] private Transform[] _monsterSpawnPoints;
    [SerializeField] private Transform _bossSpawnPoint;

    public int WaveIndex
    {
        get { return _waveIndex; }
    }

    public IReadOnlyList<Transform> MonsterSpawnPoints
    {
        get { return _monsterSpawnPoints; }
    }

    public Transform BossSpawnPoint
    {
        get { return _bossSpawnPoint; }
    }
}