using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "my", story: "qwer", category: "Action", id: "c37af286bed73b3d3d8c368824f4f5dd")]
public partial class MyAction : Action
{

    protected override Status OnStart()
    {
        Debug.Log("АјАн");
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

