using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private LayerMask playerLayer;

    private EnemyHealth enemyHealth;
    private float nextAttackTime;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        BattleCharacter target = FindNearestTarget();

        if (target == null)
        {
            return;
        }

        int finalDamage = ElementDamageCalculator.ApplyElementModifier(
            enemyHealth.BaseDamage,
            enemyHealth.ElementType,
            target.ElementType
        );


        Debug.Log($"{name} → {target.CharacterName}: {finalDamage} 피해");
        IDamageable damageable = target;
        damageable.TakeDamage(finalDamage, gameObject);

        nextAttackTime = Time.time + attackInterval;
    }

    private BattleCharacter FindNearestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            attackRange,
            playerLayer
        );

        BattleCharacter nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider hitCollider in colliders)
        {
            BattleCharacter target =
                hitCollider.GetComponentInParent<BattleCharacter>();

            if (target == null)
            {
                continue;
            }

            float distance = Vector3.Distance(
                transform.position,
                target.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = target;
            }
        }

        return nearestTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
