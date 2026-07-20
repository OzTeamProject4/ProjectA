using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Stop", story: "[Self] stops", category: "Action", id: "1d922eba6a73f5192cfcefe061db625c")]
public partial class StopAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    private BattleCharacter _battleCharacter;


    protected override Status OnStart()
    {
        _battleCharacter = Self.Value.GetComponent<BattleCharacter>();
        if (_battleCharacter == null)
        {
            Debug.LogError("_battleCharacter가 null");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        _battleCharacter.Move(Vector3.zero, false);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

