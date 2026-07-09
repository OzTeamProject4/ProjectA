using System.Collections.Generic;
using UnityEngine;

public class Test_GameObjectManager : MonoBehaviour
{
    public static Test_GameObjectManager inst { get; set; }

    [SerializeField] private GameObject _prefab_LocalEnemyView;
    [SerializeField] private Transform _root_enemy;

    private Dictionary<int, GameObject> _spawmEnemyList = new Dictionary<int, GameObject>();
    private int _spawnEnemyInstanceId;

    private void Awake()
    {
        inst = this;
    }

    public void SpawmEnemy(EnemyViewModel vm)
    {
        var createdObj = Instantiate(_prefab_LocalEnemyView, _root_enemy.transform);

        _spawmEnemyList.Add(_spawnEnemyInstanceId, createdObj);
        _spawnEnemyInstanceId++;

        var enemyView = createdObj.GetComponent<EnemyView>();

        if (enemyView != null)
        {
            enemyView.BindEnemyViewModel(vm);
        }
    }
}
