
using UnityEngine;

public class IdleState : IState
{
    private CharacterAIController _controller;
    public IdleState(CharacterAIController controller)
    {
        _controller = controller;
    }
    public void Enter()
    {
        _controller.CurCharacter.Move(Vector3.zero, false);
    }

    public void Update()
    {
        _controller.CurCharacter.Move(Vector3.zero, false);
    }

    public void Exit() 
    {
    
    }
}
