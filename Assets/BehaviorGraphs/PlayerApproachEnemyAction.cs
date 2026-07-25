using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayerApproachEnemy", story: "[Self] approaches [EnemyTarget] running [IsRunning]", category: "Action", id: "a60be9b57f691940a3b2aa163ca23f01")]
public partial class PlayerApproachEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> EnemyTarget;
    [SerializeReference] public BlackboardVariable<bool> IsRunning;

    private BattleCharacter _battleCharacter;
    private CharacterSkillSystem _skillSystem;
    private NavMeshAgent _navMeshAgent;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            return Status.Failure;
        }

        _battleCharacter = Self.Value.GetComponent<BattleCharacter>();
        _skillSystem = Self.Value.GetComponent<CharacterSkillSystem>();
        if (_battleCharacter == null || _skillSystem == null)
        {
            return Status.Failure;
        }

        _navMeshAgent = _battleCharacter.NavMeshAgent;
        if (_navMeshAgent == null)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (EnemyTarget.Value == null)
        {
            return Status.Failure;
        }

        if (_navMeshAgent.isOnNavMesh == false)
        {
            return Status.Failure;
        }

        float distance = Vector3.Distance(Self.Value.transform.position, EnemyTarget.Value.transform.position);
        if (distance <= _skillSystem.AttackRange)
        {
            _battleCharacter.Move(Vector3.zero, false);
            return Status.Success;
        }

        _navMeshAgent.nextPosition = Self.Value.transform.position;
        _navMeshAgent.SetDestination(EnemyTarget.Value.transform.position);

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

