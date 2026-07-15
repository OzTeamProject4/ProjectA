using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RunFollow", story: "[Self] runs to [Target]", category: "Action", id: "2a333b670bc0902f82f14a07479f599a")]
public partial class RunFollowAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    private BattleCharacter _battleCharacter;

    protected override Status OnStart()
    {
        _battleCharacter = Self.Value.GetComponent<BattleCharacter>();
        if (_battleCharacter == null)
        {
            Debug.Log("BattleCharacter가 null");
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

        Vector3 direction = (Target.Value.transform.position - Self.Value.transform.position).normalized;
        _battleCharacter.Move(direction, true);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

