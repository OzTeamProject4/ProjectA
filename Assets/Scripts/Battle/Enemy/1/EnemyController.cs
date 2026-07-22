using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.Behavior;
using UnityEngine;
public enum EnemyBattleState
{
    Idle,
    Walk,
    Run,
    Attack,
    Die,

}
public class EnemyController : MonoBehaviour, IDamageable
{
    public Transform _enemyTransform;

    [SerializeField] private Transform _skillTransform;
    [SerializeField] private Animator _animator;
    private EnemyBattleState _currentStateEnum;


    //private readonly int IdleHash = Animator.StringToHash("IsIdle");
    private static readonly int WalkHash = Animator.StringToHash("IsWalk");
    private static readonly int RunHash = Animator.StringToHash("IsRun");
    private static readonly int AttackHash = Animator.StringToHash("IsAttack");
    private static readonly int DieHash = Animator.StringToHash("IsDie");

    public EnemyViewModel _vm;
    public Blackboard blackboard;
    public BehaviorGraphAgent behaviorGraphAgent;

    private BattleManager _battleManager;


    private void Awake()
    {
        behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();

        if (_skillTransform == null)
        {
            _skillTransform = this.transform;
        }
    }

    public void Bind(EnemyData enemyData, EnemyViewModel vm)
    {
        _vm = vm;

        _vm.EnemyDataId = enemyData.DataId;
        _vm.Name = enemyData.Name;
        _vm.TotalExp = enemyData.TotalExp;
        _vm.ElementalType = enemyData.ElementalType;
        _vm.BaseHp = enemyData.BaseHp;
        _vm.CurrentHp = enemyData.BaseHp;
        _vm.MaxHp = enemyData.BaseHp;
        _vm.BaseDamage = enemyData.BaseDamage;
        _vm.CurrentDamage = enemyData.BaseDamage;
        _vm.PrefabAddress = enemyData.PrefabAddress;
        _vm.SkillPrefabAddress = enemyData.SkillPrefabAddress;
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (_vm.CurrentHp <= 0 || damage <= 0)
        {
            return;
        }

        _vm.CurrentHp -= damage;

        if (_vm.CurrentHp < 0)
        {
            _vm.CurrentHp = 0;
        }

        if (_vm == null)
        {
            Debug.LogError("컨트롤러에 뷰 모델이 없습니다");
            return;
        }


        if (_vm.CurrentHp == 0)
        {
            // 체력이 0이면 사망 알림
        }
    }


    public void RequestAddExpToEnemy(int exp)
    {
        if (_vm != null)
        {
            _vm.TotalExp += exp;
        }
    }
    public void RequestAddStatToEnemy(int addDamageValue)
    {
        if (_vm != null)
        {
            _vm.BaseDamage += addDamageValue;
        }
    }

    public void ChangeState(EnemyBattleState newState)
    {
        /* if (IsStateChangeable(newState) == false)
         {
             return;
         }*/

        _currentStateEnum = newState;
        PlayStateAnimation(_currentStateEnum);
    }




    public async UniTask TryAttackSkillAsync(CancellationToken cancellationToken = default)
    {
        ChangeState(EnemyBattleState.Attack);

        float attackDuration = 1.0f;

        for (int i = 0; i < 10; i++)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            if (_animator == null) return;

            AnimatorStateInfo currentInfo = _animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = _animator.GetNextAnimatorStateInfo(0);

            if (currentInfo.IsName("Attack"))
            {
                attackDuration = currentInfo.length;
                break;
            }
            else if (nextInfo.IsName("Attack"))
            {
                attackDuration = nextInfo.length;
                break;
            }
        }

        SpawnSkill();

        await UniTask.Delay(TimeSpan.FromSeconds(attackDuration), cancellationToken: cancellationToken);

        if (_animator == null) return;

    }


    private void SpawnSkill()
    {
        if(_battleManager == null)
        {
            _battleManager = new BattleManager();

        }


        _battleManager.SpawnSkillAsync(
            _vm.SkillPrefabAddress,
            _skillTransform,
            this.transform
        ).Forget();
    }


    private bool IsStateChangeable(EnemyBattleState newState)
    {
        // 예외처리 전용 (특정 상태일때는 현재 상태가 어떤지에 따라 전환 못하게 미리 막음)
        if (newState == EnemyBattleState.Walk)
        {
            if (_currentStateEnum == EnemyBattleState.Attack)
            {
                return false;
            }
        }

        return true;
    }


    private void PlayStateAnimation(EnemyBattleState state)
    {
        if (_animator == null) return;

        switch (state)
        {
            case EnemyBattleState.Idle:
                ResetBoolParameters();
                break;

            case EnemyBattleState.Walk:
                ResetBoolParameters();
                _animator.SetBool(WalkHash, true);
                break;

            case EnemyBattleState.Run:
                ResetBoolParameters();
                _animator.SetBool(RunHash, true);
                break;

            case EnemyBattleState.Attack:
                // 일회성 트리거 예시
                _animator.SetTrigger(AttackHash);
                break;

            case EnemyBattleState.Die:
                _animator.SetTrigger(DieHash);
                break;
        }
    }

    // Bool 파라미터들을 초기화해주는 편의 메서드
    private void ResetBoolParameters()
    {
        _animator.SetBool(WalkHash, false);
        _animator.SetBool(RunHash, false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            behaviorGraphAgent.SetVariableValue("Target", other.gameObject);

        }
    }
}
