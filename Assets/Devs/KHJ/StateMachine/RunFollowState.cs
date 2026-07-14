using UnityEngine;

public class RunFollowState : IState
{
    private BattleCharacter _targetCharacter;
    private BattleCharacter _curCharacter;

    public RunFollowState(BattleCharacter targetCharacter, BattleCharacter curCharacter)
    {
        _targetCharacter = targetCharacter;
        _curCharacter = curCharacter;
    }
    public void Enter()
    {

    }
    public void Update()
    {
        Vector3 direction = (_targetCharacter.transform.position -  _curCharacter.transform.position).normalized;
        _curCharacter.Move(direction, true);
    }
    public void Exit()
    {

    }
}
