using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    //TODO 희준 : 임시 공격 쿨타임 추후 변경필요
    [SerializeField] private float _attackCooldown = 1.0f;

    public void FireProjectile(GameObject prefab, Transform target, int damage, CharacterSkillSystem owner, int gaugeRecovery, float projectileSpeed, float explosionRadius = 0)
    {
        if (prefab == null)
        {
            Debug.LogError($"투사체 프리팹이 null ");
            return;
        }

        if (_firePoint == null)
        {
            Debug.LogError("_firePoint가 null 인스펙터 확인");
            return;
        }

        GameObject projectile = Instantiate(prefab, _firePoint.position, _firePoint.rotation);
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Launch(target, damage, owner, gaugeRecovery, projectileSpeed, explosionRadius);
        }
    }
}
