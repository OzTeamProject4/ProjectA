using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckDistance", story: "[Target] is farther than[FollowDistance] from [Self]", category: "Conditions", id: "cfadfd9c48715a4785a781cbd0b04bdf")]
public partial class CheckDistanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> FollowDistance;


    public override bool IsTrue()
    {
        if (Self.Value == null || Target.Value == null)
        {
            return false;
        }

        float distance = Vector3.Distance(Self.Value.transform.position, Target.Value.transform.position);
        return distance > FollowDistance.Value;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
