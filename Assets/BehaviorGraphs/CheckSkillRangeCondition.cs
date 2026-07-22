using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckSkillRange", story: "[EnemyTarget] is farther than [Self] skill range, min [UseMinRange]", category: "Conditions", id: "4a9e0b26d346d08acbb9a1860bee62d4")]
public partial class CheckSkillRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> EnemyTarget;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> UseMinRange;

    public override bool IsTrue()
    {
        if (Self.Value == null || EnemyTarget.Value == null)
        {
            return false;
        }

        CharacterSkillSystem skillSystem = Self.Value.GetComponent<CharacterSkillSystem>();
        if (skillSystem == null)
        {
            return false;
        }

        float range = UseMinRange.Value ? skillSystem.MinAttackRange : skillSystem.AttackRange;
        float distance = Vector3.Distance(Self.Value.transform.position, EnemyTarget.Value.transform.position);
        return distance > range;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
