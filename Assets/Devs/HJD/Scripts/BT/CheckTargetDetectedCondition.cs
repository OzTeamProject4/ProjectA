using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckTargetDetectedCondition", story: "Compare values of [CurrentDistance] and [ChaseDistance]", category: "Conditions", id: "06b30f6db6523bb48747fd485d1adbe9")]
public partial class CheckTargetDetectedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> CurrentDistance;
    [SerializeReference] public BlackboardVariable<float> ChaseDistance;

    public override bool IsTrue()
    {
        bool conditionResult = CurrentDistance.Value <= ChaseDistance.Value;
        return conditionResult;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
