using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BT_CheckBool", story: "[Self] check [isIdle]", category: "Action", id: "5fdf83a37efe1822ac578b8d314fa3c7")]
public partial class BT_BtCheckBoolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> IsIdle;


    protected override Status OnStart()
    {
        return Status.Running;
    }

   
}

