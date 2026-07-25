using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    //TODO 희준 : 임시 공격 쿨타임 추후 변경필요

    public void FireProjectile(string prefabKey, Transform target, int damage, CharacterSkillSystem owner, int gaugeRecovery, float projectileSpeed, float explosionRadius = 0)
    {
        if (string.IsNullOrEmpty(prefabKey) == true)
        {
            Debug.LogError($"투사체 키가 비어있음");
            return;
        }

        if (_firePoint == null)
        {
            Debug.LogError("_firePoint가 null 인스펙터 확인");
            return;
        }

        FireProjectileAsync(prefabKey, target, damage, owner, gaugeRecovery, projectileSpeed, explosionRadius).Forget();
    }

    private async UniTaskVoid FireProjectileAsync(string prefabKey, Transform target, int damage, CharacterSkillSystem owner, int gaugeRecovery, float projectileSpeed, float explosionRadius)
    {
        try
        {
            GameObject projectile = await GameManager.Instance.ObjectManager.SpawnAsync(prefabKey, null, _firePoint.position, _firePoint.rotation, destroyCancellationToken);
            if (projectile == null)
            {
                return;
            }

            if (projectile.TryGetComponent(out Projectile projectileComponent) == true)
            {
                projectileComponent.Launch(target, damage, owner, gaugeRecovery, projectileSpeed, explosionRadius);
            }
        }
        catch (System.OperationCanceledException)
        {

        }
    }
}
