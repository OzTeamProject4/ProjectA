using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BattleCharacter))]
public class PlayerController : MonoBehaviour
{
<<<<<<< HEAD:Assets/Scripts/Character/PlayerController.cs
    private Transform _cameraTransform;
=======
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Attack _attack;


>>>>>>> master:Assets/Devs/KHJ/PlayerController.cs
    private BattleCharacter _battleCharacter;
    private Vector3 _moveDirection;
    private bool _isRunning;

    private void Awake()
    {
        // TODO 희준 카메라와 플레이어 입력 중복 추후 매니저 준비시 통일화
        _battleCharacter = GetComponent<BattleCharacter>();

<<<<<<< HEAD:Assets/Scripts/Character/PlayerController.cs
        if (Camera.main == null)
        {
            Debug.LogError("MainCamer를 찾을수 없습니다");
            return;
        }

        _cameraTransform = Camera.main.transform;
=======
>>>>>>> master:Assets/Devs/KHJ/PlayerController.cs
    }

    private void OnEnable()
    {
        GameManager.Instance.InputManager.OnJumpPerformed += HandleJumpPerformed;
    }
    private void OnDisable()
    {
        GameManager.Instance.InputManager.OnJumpPerformed -= HandleJumpPerformed;
    }
    private void Update()
    {

        if (_inputAction.Player.Attack.WasPressedThisFrame())
        {
            _attack.AttackEnemy();
        }


        if (_cameraTransform == null)
        {
            return;
        }

        Vector2 input = GameManager.Instance.InputManager.MoveInput;
        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 direction = camForward * input.y + camRight * input.x;
        _moveDirection = direction;

        _isRunning = GameManager.Instance.InputManager.IsRunPressed;

    }

    private void FixedUpdate()
    {
        _battleCharacter.Move(_moveDirection, _isRunning);
    }

    private void HandleJumpPerformed()
    {
        _battleCharacter.Jump();
    }
}
