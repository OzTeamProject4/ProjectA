using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class Test_EnemySpawn : MonoBehaviour
{
    [SerializeField] Transform _transform;
    [SerializeField] string _enemyDataId;

    private void OnEnable()
    {
        EnemySpawn(_enemyDataId).Forget();


    }
    
    private async UniTaskVoid EnemySpawn(string dataId)
    {
        await SpawnEnemyAsync(dataId, _transform);

    }
    public async UniTask SpawnEnemyAsync(string enemyDataId, Transform enemySpawnTransform)
    {
        EnemyViewModel vm = new EnemyViewModel();
        if (GameManager.Instance.DataManager.TryGetData<EnemyData>(enemyDataId, out EnemyData enemyData))
        {

            if (enemyData == null)
            {
                Debug.LogError("적 데이터를 로드하지 못했습니다.");
                return;
            }
            GameObject prefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(enemyData.PrefabAddress);


            if (prefab == null)
            {
                Debug.LogError("적 프리팹을 로드하지 못했습니다.");
                return;
            }
            GameObject enemyInstance = Instantiate(prefab);

            EnemyController enemyController = enemyInstance.GetComponent<EnemyController>();

            enemyController.Bind(enemyData,vm);


            enemyInstance.gameObject.transform.position = enemySpawnTransform.position;

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



    public async UniTask SpawnSkillAsync(string skillDataId, Transform spawnTransform, Transform rotationTransform)
    {

        EnemySkillViewModel vm = new EnemySkillViewModel();
        if (GameManager.Instance.DataManager.TryGetData<EnemySkillData>(skillDataId, out EnemySkillData skillData))
        {
            vm.SkillDataId = skillData.DataId;
            vm.Name = skillData.Name;
            vm.ElementalType = skillData.ElementalType;
            vm.BaseDamage = skillData.BaseDamage;
            vm.PrefabAddress = skillData.PrefabAddress;

        }

        GameObject prefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(vm.PrefabAddress);
        if (prefab == null)
        {
            Debug.LogError("스킬 프리팹을 로드하지 못했습니다.");
            return;
        }
        GameObject enemyInstance = Instantiate(prefab);

        var enemySkillController = enemyInstance.GetComponent<EnemySkillController>();
        enemySkillController.vm = vm;



        enemyInstance.transform.position = spawnTransform.position;
        enemyInstance.transform.rotation = rotationTransform.rotation;


    }

}
