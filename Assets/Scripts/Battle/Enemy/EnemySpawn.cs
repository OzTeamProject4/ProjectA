using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

[Serializable]
public class EnemySpawnData
{
    public string enemyId;
    public int spawnCount;
    public int maxActiveEnemyCount;
    public float spawnInterval;
}

public class EnemySpawn : MonoBehaviour
{
    private const string EnemyDataJsonAddress = "EnemyData";
    // Enemy Data가 있는 JSON 파일의 어드레서블 주소를 작성해주세요

    [SerializeField] private GameObject[] enemySpawnPoint;
    // 스폰될 장소
    
    [SerializeField] private Transform enemyParent;
    // 적 오브젝트가 enemyParent 안에서 동적 생성 됩니다.
    
    [SerializeField] private float spawnHeightOffset;
    // 오브젝트가 땅에 박히면 해당 값을 올려주세요.

    [SerializeField] private UnityEvent onBattleEnd;


    private readonly Queue<GameObject> enemyPool = new Queue<GameObject>();
    // 오브젝트 풀링을 위한 enemyPool Queue 선언
    
    private readonly List<GameObject> activeEnemies = new List<GameObject>();
    // 오브젝트 풀링을 위한 액티브 enemy 리스트 선언
    
    private readonly EnemyDataRepository enemyDataRepository = new EnemyDataRepository(EnemyDataJsonAddress);
    // JSON을 읽을 준비


    // [SerializeField] private int spawnCount;

    // [SerializeField] private int maxActiveEnemyCount;

    // [SerializeField] private float spawnInterval;

    // [SerializeField] private string enemyId;

    private int spawnedTotalCount;


    // 테스트용 임시 코드
    private void Start()
    {
        EnemySpawnData testData = new EnemySpawnData
        {
            enemyId = "Enemy_Test_01",
            spawnCount = 3,
            maxActiveEnemyCount = 1,
            spawnInterval = 1f
        };

        SpawnEnemies(
            testData,
            this.GetCancellationTokenOnDestroy()
        ).Forget();
    }


    // 적 오브젝트 생성
    public async UniTask SpawnEnemies(EnemySpawnData data, CancellationToken token)
    {

        // SpawnData가 없으면?
        if (data == null)
        {
            Debug.LogError("EnemySpawnData is missing.");
            return;
        }


        // SpawnData.enemyId가 없으면?
        if (string.IsNullOrWhiteSpace(data.enemyId))
        {
            Debug.LogError("Enemy data ID is missing.");
            return;
        }

        // 스폰 포인트가 없으면?
        if (enemySpawnPoint == null || enemySpawnPoint.Length == 0)
        {
            Debug.LogError("Enemy spawn point is missing.");
            return;
        }
        
        // enemyParent가 없으면?
        if (enemyParent == null)
        {
            Debug.LogError("Enemy parent is missing.");
            return;
        }


        // maxActiveEnemyCount 가 0 보다 작게 세팅이 되어 있으면?
        if (data.maxActiveEnemyCount <= 0)
        {
            Debug.LogError("Max active enemy count must be greater than zero.");
            return;
        }

        // 스폰 간격이 0f보다 낮게 설정되어 있으면?
        if (data.spawnInterval < 0f)
        {
            Debug.LogError("Spawn interval cannot be negative.");
            return;
        }


        /// 이전 전투 내역 제거
        ClearPreviousBattle();


        EnemyData enemyData;


        try
        {
            enemyData = await enemyDataRepository.GetByIdAsync(data.enemyId, token);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to load enemy data: {exception.Message}");
            return;
        }
        
        // enemyID가 없다면?
        if (enemyData == null)
        {
            Debug.LogError($"Enemy data was not found: {data.enemyId}");
            return;
        }

        // PrefabAddress가 없다면?
        if (string.IsNullOrWhiteSpace(enemyData.PrefabAddress))
        {
            Debug.LogError($"Enemy prefab address is missing: {data.enemyId}");
            return;
        }

        GameObject prefab;

        // 프리팹 로드
        try
        {
            prefab = await Addressables
                .LoadAssetAsync<GameObject>(enemyData.PrefabAddress)
                .ToUniTask(cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to load enemy prefab: {exception.Message}");
            return;
        }


        // 적 오브젝트 풀 생성
        CreateEnemyPool(prefab, data.maxActiveEnemyCount);



        while (!token.IsCancellationRequested)
        {

            // 승리 조건
            if (spawnedTotalCount >= data.spawnCount && activeEnemies.Count == 0)
            {
                onBattleEnd?.Invoke();
                return;
            }


            // 오브젝트 풀링 꺼내기 조건
            if (spawnedTotalCount < data.spawnCount && activeEnemies.Count < data.maxActiveEnemyCount)
            {
                SpawnEnemyFromPool(enemyData);
                spawnedTotalCount++;
            }

            await UniTask.Delay(
                TimeSpan.FromSeconds(data.spawnInterval),
                cancellationToken: token
            );
        }
    }


    // 이전 전투 내역 제거
    private void ClearPreviousBattle()
    {
        foreach (Transform child in enemyParent)
        {
            Destroy(child.gameObject);
        }

        enemyPool.Clear();
        activeEnemies.Clear();
        spawnedTotalCount = 0;
    }


    // 적 오브젝트 풀 생성
    private void CreateEnemyPool(GameObject prefab, int maxActiveEnemyCount)
    {
        for (int i = 0; i < maxActiveEnemyCount; i++)
        {
            GameObject enemy = Instantiate(prefab, enemyParent);
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                Debug.LogError("Enemy prefab is missing EnemyHealth.");
                Destroy(enemy);
                continue;
            }

            enemyHealth.onDead += ReturnEnemy;
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }



    // 적 오브젝트 풀링 소환
    private void SpawnEnemyFromPool(EnemyData enemyData)
    {
        if (enemyPool.Count == 0)
        {
            return;
        }

        GameObject enemy = enemyPool.Dequeue();
        GameObject point = enemySpawnPoint[spawnedTotalCount % enemySpawnPoint.Length];
        // 포인트 순회하면서 소환

        enemy.transform.position = point.transform.position + Vector3.up * spawnHeightOffset;
        enemy.transform.rotation = point.transform.rotation;

        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            Debug.LogError("Enemy prefab is missing EnemyHealth.");
            enemyPool.Enqueue(enemy);
            return;
        }


        // EnemyViewModel enemyViewModel = new EnemyViewModel(enemyData);
        // JSON의 enemyData를 이용해 새 전투 상태를 만듬 
        
        // enemyHealth.Bind(enemyViewModel);
        // 전투 상태를 적 프리팹의 EnemyHealth에 연결

        enemy.SetActive(true);
        activeEnemies.Add(enemy);
    }

    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy == null || !activeEnemies.Remove(enemy))
        {
            return;
        }

        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
}
