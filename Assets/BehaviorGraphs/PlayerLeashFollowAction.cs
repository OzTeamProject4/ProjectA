using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayerLeashFollow", story: "[Self] father than [LeashDistance] navigate to [FollowTarget]", category: "Action", id: "5a45b949a954401d64b8c4d1319cf253")]
public partial class PlayerLeashFollowAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> LeashDistance;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> IsRunning;
    [SerializeReference] public BlackboardVariable<float> LeashStopDistance;
    [SerializeReference] public BlackboardVariable<int> SlotIndex;

    private BattleCharacter _battleCharacter;
    private NavMeshAgent _navMeshAgent;
    private bool _isFollowing;
    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            return Status.Failure;
        }

        _battleCharacter = Self.Value.GetComponent<BattleCharacter>();
        if (_battleCharacter == null)
        {
            Debug.LogError("BattleCharacter가 null");
            return Status.Failure;
        }

        _navMeshAgent = _battleCharacter.NavMeshAgent;
        if (_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent가 null");
            return Status.Failure;
        }
        _isFollowing = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Target.Value == null)
        {
            return Status.Failure;
        }

        if (_navMeshAgent.isOnNavMesh == false)
        {
            return Status.Failure;
        }

        float distance = Vector3.Distance(Self.Value.transform.position, Target.Value.transform.position);

        if (_isFollowing == false)
        {
            if (distance > LeashDistance.Value)
            {
                _isFollowing = true;
            }

            else
            {
                return Status.Failure;
            }
        }

        else
        {
            if (distance <= LeashStopDistance.Value)
            {
                _isFollowing = false;
                _battleCharacter.Move(Vector3.zero, false);
                return Status.Success;
            }
        }

        _navMeshAgent.nextPosition = Self.Value.transform.position;

        Vector3 destination = AIFormationUtil.GetFormationPosition(Target.Value.transform.position, SlotIndex.Value, LeashStopDistance.Value);
        _navMeshAgent.SetDestination(destination);

        if (_navMeshAgent.pathPending == false && _navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            _navMeshAgent.SetDestination(Target.Value.transform.position);
        }

        Vector3 direction = _navMeshAgent.desiredVelocity;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
        {
            _battleCharacter.Move(Vector3.zero, false);
            return Status.Running;
        }

        _battleCharacter.Move(direction.normalized, IsRunning.Value);
        return Status.Running;
    }


    protected override void OnEnd()
    {
    }
}


