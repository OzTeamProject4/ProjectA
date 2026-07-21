using Unity.AppUI.UI;
using UnityEngine;

public class Test_SpawnEnemyButton : MonoBehaviour
{
    [SerializeField] private Button Button;
    [SerializeField] private string _spawnEnemyDataId;
    
    public void OnClickSpawnButton()
    {

        Test_GameObjectManager.Inst.SpawnEnemyAsync(_spawnEnemyDataId).Forget();

    }

    public void OnClickLoadButton()
    {
        Test_GameManager.Inst.PreloadData();

    }
}
