using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TryAttack", story: "[Self] Try Attack", category: "Action", id: "dd925f011f5bff37c0983c40b2a9f009")]
public partial class TryAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    private EnemyController _enemyController;
   
    protected override Status OnStart()
    {
        if (_enemyController == null && Self.Value != null)
        {
            _enemyController = Self.Value.GetComponent<EnemyController>();
        }

      
         
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_enemyController == null) return Status.Failure;

        

        _enemyController.TryAttackSkill();


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

