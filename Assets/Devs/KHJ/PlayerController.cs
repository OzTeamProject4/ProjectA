using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BattleCharacter))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;

    private BattleCharacter _battleCharacter;
    private PlayerInputActions _inputAction;
    private Vector3 _moveDirection;
    private bool _isRunning;

    private void Awake()
    {
        // TODO 희준 카메라와 플레이어 입력 중복 추후 매니저 준비시 통일화
        _inputAction = new PlayerInputActions();
        _battleCharacter = GetComponent<BattleCharacter>();
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

        _isRunning = _inputAction.Player.Run.IsPressed();
        
        if (_inputAction.Player.Jump.WasPressedThisFrame())
        {
            _battleCharacter.Jump();
        }
    }

    private void FixedUpdate()
    {
        _battleCharacter.Move(_moveDirection, _isRunning);
    }
}
