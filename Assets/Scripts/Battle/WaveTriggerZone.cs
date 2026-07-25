using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider))]
public class WaveTriggerZone : MonoBehaviour
{
    [SerializeField] private int _waveIndex;
    [SerializeField] private Transform[] _monsterSpawnPoints;
    [SerializeField] private Transform _bossSpawnPoint;

    [SerializeField] private string _stageWaveDataId = "Stage_001_Wave_1";
    private Transform _spawnTransform;
    private float _randomSpawnRadius = 1.5f;


    private void Start()
    {

    }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;

            TriggerRoutine().Forget();


        }
    }
    private async UniTaskVoid TriggerRoutine()
    {
        await PlayerFind();

        this.gameObject.SetActive(false);
    }
    private async UniTask PlayerFind()
    {
        if (GameManager.Instance.DataManager.TryGetData<StageWaveData>(_stageWaveDataId, out StageWaveData waveData))
        {

            if (waveData.TryGetMonsters(out var monsters))
            {
                foreach (var (monsterId, count) in monsters)
                {
                    Debug.Log($"몬스터 ID: {monsterId}, 소환 수: {count}");

                    for (int i = 0; i < count; i++)
                    {

                        GetRandomSpawnPosition(_randomSpawnRadius);
                        await GameManager.Instance.BattleManager.SpawnEnemyAsync(monsterId, _spawnTransform);
                        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                    }
                }
            }
        }
    }

    private void GetRandomSpawnPosition(float radius)
    {
        _spawnTransform = GetRandomSpawnPoint();
        if (_spawnTransform == null)
            return;

        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 randomPos = _spawnTransform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        if (UnityEngine.AI.NavMesh.SamplePosition(randomPos, out UnityEngine.AI.NavMeshHit hit, radius + 1f, UnityEngine.AI.NavMesh.AllAreas))
        {
            _spawnTransform.position = hit.position;
        }

    }
    private Transform GetRandomSpawnPoint()
    {
        if (_monsterSpawnPoints == null || _monsterSpawnPoints.Length == 0)
        {
            Debug.LogWarning("스폰 포인트 배열이 비어있습니다!");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, _monsterSpawnPoints.Length);

        return _monsterSpawnPoints[randomIndex];
    }
}