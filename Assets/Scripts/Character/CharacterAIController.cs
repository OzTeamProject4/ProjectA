using Unity.Behavior;
using UnityEngine;

[RequireComponent(typeof(BehaviorGraphAgent))]
public class CharacterAIController : MonoBehaviour
{
    private BehaviorGraphAgent _agent;
    private CharacterDetector _detector;

    private void Awake()
    {
        _agent = GetComponent<BehaviorGraphAgent>();
        _detector = GetComponentInChildren<CharacterDetector>();
        if (_detector == null)
        {
            Debug.LogError("_detector가 null");
            return;
        }
    }

    private void OnEnable()
    {
        if (_detector == null)
        {
            Debug.LogError("_detector가 null");
            return;
        }

        _detector.OnEnemyDetected += HandleEnemyDetected;
        _detector.OnEnemyLost += HandleEnemyLost;
    }

    private void OnDisable()
    {
        if (_detector == null)
        {
            Debug.LogError("_detector가 null");
            return;
        }

        _detector.OnEnemyDetected -= HandleEnemyDetected;
        _detector.OnEnemyLost -= HandleEnemyLost;
    }

    public void SetAIFollowTarget(BattleCharacter targetCharacter)
    {
        GameObject target = targetCharacter.gameObject;
        _agent.SetVariableValue("FollowTarget", target);
    }

    public void EnableAI()
    {
        _agent.enabled = true;
    }

    public void DisableAI()
    {
        _agent.enabled = false;
    }

    private void HandleEnemyDetected(GameObject enemy)
    {
        _agent.SetVariableValue("EnemyTarget", enemy);
    }

    private void HandleEnemyLost()
    {
        _agent.SetVariableValue("EnemyTarget", (GameObject)null);
    }
}