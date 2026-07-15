using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    //TODO 희준 : 임시 공격 쿨타임 추후 변경필요
    [SerializeField] private float _attackCooldown = 1.0f;

    private float _lastAttackTime;

    public void FireProjectile(Transform target)
    {
        if (_projectilePrefab == null)
        {
            Debug.LogError("_projectilePrefab이 null 인스펙터 확인");
            return;
        }

        if (_firePoint == null)
        {
            Debug.LogError("_firePoint가 null 인스펙터 확인");
            return;
        }

        if (target == null)
        {
            return;
        }

        if (Time.time - _lastAttackTime < _attackCooldown)
        {
            return;
        }

        GameObject projectile = Instantiate(_projectilePrefab, _firePoint.position, _firePoint.rotation);
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Launch(target);
        }
        _lastAttackTime = Time.time;
    }
    // TODO 희준: 실제 게이지/쿨타임 로직으로 교체 
    public bool CanUseUlt()
    {
        return false;   // 임시: 항상 궁극기 불가
    }

    public bool CanUseSkill()
    {
        return false;   // 임시: 항상 스킬 불가
    }

    public void UseUlt(Transform target)
    {
        Debug.Log("궁극기!");   // 임시
    }

    public void UseSkill(Transform target)
    {
        Debug.Log("스킬!");     // 임시
    }
}
