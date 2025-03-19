using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLocker : MonoBehaviour
{
    private bool _mouseUsed = false;

    void Start()
    {
        // Hide and confine the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        // Check if the mouse has moved
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        if (mouseDelta.sqrMagnitude > 0.01f) // Check if mouse moved (use a small threshold)
        {
            _mouseUsed = true;
        }

        // Check if gamepad joystick is being used
        if (Gamepad.current != null)
        {
            Vector2 leftStick = Gamepad.current.leftStick.ReadValue();
            if (leftStick.sqrMagnitude > 0.01f)
            {
                _mouseUsed = false;
            }
        }

        // Lock cursor to the right center edge of the screen if gamepad is being used
        if (!_mouseUsed)
        {
            Cursor.lockState = CursorLockMode.Locked; // This locks the cursor in place
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined; // Allow normal movement when using mouse
            Cursor.visible = true;
        }
    }
}
