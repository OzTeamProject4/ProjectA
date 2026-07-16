using UnityEngine;

public class Attack : MonoBehaviour
{
    // 공격 범위
    [SerializeField] private float attackRadius = 1f;
    // 공격 대상
    [SerializeField] private LayerMask enemyLayer;


    private BattleCharacter battleCharacter;

    private void Awake()
    {
        battleCharacter = GetComponent<BattleCharacter>();
    }

    public void AttackEnemy()
    {
        if(battleCharacter == null)
        {
            return;
        }

        Vector3 attackPosition = transform.position + transform.forward * 1.5f;

        Collider[] hitColliders = Physics.OverlapSphere(
            attackPosition,
            attackRadius,
            enemyLayer
        );

        foreach (Collider hitCollider in hitColliders)
        {
            EnemyHealth enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth == null)
            {
                continue;
            }
            
            // 임시 - 공격력을 그대로 사용
            int damageBeforeElement = battleCharacter.CurrentAttack;

            int finalDamage = ElementDamageCalculator.ApplyElementModifier(
                   damageBeforeElement,
                   battleCharacter.ElementType,
                   enemyHealth.ElementType
               );

            IDamageable damageable = enemyHealth;
            damageable.TakeDamage(finalDamage, gameObject);
        }

    }
}

