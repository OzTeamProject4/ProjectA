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
    private float _coolDownDuration = 3.0f;
    private float _lastAttackTime = -3.0f;
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

        
        if (Time.time - _lastAttackTime < _coolDownDuration)
        {
            Debug.Log("쿨타임");
            return Status.Failure; 
        }

        // [실행] 쿨타임이 지났다면 공격 호출 후 타이머 갱신
        _enemyController.TryAttackSkill();
        _lastAttackTime = Time.time;

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

