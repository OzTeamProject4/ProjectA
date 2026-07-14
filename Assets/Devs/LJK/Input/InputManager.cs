using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : BaseManager<InputManager>
{
    private InputActions _inputActions;

    public Vector2 MoveInput { get; private set; }

    public bool IsRunPressed { get; private set; }

    public event Action OnJumpPerformed;
    public event Action OnSwitchPerformed;

    public override UniTask InitializeAsync()
    {
        if(_inputActions == null)
        {
            InitializeInputActions();
            SubscribePlayerActions();
        }

        return UniTask.CompletedTask;
    }

    public void EnablePlayerActions()
    {
        _inputActions.Player.Enable();
    }

    public void DisablePlayerActions()
    {
        _inputActions.Player.Disable();
    }

    private void InitializeInputActions()
    {
        _inputActions = new InputActions();
    }

    private void SubscribePlayerActions()
    {
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;

        _inputActions.Player.Run.performed += OnRun;
        _inputActions.Player.Run.canceled += OnRun;

        _inputActions.Player.Jump.performed += OnJump;

        _inputActions.Player.Switch.performed += OnSwitch;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        Debug.Log(MoveInput);
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        IsRunPressed = context.action.IsPressed();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (OnJumpPerformed == null)
        {
            return;
        }

        OnJumpPerformed.Invoke();
    }

    private void OnSwitch(InputAction.CallbackContext context)
    {
        if (OnSwitchPerformed == null)
        {
            return;
        }

        OnSwitchPerformed.Invoke();
    }
}