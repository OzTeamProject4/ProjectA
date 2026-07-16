using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _turnSpeed = 200.0f;
    [SerializeField] private float _lifeTime = 3.0f;

    private const string EnemyTag = "Enemy";

    private Rigidbody _rigidbody;
    private Transform _target;
    private int _damage;
    private CharacterSkillSystem _ownerSkillSystem;
    private int _gaugeRecovery;

    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 direction = (_target.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);
        _rigidbody.linearVelocity = transform.forward * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag(EnemyTag))
        {
            //TODO IDamageable.TakeDamage 호출 필요
            Debug.Log($"{other.name}에게 {_damage}데미지 전달");

            if (_ownerSkillSystem != null )
            {
                _ownerSkillSystem.AddGauge(_gaugeRecovery);
            }

            Destroy(gameObject);
        }
    }
    public void Launch(Transform target, int damage, CharacterSkillSystem owner, int gaugeRecovery)
    {
        _target = target;
        _damage = damage;
        _ownerSkillSystem = owner;
        _gaugeRecovery = gaugeRecovery;
        transform.rotation = Quaternion.LookRotation((target.position - transform.position).normalized);
        Destroy(gameObject, _lifeTime);
    }
}
