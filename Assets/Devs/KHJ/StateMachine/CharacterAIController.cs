using UnityEngine;

public class CharacterAIController : MonoBehaviour
{
    private StateMachine _stateMachine;
    private BattleCharacter _curCharacter;
    private BattleCharacter _targetCharacter;
    private IState _idleState;
    private IState _followState;
    private IState _runFollowState;
    private float _followDistance = 10.0f; // TODO 희준 임시 거리값, 추후 조정 필요
    private float _stopDistance = 5.0f;
    private float _runFollowDistance = 15.0f;
    private float _walkFollowDistance = 12.0f;

    public BattleCharacter CurCharacter
    {
        get
        {
            return _curCharacter;
        }
    }

    public BattleCharacter TargetCharacter
    { 
        get 
        { 
            return _targetCharacter; 
        } 
    }

    private void Awake()
    {
        _stateMachine = new StateMachine();
    }
    private void Update()
    {
        float distance = Vector3.Distance(_curCharacter.transform.position, _targetCharacter.transform.position);

        if (_stateMachine.CurrentState == _idleState)
        {
            if (distance > _followDistance)
            {
                _stateMachine.ChangeState(_followState);
            }
        }

        else if (_stateMachine.CurrentState == _followState)
        {
            if (distance < _stopDistance)
            {
                _stateMachine.ChangeState(_idleState);
            }

            else if (distance > _runFollowDistance)
            {
                _stateMachine.ChangeState(_runFollowState);
            }
        }

        else if (_stateMachine.CurrentState == _runFollowState)
        {
             if (distance < _stopDistance)
            {
                _stateMachine.ChangeState(_idleState);
            }

            else if (distance < _walkFollowDistance)
            {
                _stateMachine.ChangeState(_followState);
            }
        }

        _stateMachine.Update();
    }

    public void Initialize(BattleCharacter targetCharacter, BattleCharacter curCharacter)
    {
        _curCharacter = curCharacter;
        _targetCharacter = targetCharacter;

        _idleState = new IdleState(this);
        _followState = new FollowState(this);
        _runFollowState = new RunFollowState(this);

        _stateMachine.ChangeState(_idleState);
    }

    public void SetAIFollowTarget(BattleCharacter targetCharacter)
    {
        _targetCharacter = targetCharacter;
    }
}
