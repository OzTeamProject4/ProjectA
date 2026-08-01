using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BehaviorGraphAgent))]
public class CharacterAIController : MonoBehaviour
{
    private const float TargetRefreshInterval = 0.2f;
    private const float StuckCheckInterval = 0.5f;
    private const float StuckTeleportDelay = 3.0f;
    private const float TeleportSampleRadius = 30.0f;
    private const float TeleportSkipDistance = 3.0f;
    private const string EnemyTargetKey = "EnemyTarget";
    private const string FollowTargetKey = "FollowTarget";
    private const string SlotIndexKey = "SlotIndex";

    private BehaviorGraphAgent _agent;
    private CharacterDetector _detector;
    private BattleCharacter _battleCharacter;
    private BattleCharacter _followTarget;
    private int _slotIndex;
    private NavMeshPath _stuckPath;
    private float _lastStuckCheckTime;
    private float _lastRefreshTime;
    private float _stuckTimer;

    private void Awake()
    {
        _stuckPath = new NavMeshPath();
        _agent = GetComponent<BehaviorGraphAgent>();
        _detector = GetComponentInChildren<CharacterDetector>();
        _battleCharacter = GetComponent<BattleCharacter>();

        if (_detector == null)
        {
            Debug.LogError("_detector가 null");
        }

        if (_battleCharacter == null)
        {
            Debug.LogError("_battleCharacter가 null");
        }

        _lastRefreshTime = -TargetRefreshInterval;
    }

    private void Update()
    {
        if (_agent.enabled == false)
        {
            return;
        }

        UpdateEnemyTarget();
        UpdateStuckCheck();
    }

    public void SetAIFollowTarget(BattleCharacter targetCharacter)
    {
        _followTarget = targetCharacter;
        _agent.SetVariableValue(FollowTargetKey, targetCharacter.gameObject);
    }

    public void SetSlotIndex(int slotIndex)
    {
        _slotIndex = slotIndex;
        _agent.SetVariableValue(SlotIndexKey, slotIndex);
    }

    public void EnableAI()
    {
        _stuckTimer = 0f;
        _agent.enabled = true;
    }

    public void DisableAI()
    {
        _agent.enabled = false;
    }

    private void UpdateEnemyTarget()
    {
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

    private void UpdateStuckCheck()
    {
        if (_followTarget == null || _battleCharacter == null)
        {
            return;
        }

        NavMeshAgent navMeshAgent = _battleCharacter.NavMeshAgent;

        if (navMeshAgent == null || navMeshAgent.isOnNavMesh == false)
        {
            return;
        }

        if (Time.time - _lastStuckCheckTime < StuckCheckInterval)
        {
            return;
        }

        float elapsed = Time.time - _lastStuckCheckTime;
        _lastStuckCheckTime = Time.time;

        Vector3 targetPosition = _followTarget.transform.position;

        if (Vector3.Distance(transform.position, targetPosition) <= navMeshAgent.stoppingDistance)
        {
            _stuckTimer = 0f;
            return;
        }

        bool reachable = navMeshAgent.CalculatePath(targetPosition, _stuckPath);

        if (reachable && _stuckPath.status == NavMeshPathStatus.PathComplete)
        {
            _stuckTimer = 0f;
            return;
        }

        _stuckTimer += elapsed;

        if (_stuckTimer < StuckTeleportDelay)
        {
            return;
        }

        _stuckTimer = 0f;
        TeleportToFollowTarget();
    }

    private void TeleportToFollowTarget()
    {
        NavMeshAgent navMeshAgent = _battleCharacter.NavMeshAgent;

        Vector3 desiredPosition = AIFormationUtil.GetFormationPosition(
            _followTarget.transform.position, _slotIndex, navMeshAgent.stoppingDistance);

        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, TeleportSampleRadius, NavMesh.AllAreas) == false)
        {
            Debug.LogWarning($"[CharacterAIController] 순간이동할 NavMesh 지점을 찾지 못했습니다. name={name}");
            return;
        }

        if (Vector3.Distance(transform.position, hit.position) < TeleportSkipDistance)
        {
            return;
        }

        _battleCharacter.Teleport(hit.position);
    }
}