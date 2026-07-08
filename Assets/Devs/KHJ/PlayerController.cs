using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BattleCharacter _battleCharacter;
    private PlayerInputActions _inputAction;

    private void Awake()
    {
        _inputAction = new PlayerInputActions();

    }

    private void OnEnable()
    {
        _inputAction.Player.Enable();
    }
    private void OnDisable()
    {
        _inputAction?.Player.Disable();
    }
    private void Update()
    {
        if (_battleCharacter == null)
        {
            return;
        }

        Vector2 input = _inputAction.Player.Move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(input.x, 0, input.y);
        _battleCharacter.Move(direction);
    }

    

    public void SetControlTarget(BattleCharacter targetCharacter)
    {
        _battleCharacter = targetCharacter;
    }
}
