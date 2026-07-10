using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class Test_GameObjectManager : MonoBehaviour
{
    public static Test_GameObjectManager Inst { get; set; }

    [SerializeField] private GameObject _prefab_LocalEnemyView;
    [SerializeField] private Transform _root_enemy;

    private Dictionary<int, GameObject> _spawnEnemyList = new Dictionary<int, GameObject>();
    private int _spawnEnemyInstanceId;

    private void Awake()
    {
        Inst = this;
    }

    public async UniTaskVoid SpawnEnemyAsync(EnemyViewModel vm)
    {
        
        GameObject prefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(vm.PrefabAddress);
        if (prefab == null)
        {
            Debug.LogError("적 프리팹을 로드하지 못했습니다.");
            return;
        }
        GameObject enemyInstance = Instantiate(prefab);
        _spawnEnemyList.Add(_spawnEnemyInstanceId, enemyInstance);
        _spawnEnemyInstanceId++;

        enemyInstance.gameObject.transform.position = _root_enemy.position;

        if (enemyInstance.TryGetComponent<EnemyView>(out var enemyView))
        {
            enemyView.BindEnemyViewModel(vm);
        }
        else
        {
            Debug.LogError("생성된 에셋에 EnemyView 컴포넌트가 없습니다.");
        }
        
    }
}
