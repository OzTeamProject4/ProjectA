using UnityEngine;

public class FollowState : IState
{
    private BattleCharacter _targetCharacter;
    private BattleCharacter _curCharacter;

    public FollowState(BattleCharacter targetCharacter, BattleCharacter curCharacter)
    {
        _targetCharacter = targetCharacter;
        _curCharacter = curCharacter;
    }
    public void Enter()
    {

    }

    public void Update()
    {
        Vector3 direction = (_targetCharacter.transform.position - _curCharacter.transform.position).normalized;
        _curCharacter.Move(direction, false);
    }

    public void Exit() 
    {

    }
}
