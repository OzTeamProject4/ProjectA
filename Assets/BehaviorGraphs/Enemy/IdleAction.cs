using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Idle", story: "[Self] Stop Chase", category: "Action", id: "5f96b75f382dce2ff85a509a5ba430af")]
public partial class IdleAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        var _agent = Self.Value.GetComponent<NavMeshAgent>();

        if (_agent == null) { return Status.Failure; }

        _agent.ResetPath();

        return Status.Running;
    }


    

    protected override void OnEnd()
    {
    }
}

