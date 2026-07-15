using Unity.Behavior;
using UnityEngine;

[RequireComponent(typeof(BehaviorGraphAgent))]
public class CharacterAIController : MonoBehaviour
{
    private BehaviorGraphAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<BehaviorGraphAgent>();
    }

    public void SetAIFollowTarget(BattleCharacter targetCharacter)
    {
        GameObject target = targetCharacter.gameObject;
        _agent.SetVariableValue("Target", target);
    }

    public void EnableAI()
    {
        _agent.enabled = true;
    }

    public void DisableAI()
    {
        _agent.enabled = false;
    }
}