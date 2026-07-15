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
    private CharacterAttack _characterAttack;
    private BattleCharacter _battleCharacter;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_characterAttack == null)
        {
            _characterAttack = Self.Value.GetComponent<CharacterAttack>();
        }

        if (_battleCharacter == null)
        {
            _battleCharacter = Self.Value.GetComponent<BattleCharacter>();
        }
        if (_characterAttack == null || EnemyTarget.Value == null)
        {
            return Status.Failure;   
        }

        _battleCharacter.LookAt(EnemyTarget.Value.transform.position);
        _characterAttack.FireProjectile(EnemyTarget.Value.transform);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

