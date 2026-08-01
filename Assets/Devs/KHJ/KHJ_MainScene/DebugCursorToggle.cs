using UnityEngine;
using UnityEngine.InputSystem;

// TODO 희준: 개발용 커서 토글, 릴리즈 전 제거  
public class DebugCursorToggle : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.f1Key.wasPressedThisFrame == false)
        {
            return;
        }

        bool isLocked = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isLocked;
    }
}