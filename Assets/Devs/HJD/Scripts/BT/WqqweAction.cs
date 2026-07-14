using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "wqqwe", story: "[AAA]", category: "Action", id: "93a38866f8f37db07c9cee72581ad718")]
public partial class WqqweAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> AAA;
    [SerializeReference] public BlackboardVariable<float> _baseHp;


    protected override Status OnStart()
    {
        
        Debug.Log("진입");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Debug.Log("업데이트");

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

