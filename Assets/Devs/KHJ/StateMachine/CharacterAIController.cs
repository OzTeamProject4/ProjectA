using UnityEngine;

public class CharacterAIController : MonoBehaviour
{
    private StateMachine _stateMachine;
    private BattleCharacter _curCharacter;
    private BattleCharacter _targetCharacter;
    private IState _idleState;
    private IState _followState;
    private float _followDistance = 10.0f; // TODO 희준 임시 거리값, 추후 조정 필요
    private float _stopDistance = 5.0f;

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

        else
        {
            if (distance <_stopDistance)
            {
                _stateMachine.ChangeState(_idleState);
            }
        }

        _stateMachine.Update();
    }

    public void Initialize(BattleCharacter targetCharacter, BattleCharacter curCharacter)
    {
        _curCharacter = curCharacter;
        SetAIFollowTarget(targetCharacter);
        _idleState = new IdleState(curCharacter);
        _stateMachine.ChangeState(_idleState);
    }

    public void SetAIFollowTarget(BattleCharacter targetCharacter)
    {
        _targetCharacter = targetCharacter;
        _followState = new FollowState(targetCharacter, _curCharacter);
    }
}
