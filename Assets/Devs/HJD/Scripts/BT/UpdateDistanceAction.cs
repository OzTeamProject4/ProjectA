using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateDistance", story: "Update [Self] and [Target] [CurrentDist]", category: "Action", id: "cdf2fbdb4affae9c3b37296161901b61")]
public partial class UpdateDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> CurrentDist;

    protected override Status OnStart()
    {
        if (Target.Value == null) {
            return Status.Failure;
        }
        CurrentDist.Value = Vector2.Distance(Self.Value.transform.position, Target.Value.transform.position);

        return Status.Success;
    }

  

    protected override void OnEnd()
    {
    }
}

