using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 0.1f;

    private Transform _target;
    private PlayerInputActions _inputAction;
    private float _yaw;
    private float _pitch;

    private void Awake()
    {
        // TODO 희준 카메라와 플레이어 입력 중복 추후 매니저 준비시 통일화
        _inputAction = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputAction.Player.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        _inputAction?.Player.Disable();
    }
    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        transform.position = _target.position;
        Vector2 look = _inputAction.Player.Look.ReadValue<Vector2>();
        _yaw = _yaw + look.x * _mouseSensitivity;
        _pitch = _pitch - look.y * _mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, -30f, 70f);
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0);

        // 임시 : 개발용 커서 해제 코드
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
