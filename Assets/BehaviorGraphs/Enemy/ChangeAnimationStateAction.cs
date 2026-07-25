using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeAnimationState", story: "[Self] to [EnemyState]", category: "Action", id: "df79cfd2aeb587988187588f5af288d6")]
public partial class ChangeAnimationStateAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<EnemyBattleState> EnemyState;

    protected override Status OnStart()
    {
        var enemyControllerSelf = Self.Value.GetComponent<EnemyController>();
        if (enemyControllerSelf)
        {
            enemyControllerSelf.ChangeState(EnemyState);
        }

        return Status.Success;
    }

   

    protected override void OnEnd()
    {
    }
}

