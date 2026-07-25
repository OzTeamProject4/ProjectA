using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Self] Navigate To [Target]", category: "Action", id: "75dcd5457114f04ab9a06df7d7b81d4f")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private NavMeshAgent _agent;

    protected override Status OnStart()
    {
        _agent = Self.Value.GetComponent<NavMeshAgent>();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Target.Value != null)
        {
            _agent.SetDestination(Target.Value.transform.position);
        }
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

