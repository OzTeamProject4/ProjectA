using Unity.Behavior;
using UnityEngine;

[RequireComponent(typeof(BehaviorGraphAgent))]
public class CharacterAIController : MonoBehaviour
{
    private const float TargetRefreshInterval = 0.2f;
    private const string EnemyTargetKey = "EnemyTarget";
    private const string FollowTargetKey = "FollowTarget";

    private BehaviorGraphAgent _agent;
    private CharacterDetector _detector;
    private float _lastRefreshTime;

    private void Awake()
    {
        _agent = GetComponent<BehaviorGraphAgent>();
        _detector = GetComponentInChildren<CharacterDetector>();

        if (_detector == null)
        {
            Debug.LogError("_detector가 null");
        }

        _lastRefreshTime = -TargetRefreshInterval;
    }

    private void Update()
    {
        if (_agent.enabled == false)
        {
            return;
        }

        if (_detector == null)
        {
            return;
        }

        if (Time.time - _lastRefreshTime < TargetRefreshInterval)
        {
            return;
        }

        _lastRefreshTime = Time.time;
        _agent.SetVariableValue(EnemyTargetKey, _detector.GetNearestEnemy());
    }

    public void SetAIFollowTarget(BattleCharacter targetCharacter)
    {
        GameObject target = targetCharacter.gameObject;
        _agent.SetVariableValue(FollowTargetKey, target);
    }

    public void EnableAI()
    {
        _agent.enabled = true;
    }

    public void DisableAI()
    {
        _agent.enabled = false;
    }
}