using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BattleCharacter _battleCharacter;
    private PlayerInputActions _inputAction;
    private Vector3 _moveDirection;
    private Transform _cameraTransform;


    private void Awake()
    {
        // TODO 희준 카메라와 플레이어 입력 중복 추후 매니저 준비시 통일화
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
        if (_cameraTransform == null)
        {
            return;
        }

        Vector2 input = _inputAction.Player.Move.ReadValue<Vector2>();
        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;

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

    public void SetCameraTransform(Transform cameraTransform)
    {
        _cameraTransform = cameraTransform;
    }
}
