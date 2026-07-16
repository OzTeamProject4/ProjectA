using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayerUseUlt", story: "[Self] use ult on [EnemyTarget]", category: "Action", id: "35140f088053f6a9739f38f3b8a038e9")]
public partial class PlayerUseUltAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> EnemyTarget;
    private CharacterSkillSystem _characterSkillSystem;


    protected override Status OnStart()
    {
        if (_characterSkillSystem == null)
        {
            _characterSkillSystem = Self.Value.GetComponent<CharacterSkillSystem>();
        }

        if (_characterSkillSystem == null || EnemyTarget.Value == null)
        {
            return Status.Failure;
        }

        _characterSkillSystem.UseUltSkill(EnemyTarget.Value.transform);
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

