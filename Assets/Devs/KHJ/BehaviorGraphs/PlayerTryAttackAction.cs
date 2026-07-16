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
    private CharacterSkillSystem _characterSkillSystem;
    private BattleCharacter _battleCharacter;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_characterSkillSystem == null)
        {
            _characterSkillSystem = Self.Value.GetComponent<CharacterSkillSystem>();
        }

        if (_battleCharacter == null)
        {
            _battleCharacter = Self.Value.GetComponent<BattleCharacter>();
        }
        if (_characterSkillSystem == null || _battleCharacter == null || EnemyTarget.Value == null)
        {
            return Status.Failure;   
        }

        _battleCharacter.LookAt(EnemyTarget.Value.transform.position);
        _characterSkillSystem.UseNormalSkill(EnemyTarget.Value.transform);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

