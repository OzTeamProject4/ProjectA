using System.Collections.Generic;
using UnityEngine;

public class CharacterDetector : MonoBehaviour
{
    private const string EnemyTag = "Enemy";

    private readonly List<EnemyController> _enemyList = new List<EnemyController>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(EnemyTag) == false)
        {
            return;
        }

        EnemyController enemy = other.GetComponentInParent<EnemyController>();

        if (enemy == null)
        {
            return;
        }

        if (_enemyList.Contains(enemy))
        {
            return;
        }

        _enemyList.Add(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(EnemyTag) == false)
        {
            return;
        }

        EnemyController enemy = other.GetComponentInParent<EnemyController>();

        if (enemy == null)
        {
            return;
        }

        _enemyList.Remove(enemy);
    }

    public GameObject GetNearestEnemy()
    {
        EnemyController nearest = null;
        float nearestSqrDistance = float.MaxValue;
        Vector3 myPosition = transform.position;

        for (int i = _enemyList.Count - 1; i >= 0; i--)
        {
            EnemyController enemy = _enemyList[i];

            if (enemy == null)
            {
                _enemyList.RemoveAt(i);
                continue;
            }

            if (enemy.gameObject.activeInHierarchy == false)
            {
                _enemyList.RemoveAt(i);
                continue;
            }

            if (enemy.IsAlive == false)
            {
                continue;
            }

            float sqrDistance = (enemy.transform.position - myPosition).sqrMagnitude;

            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearest = enemy;
            }
        }

        if (nearest == null)
        {
            return null;
        }

        return nearest.gameObject;
    }
}