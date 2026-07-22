using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "PlayerCanUseUlt", story: "[Self] can use ult", category: "Conditions", id: "9d6a3d7f3728e8136a47a313c7eabb61")]
public partial class PlayerCanUseUltCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    private CharacterSkillSystem _characterSkillSystem;

    public override bool IsTrue()
    {
        if (_characterSkillSystem == null && Self.Value != null)
        {
            _characterSkillSystem = Self.Value.GetComponent<CharacterSkillSystem>();
        }

        if (_characterSkillSystem == null)
        {
            return false;
        }

        return _characterSkillSystem.CanUseUltSkill();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
