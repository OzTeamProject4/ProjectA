using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private const string EnemyTag = "Enemy";
    private const float HitEffectLifeTime = 3.0f;

    [SerializeField] private float _turnSpeed = 200.0f;
    [SerializeField] private float _lifeTime = 3.0f;
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private LayerMask _obstacleLayer;

    private Rigidbody _rigidbody;
    private Transform _target;
    private int _damage;
    private CharacterSkillSystem _ownerSkillSystem;
    private int _gaugeRecovery;
    private float _explosionRadius;
    private float _projectileSpeed;
    private float _spawnTime = float.MaxValue;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_target != null)
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);
        }

        _rigidbody.linearVelocity = transform.forward * _projectileSpeed;

        float checkDistance = _projectileSpeed * Time.fixedDeltaTime;
        if (Physics.Raycast(transform.position, transform.forward, checkDistance, _obstacleLayer, QueryTriggerInteraction.Ignore) == true)
        {
            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }

        if (Time.time - _spawnTime >= _lifeTime)
        {
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag(EnemyTag))
        {
            if (_explosionRadius > 0)
            {
                Explode();
            }

            else
            {
                DealSingleDamage(other);
            }
            
            if (_ownerSkillSystem != null)
            {
                _ownerSkillSystem.AddGauge(_gaugeRecovery);
            }

            SpawnHitEffect();

            Destroy(gameObject);
        }
    }
    public void Launch(Transform target, int damage, CharacterSkillSystem owner, int gaugeRecovery, float projectileSpeed, float explosionRadius)
    {
        _target = target;
        _damage = damage;
        _ownerSkillSystem = owner;
        _gaugeRecovery = gaugeRecovery;
        _explosionRadius = explosionRadius;
        _projectileSpeed = projectileSpeed;
        _spawnTime = Time.time;

        if (target != null)
        {
            transform.rotation = Quaternion.LookRotation((target.position - transform.position).normalized);
        }

    }

    private void DealSingleDamage(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && _ownerSkillSystem != null)
        {
            damageable.TakeDamage(_damage, _ownerSkillSystem.gameObject);
        }
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.isTrigger) 
            {
                continue;
            }

            if (hit.CompareTag(EnemyTag) == false)
            {
                continue;
            }

            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && _ownerSkillSystem != null)
            {
                damageable.TakeDamage(_damage, _ownerSkillSystem.gameObject);
            }    
        }
    }

    private void SpawnHitEffect()
    {
        if (_hitEffectPrefab == null)
        {
            return;
        }

        GameObject hitEffect = Instantiate(_hitEffectPrefab, transform.position, Quaternion.identity);
        Destroy(hitEffect, HitEffectLifeTime);
    }
}
