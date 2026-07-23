using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayerFollowTarget", story: "[Self] navigates to [Target] running[IsRunning]", category: "Action", id: "a53c0a1a49a30fcfed69192133254001")]
public partial class PlayerFollowTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> IsRunning;

    private BattleCharacter _battleCharacter;
    private NavMeshAgent _navMeshAgent;
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

        _navMeshAgent.nextPosition = Self.Value.transform.position;
        _navMeshAgent.SetDestination(Target.Value.transform.position);

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

