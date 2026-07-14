using System.Xml;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyBattleState
{
    Idle,
    Walk,
    Run,
    Attack,
    Die,
    
}
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform target;
    private NavMeshAgent agent;
    private EnemyBattleState _currentStateEnum;


    public EnemyViewModel vm;
    public Blackboard blackboard;
    public BehaviorGraphAgent behaviorGraphAgent;
    public bool IsIdle = true;

    private void Awake()
    {
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
       
    }

    [ContextMenu("Test")]
    public void Test()
    {
        IsIdle = !IsIdle;
        behaviorGraphAgent.SetVariableValue("IsIdle", IsIdle);

    }

    public void RequestAddExpToEnemy(int exp)
    {
        if (vm != null)
        {
            vm.TotalExp += exp;
        }
    }
    public void RequestAddStatToEnemy(int addDamageValue)
    {
        if (vm != null)
        {
            vm.BaseDamage += addDamageValue;
        }
    }
    /*private void UpdateAnimation(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                Animator_Enemy.CrossFade("Idle", 0.1f);
                break;
            case EnemyState.Patrol:
                Animator_Enemy.CrossFade("Walk", 0.1f);
                break;
            case EnemyState.Attack:
                Animator_Enemy.SetTrigger("IsAttack"); // 트리거 형태도 가능
                break;
            case EnemyState.Die:
                Animator_Enemy.SetTrigger("IsDie");
                break;
        }
    }*/

    private bool IsStateChangeable(EnemyBattleState newState)
    {
        // 예외처리 전용 (특정 상태일때는 현재 상태가 어떤지에 따라 전환 못하게 미리 막음)
        if (newState == EnemyBattleState.Attack)
        {
            if (_currentStateEnum == EnemyBattleState.Walk)
            {
                return false;
            }
        }

        return true;
    }
    public void ChangeState(EnemyBattleState newState)
    {
        

        if (IsStateChangeable(newState) == false)
        {
            return;
        }

        _currentStateEnum=newState;

        Debug.Log(_currentStateEnum);


    }
}
