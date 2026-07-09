using UnityEngine;

public class Test_GameObjectManager : MonoBehaviour
{
    public static Test_GameObjectManager inst { get; set; }

    [SerializeField] private GameObject _prefab_LocalEnemyView;
    [SerializeField] private Transform _root_enemy;

    private void Awake()
    {
        inst = this;
    }

    public void CreateEnemy(EnemyViewModel vm)
    {
        var createdObj = Instantiate(_prefab_LocalEnemyView, _root_enemy.transform);

        var enemyView = createdObj.GetComponent<EnemyView>();

        if (enemyView != null)
        {
            enemyView.BindEnemyViewModel(vm);
        }
    }
}
