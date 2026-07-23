using Unity.Behavior;
using UnityEngine;

public class EnemySkillController : MonoBehaviour
{
    public EnemySkillViewModel _vm;
    [SerializeField] private float speed = 10f;

    private Rigidbody _rb;

    private void Awake()
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();

        }
    }

    public void Bind(EnemySkillData enemySkillData, EnemySkillViewModel vm,EnemyController enemyController)
    {

        _vm = vm;

        vm.SkillDataId = enemySkillData.DataId;
        vm.Name = enemySkillData.Name;
        vm.ElementalType = enemySkillData.ElementalType;
        vm.BaseDamage = enemySkillData.BaseDamage;
        vm.PrefabAddress = enemySkillData.PrefabAddress;
        vm.UseEnemyController = enemyController;
    }




    private void FixedUpdate()
    {
        // 바라보는 방향(forward)으로 속도 부여
        _rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Hit(other);
        }


    }
    private void Hit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && _vm.UseEnemyController != null)
        {
            damageable.TakeDamage(_vm.UseEnemyController._vm.CurrentDamage, _vm.UseEnemyController.gameObject);
        }

        GameManager.Instance.ObjectManager.Despawn(this.gameObject);
    }
}
