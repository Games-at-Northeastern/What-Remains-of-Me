using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour {
    public enum InputDevice { KEYBOARD, GAMEPAD }
    public static InputDevice DEVICE { get; private set; } = InputDevice.KEYBOARD;

    private void Update() => DetermineInput();

    private void DetermineInput() {
        if (DEVICE != InputDevice.KEYBOARD && IsKeyboardMousePressed()) {
            DEVICE = InputDevice.KEYBOARD;
            Debug.Log("Keyboard/Mouse key pressed!");
        }

        if (DEVICE != InputDevice.GAMEPAD && IsGamepadPressed()) {
            DEVICE = InputDevice.GAMEPAD;
            Debug.Log("Gamepad button pressed!");
        }
    }

    private bool IsKeyboardMousePressed() => Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame
               || Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame;

    private bool IsGamepadPressed() {
        if (Gamepad.current == null) {
            return false;
        }


        foreach (var control in Gamepad.current.allControls) {
            if (control is ButtonControl button && button.wasPressedThisFrame) {
                return true; // A button on the gamepad was pressed
            }
        }
        return false;
    }
}
