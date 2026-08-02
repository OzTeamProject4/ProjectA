using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckCloserThan", story: "[EnemyTarget] is closer than [Distance] from [Self]", category: "Conditions", id: "9ed0c3a4c531d4deca56ce90ffac2a96")]
public partial class CheckCloserThanCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> EnemyTarget;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        if (Self.Value == null || EnemyTarget.Value == null)
        {
            return false;
        }

        float distance = Vector3.Distance(Self.Value.transform.position, EnemyTarget.Value.transform.position);
        return distance < Distance.Value;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
