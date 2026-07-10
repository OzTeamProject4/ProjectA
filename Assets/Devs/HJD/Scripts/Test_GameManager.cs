using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
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


    }
    public void RequestCreateEnemy(string enemyDataId)
    {
        EnemyViewModel enemyVm = EnemyService.CreateEnemyViewModel(enemyDataId);

        if (enemyVm == null) {
            Debug.Log("localEnemyVm 이 비었습니다.");
            return;
        }
        Test_GameObjectManager.Inst.SpawnEnemyAsync(enemyVm).Forget();
    }
    public void  PreloadData()
    {
        GameManager.Instance.DataManager.PreloadDataAsync().Forget();
    }
    


}