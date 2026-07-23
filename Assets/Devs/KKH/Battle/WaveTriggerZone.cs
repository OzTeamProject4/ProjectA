using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WaveTriggerZone : MonoBehaviour
{
    [SerializeField] private int _waveIndex;
    [SerializeField] private Transform[] _monsterSpawnPoints;
    [SerializeField] private Transform _bossSpawnPoint;

    [SerializeField] private string _stageWaveDataId = "Stage_001_Wave_1";



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
                        //GameManager.Instance.BattleManager.SpawnEnemyById(monsterId, GetRandomSpawnPoint());
                        
                         await GameManager.Instance.BattleManager.SpawnEnemyAsync(monsterId, GetRandomSpawnPoint());
                        await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
                    }
                }
            }
        }

    }
    public Transform GetRandomSpawnPoint()
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