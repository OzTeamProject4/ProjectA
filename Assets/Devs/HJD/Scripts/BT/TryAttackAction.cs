using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TryAttack", story: "[Self] Try Attack", category: "Action", id: "dd925f011f5bff37c0983c40b2a9f009")]
public partial class TryAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    private EnemyController _enemyController;
    private bool _isAttacking;
    private CancellationTokenSource _cts;

    protected override Status OnStart()
    {
        if (_enemyController == null && Self.Value != null)
        {
            _enemyController = Self.Value.GetComponent<EnemyController>();
        }
        if(_enemyController == null)
            return Status.Failure;

        _cts = new CancellationTokenSource();
        _isAttacking = true;

        ExecuteAttackAsync(_cts.Token).Forget();

        return Status.Running;
    }

    private async UniTaskVoid ExecuteAttackAsync(CancellationToken token)
    {
        try
        {
            // TryAttackSkillAsync 내부에서 애니메이션 재생 및 대기(await)를 처리
            await _enemyController.TryAttackSkillAsync(token);
        }
        catch (OperationCanceledException)
        {
            // 노드가 도중에 취소되었을 때 처리
        }
        finally
        {
            _isAttacking = false;
        }
    }

    protected override Status OnUpdate()
    {
        if (_enemyController == null) return Status.Failure;


        if (_isAttacking)
        {
            return Status.Running;
        }

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

