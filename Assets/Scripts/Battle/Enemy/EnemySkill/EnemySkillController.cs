using UnityEngine;
using PixPlays.ElementalVFX; 

public class EnemySkillController : MonoBehaviour
{
    public EnemySkillViewModel _vm;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 5f;

    private Enemy_ProjectileVfx _vfx;
    private bool _hasHit = false;
    private Rigidbody _rb;

    private void Awake()
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
        }
        _vfx = GetComponent<Enemy_BaseVfx>() as Enemy_ProjectileVfx;
    }

    private void OnEnable()
    {
        _hasHit = false;
    }
    private void FixedUpdate()
    {
        // 이미 충돌했다면 이동 멈춤
        if (_hasHit)
        {
            _rb.linearVelocity = Vector3.zero;
            return;
        }

        // 바라보는 방향(forward)으로 속도 부여
        _rb.linearVelocity = transform.forward * speed;
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

        Fire(this.gameObject.transform.position, this.gameObject.transform.forward);
    }




    private void Fire(Vector3 spawnPos, Vector3 direction)
    {
        _hasHit = false;
        transform.position = spawnPos;
        transform.forward = direction.normalized;

        if (_vfx != null)
        {
            Vector3 targetDummyPos = spawnPos + transform.forward * 10f;
            VfxData data = new VfxData(spawnPos, targetDummyPos, lifeTime, 1f);
            _vfx.Play(data);
        }

        CancelInvoke(nameof(DespawnSelf));
        Invoke(nameof(DespawnSelf), lifeTime);
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
        _hasHit = true;
        _rb.linearVelocity = Vector3.zero;
        CancelInvoke(nameof(DespawnSelf));

        // 1. 데미지 처리
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && _vm != null && _vm.UseEnemyController != null)
        {
            damageable.TakeDamage(_vm.UseEnemyController._vm.CurrentDamage, _vm.UseEnemyController.gameObject);
        }

        // 2. 충돌 위치 및 방향으로 Hit 이펙트 재생
        if (_vfx != null)
        {
            // 충돌한 반대 방향으로 Hit 파티클 정렬
            _vfx.PlayHitEffect(transform.position, -transform.forward);
        }

        // 3. Hit 파티클이 사라질 시간(1초)을 확보한 뒤 Despawn
        Invoke(nameof(DespawnSelf), 1f);
    }

    private void DespawnSelf()
    {
        GameManager.Instance.ObjectManager.Despawn(this.gameObject);
    }
}
