using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayerRetreatFromTarget", story: "[Self] retreats from [EnemyTarget] to [MinAttackRange]", category: "Action", id: "15b1b3fb44eddbacd2cee70163c93e71")]
public partial class PlayerRetreatFromTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> EnemyTarget;
    [SerializeReference] public BlackboardVariable<float> MinAttackRange;

    private const float RetreatBuffer = 3f; // TODO희준 : 테스트용 상수, 이후 조절 필요함
    private const float RetreatDestinationExtra = 5f;

    private BattleCharacter _battleCharacter;
    private NavMeshAgent _navMeshAgent;
    private CharacterSkillSystem _skillSystem;

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
        if (distance > _skillSystem.MinAttackRange + RetreatBuffer)
        {
            _battleCharacter.Move(Vector3.zero, false);
            return Status.Success;
        }

        Vector3 awayDirection = Self.Value.transform.position - EnemyTarget.Value.transform.position;
        awayDirection.y = 0;

        if (awayDirection.sqrMagnitude < 0.01f)
        {
            return Status.Success;
        }

        awayDirection = awayDirection.normalized;
        _navMeshAgent.nextPosition = Self.Value.transform.position;

        Vector3 retreatPosition = EnemyTarget.Value.transform.position + awayDirection * (_skillSystem.MinAttackRange + RetreatBuffer + RetreatDestinationExtra);
        _navMeshAgent.SetDestination(retreatPosition);

        Vector3 direction = _navMeshAgent.desiredVelocity;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
        {
            _battleCharacter.Move(Vector3.zero, false);
            return Status.Running;
        }

        _battleCharacter.Move(direction.normalized, false, false);
        _battleCharacter.LookAt(EnemyTarget.Value.transform.position);

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

