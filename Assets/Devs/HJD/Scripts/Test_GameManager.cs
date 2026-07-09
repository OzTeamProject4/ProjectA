using UnityEngine;

public class Test_GameManager : MonoBehaviour
{
    public static Test_GameManager Inst { get; set; }

    public EnemyService EnemyService { get; private set; }

    private void Awake()
    {
        Inst = this;

        EnemyService = new EnemyService();

    }
    private void Start()
    {
        RequestCreateEnemy("데이터 ID가 들어갈 곳");


    }
    public void RequestCreateEnemy(string enemyDataId)
    {
        EnemyViewModel localEnemyVm = EnemyService.CreateEnemyViewModel(enemyDataId);

        if (localEnemyVm == null) {
            Debug.Log("localEnemyVm 이 비었습니다.");
            return;
        }
        Test_GameObjectManager.inst.SpawmEnemy(localEnemyVm);
    }

}