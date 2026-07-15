using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "PlayerCanUseSkill", story: "[Self] can use Skill", category: "Conditions", id: "3d7cb0f8c943be7903f01e08e74bb5a6")]
public partial class PlayerCanUseSkillCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    private CharacterAttack _characterAttack;

    public override bool IsTrue()
    {
        if (_characterAttack == null && Self.Value != null)
        {
            _characterAttack = Self.Value.GetComponent<CharacterAttack>();
        }

        if (_characterAttack == null)
        {
            return false;
        }

        return _characterAttack.CanUseSkill();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
