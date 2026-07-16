
using UnityEngine;

public class IdleState : IState
{
    private BattleCharacter _battleCharacter;
    public IdleState(BattleCharacter battleCharacter)
    {
        _battleCharacter = battleCharacter;
    }
    public void Enter()
    {
        _battleCharacter.Move(Vector3.zero);
    }

    public void Update()
    {

    }

    public void Exit() 
    {
    
    }


}
