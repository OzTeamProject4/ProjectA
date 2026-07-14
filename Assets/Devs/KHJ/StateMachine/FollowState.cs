using UnityEngine;

public class FollowState : IState
{
    private CharacterAIController _controller;

    public FollowState(CharacterAIController controller)
    {
        _controller = controller;
    }
    public void Enter()
    {

    }

    public void Update()
    {
        BattleCharacter target = _controller.TargetCharacter;
        BattleCharacter curCharacter = _controller.CurCharacter;

        Vector3 direction = (target.transform.position - curCharacter.transform.position).normalized;
        curCharacter.Move(direction, false);
    }

    public void Exit() 
    {

    }
}
