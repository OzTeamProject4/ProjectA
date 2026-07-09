using UnityEngine;

public class CharacterAIController : MonoBehaviour
{
    private StateMachine _stateMachine;
    private BattleCharacter _curCharacter;
    private BattleCharacter _targetCharacter;
    private IState _idleState;
    private IState _followState;
    private float _followDistance = 10.0f; // TODO 희준 임시 거리값, 추후 조정 필요

    private void Awake()
    {
        _stateMachine = new StateMachine();
    }
    private void Update()
    {
        float distance = Vector3.Distance(_curCharacter.transform.position, _targetCharacter.transform.position);

        if (distance > _followDistance)
        {
            _stateMachine.ChangeState(_followState);
        }

        else
        {
            _stateMachine.ChangeState(_idleState);
        }

        _stateMachine.Update();
    }

    public void Initialize(BattleCharacter targetCharacter, BattleCharacter curCharacter)
    {
        _curCharacter = curCharacter;
        _targetCharacter = targetCharacter;

        _idleState = new IdleState(curCharacter);
        _followState = new FollowState(targetCharacter, curCharacter);

        _stateMachine.ChangeState(_idleState);
    }
}
