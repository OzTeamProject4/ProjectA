using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayerTryAttack", story: "[Self] attack [EnemyTarget]", category: "Action", id: "c137f24be9a0e83274a52cfd9a9132e5")]
public partial class PlayerTryAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> EnemyTarget;
    private BattleCharacter _battleCharacter;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_battleCharacter == null)
        {
            _battleCharacter = Self.Value.GetComponent<BattleCharacter>();
        }

        if (_battleCharacter == null || EnemyTarget.Value == null)
        {
            Debug.Log($"공격 노드 진입했으나 실패: bc={_battleCharacter}, enemy={EnemyTarget.Value}");
            return Status.Failure;   
        }

        Debug.Log($"{Self.Value.name}이 {EnemyTarget.Value.name} 공격!");
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

