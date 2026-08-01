using System;
using UnityEngine;

public class CharacterDetector : MonoBehaviour
{
    private const string EnemyTag = "Enemy";

    public event Action<GameObject> OnEnemyDetected;
    public event Action OnEnemyLost;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(EnemyTag))
        {
            OnEnemyDetected?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(EnemyTag))
        {
            OnEnemyLost?.Invoke();
        }
    }
}
