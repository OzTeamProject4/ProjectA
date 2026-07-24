using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayerUseNormalSkill", story: "[Self] use NormalSkill on [EnemyTarget]", category: "Action", id: "55d9c2265ab6360b103632c84cb1e5d3")]
public partial class PlayerUseSkillAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> EnemyTarget;
    private CharacterSkillSystem _characterSkillSystem;
    private BattleCharacter _battleCharacter;

    protected override Status OnStart()
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

        _battleCharacter.Move(Vector3.zero, false);
        _battleCharacter.LookAt(EnemyTarget.Value.transform.position);
        _characterSkillSystem.UseNormalSkill(EnemyTarget.Value.transform);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

