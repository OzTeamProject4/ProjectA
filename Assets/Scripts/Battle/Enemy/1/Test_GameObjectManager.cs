using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class Test_GameObjectManager : MonoBehaviour
{
    public static Test_GameObjectManager Inst { get; set; }

    [SerializeField] private Transform _root_enemy;

    private Dictionary<int, GameObject> _spawnEnemyList = new Dictionary<int, GameObject>();
    private int _spawnEnemyInstanceId;

    private Dictionary<int, GameObject> _spawnSkillList = new Dictionary<int, GameObject>();
    private int _spawnSkillInstanceId;


    private void Awake()
    {
        Inst = this;
    }

    public async UniTaskVoid SpawnEnemyAsync(string enemyDataId)
    {
        EnemyViewModel vm = new EnemyViewModel();
        if (GameManager.Instance.DataManager.TryGetData<EnemyData>(enemyDataId, out EnemyData enemyData)) {
            GameObject prefab = await GameManager.Instance.ResourceManager.LoadAssetAsync<GameObject>(enemyData.PrefabAddress);


            if (prefab == null)
            {
                Debug.LogError("적 프리팹을 로드하지 못했습니다.");
                return;
            }
            GameObject enemyInstance = Instantiate(prefab);

            EnemyController enemyController = enemyInstance.GetComponent<EnemyController>();

            enemyController.Bind(enemyData);


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

    public void DespawnEnemy(int enemyInstanceId)
    {
        if (_spawnEnemyList.TryGetValue(enemyInstanceId, out GameObject enemyInstance))
        {
            _spawnEnemyList.Remove(enemyInstanceId);

            if (enemyInstance != null)
            {
                Destroy(enemyInstance);
            }
        }
        else
        {
            Debug.LogWarning($"[DespawnEnemy] ID {enemyInstanceId}에 해당하는 스킬을 찾을 수 없습니다.");
        }
    }

    public async UniTaskVoid SpawnSkillAsync( string skillDataId,Transform spawnTransform,Transform rotationTransform)
    {
        int currentSkillInstanceId = _spawnSkillInstanceId++;

        EnemySkillViewModel vm = new EnemySkillViewModel();
        if (GameManager.Instance.DataManager.TryGetData<EnemySkillData>(skillDataId, out EnemySkillData skillData))
        {
            vm.SkillDataId = skillData.DataId;
            vm.InstanceId = currentSkillInstanceId;
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

        _spawnSkillList.Add(currentSkillInstanceId, enemyInstance);

        var enemySkillController = enemyInstance.GetComponent<EnemySkillController>();
        enemySkillController.vm = vm;

       

        enemyInstance.transform.position = spawnTransform.position;
        enemyInstance.transform.rotation = rotationTransform.rotation;


    }
    public void DespawnSkill(int skillInstanceId)
    {
        if (_spawnSkillList.TryGetValue(skillInstanceId, out GameObject skillInstance))
        {
            if (skillInstance != null)
            {
                Destroy(skillInstance);
            }
            _spawnSkillList.Remove(skillInstanceId);

            
        }
        else
        {
            Debug.LogWarning($"[DespawnSkill] ID {skillInstanceId}에 해당하는 스킬을 찾을 수 없습니다.");
        }
    }
}
