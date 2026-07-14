using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class Test_GameManager : MonoBehaviour
{
    public static Test_GameManager Inst { get; set; }

    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {


    }
    public void RequestCreateEnemy(string enemyDataId)
    {

        Test_GameObjectManager.Inst.SpawnEnemyAsync(enemyDataId).Forget();
    }
    public void  PreloadData()
    {
        GameManager.Instance.DataManager.PreloadDataAsync().Forget();
    }
    


}