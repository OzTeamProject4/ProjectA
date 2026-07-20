using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class EnemySpawnTest : MonoBehaviour
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

    // 전투 종료 이후 발생할 이벤트를 하이어라키에서 넣어주세요.
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
            // 첫 실행 시 for 문 실행 X -> activeEnemeies.Count = 0
            {
                GameObject enemy = activeEnemies[i];


                // enemy가 없음
                if (enemy == null)
                {
                    activeEnemies.RemoveAt(i);
                    continue;
                }


                // enemy가 비활성화
                if (!enemy.activeSelf)
                {
                    activeEnemies.RemoveAt(i);
                    enemyPool.Enqueue(enemy);
                }
            }

            if (spawnedTotalCount >= testEnemyCount && activeEnemies.Count == 0)
            {
                Debug.Log("전투 종료");
                onBattleEnd?.Invoke();
                return;
            }


            // 소환한 적 오브젝트의 수가 목표 소환 수에 도달했는지 확인
            // 현재 살아있는 적 오브젝트의 수가 최대 동시 활성화 수 보다 적은지 확인
            if (spawnedTotalCount < testEnemyCount && activeEnemies.Count < maxActiveEnemyCount)
            {
                SpawnEnemyFromPool(token);
                spawnedTotalCount++;
            }

            await UniTask.Delay(
                System.TimeSpan.FromSeconds(spawnInterval),
                cancellationToken: token
            );
        }
    }

    private void CreateEnemyPool(GameObject prefab) // 최대 활성화 수만큼 적을 미리 생성
    {
        for (int i = 0; i < maxActiveEnemyCount; i++)
        {
            GameObject enemy = Instantiate(prefab, enemyParent); // 적 오브젝트 생성
            enemy.SetActive(false); // 생성된 적 비활성화
            enemyPool.Enqueue(enemy); // 풀의 맨 뒤에 적 추가
        }
    }

    private void SpawnEnemyFromPool(CancellationToken token)
    {
        if (enemyPool.Count == 0)
            return;

        GameObject enemy = enemyPool.Dequeue();

        GameObject point = enemySpawnPoint[spawnedTotalCount % enemySpawnPoint.Length];

        enemy.transform.position =
            point.transform.position + Vector3.up * spawnHeightOffset;

        enemy.transform.rotation = point.transform.rotation;

        enemy.SetActive(true);
        activeEnemies.Add(enemy);

        Debug.Log($"적 활성화: {enemy.name}, ID: {enemy.GetInstanceID()}");

        // 테스트용: 활성화 후 2초 뒤 자동 비활성화
        DisableEnemyForTest(enemy, token).Forget();
    }
    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy == null) // 반환할 적이 없으면 종료
            return;

        enemy.SetActive(false); // 적 비활성화
        activeEnemies.Remove(enemy); // 활성화된 적 목록에서 제거
        enemyPool.Enqueue(enemy); // 적을 풀의 맨 뒤로 반환
    }

    public async UniTask DisableEnemyForTest(GameObject enemy, CancellationToken token)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(2f), cancellationToken: token);

        if (enemy == null || !enemy.activeSelf)
        {
            return;
        }

        Debug.Log($"비활성화 테스트: {enemy.name}");
        enemy.SetActive(false);
    }

    /*
    public void Initialize(StageData stageData)
    {
      스테이지의 적 정보를 받는 함수  
    }
    */
}

