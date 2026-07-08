using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BattleCharacter _battleCharacter;
    private PlayerInputActions _inputAction;
    private Vector3 _moveDirection;


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
        Vector2 input = _inputAction.Player.Move.ReadValue<Vector2>();
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 direction = camForward * input.y + camRight * input.x;
        _moveDirection = direction;

        if (_battleCharacter == null)
        {
            return;
        }

        if (_inputAction.Player.Jump.WasPressedThisFrame())
        {
            _battleCharacter.Jump();

        }

    }

    private void FixedUpdate()
    {
        if (_battleCharacter == null)
        {
            return;
        }

        _battleCharacter.Move(_moveDirection);
    }


    public void SetControlTarget(BattleCharacter targetCharacter)
    {
        _battleCharacter = targetCharacter;
    }
}
