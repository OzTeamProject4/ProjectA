using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class EnemySpawn : MonoBehaviour
{
    // 적 Spawn 장소 등록
    [SerializeField] private GameObject[] enemySpawnPoint;

    // 적 오브젝트 프리팹 넣기
    [SerializeField] private AssetReference enemyPrefab;

    // 적 생성량 조절
    [SerializeField] private int testEnemyCount = 5;

 

    // 전투 상황에 최대로 존재할 수 있는 적 오브젝트의 개수
    [SerializeField] private int maxActiveEnemyCount = 10;

    // 생성된 적 오브젝트는 enemyParent 산하에 배치되도록 설정
    [SerializeField] private Transform enemyParent;

    // 생성 간격 설정
    [SerializeField] private float spawnInterval = 3f;

    // Spawn시 땅에 박히는 경우 이 값을 올려주세요.
    [SerializeField] private float spawnHeightOffset = 2f;

    // 전투가 끝났는가?
    [SerializeField] private UnityEvent onBattleEnd;

    // 오브젝트 풀링 사용
    private readonly Queue<GameObject> enemyPool = new Queue<GameObject>();

    // 현재 active 상태인 enemy
    private readonly List<GameObject> activeEnemies = new List<GameObject>();


    private int spawnedTotalCount;


    private void Start()
    {
        SpawnTestEnemies(this.GetCancellationTokenOnDestroy()).Forget();
    }

    public async UniTask SpawnTestEnemies(CancellationToken token)
    {

        // 스폰 지점 없음
        if (enemySpawnPoint == null || enemySpawnPoint.Length == 0)
        {
            Debug.LogError("Enemy Spawn Point가 없습니다.");
            return;
        }

        // 적 프리팹 없음
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab이 없습니다.");
            return;
        }

        // 프리팹 변수 생성
        GameObject prefab;


        try
        {
            // 프리팹을 불러와서 저장
            prefab = await enemyPrefab
                   .LoadAssetAsync<GameObject>() // GameObject 타입 에셋을 비동기로 로드
                   .ToUniTask(cancellationToken: token); // Addressables 로딩 작업을 UniTask 방식으로
        }

        catch
        {
            // 로딩 실패
            Debug.LogError("프리팹 로딩 실패");
            return;
        }


        // 함수 실행
        CreateEnemyPool(prefab);

        while (!token.IsCancellationRequested) 
        {
            for (int i = activeEnemies.Count - 1; i >= 0; i--) 
            // 뒤에서 부터 비활성화&활성화
            // 앞에서 부터 작업시 순서 꼬임
            {
                GameObject enemy = activeEnemies[i];


                // enemy가 없거나 비활성화라면?
                if (enemy == null || !enemy.activeSelf)
                {
                    activeEnemies.RemoveAt(i); // activeEnemies 리스트에서 제거
                }
            }

            if (spawnedTotalCount >= testEnemyCount && activeEnemies.Count == 0)
            {
                Debug.Log("전투 종료");
                onBattleEnd?.Invoke();
                return;
            }

            if (spawnedTotalCount < testEnemyCount &&
                activeEnemies.Count < maxActiveEnemyCount)
            {
                SpawnEnemyFromPool();
                spawnedTotalCount++;
            }

            await UniTask.Delay(
                System.TimeSpan.FromSeconds(spawnInterval),
                cancellationToken: token
            );
        }
    }

    private void CreateEnemyPool(GameObject prefab)  // 최대치 만큼만 생성해서 enemypool에 넣기
    {
        for (int i = 0; i < maxActiveEnemyCount; i++)
        {
            GameObject enemy = Instantiate(prefab, enemyParent);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy); // Enquque -> 맨 뒤에 넣기
        }
    }

    private void SpawnEnemyFromPool()
    {
        if (enemyPool.Count == 0)
            return;

        GameObject enemy = enemyPool.Dequeue();

        GameObject point = enemySpawnPoint[spawnedTotalCount % enemySpawnPoint.Length];

        enemy.transform.position = point.transform.position + Vector3.up * spawnHeightOffset;
        enemy.transform.rotation = point.transform.rotation;

        enemy.SetActive(true);
        activeEnemies.Add(enemy);
    }

    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy == null)
            return;

        enemy.SetActive(false);
        activeEnemies.Remove(enemy);
        enemyPool.Enqueue(enemy);
    }
}

