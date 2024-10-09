using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace PlayerController
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public float TimeSinceJumpWasPressed { get; private set; }
        private bool jumpPressed;
        private bool jumpHeld;
        private bool jumpCanceled;
        private Vector2 moveInput;
        private bool dashPressed;

        private bool wireThrown;
        public float deadZone = 0.1f;

        private bool isLocked = false; // when locked, input handler wont return or process any inputs

        public bool IsJumpPressed() => jumpPressed && !isLocked;
        public bool IsJumpHeld() => jumpHeld && !isLocked;
        public bool IsJumpCanceled() => jumpCanceled && !isLocked;
        public Vector2 GetMoveInput() => isLocked ? new Vector2(0, 0) : moveInput;
        public bool IsDashPressed() => dashPressed && !isLocked;
        public bool IsWireThrown() => wireThrown && !isLocked;
        public void Lock()
        {
            isLocked = true;
            jumpPressed = false;
            jumpHeld = false;
            dashPressed = false;
            wireThrown = false;
            TimeSinceJumpWasPressed = Mathf.Infinity;
        }
        public void Unlock() => isLocked = false;

        // Update is called once per frame
        private void FixedUpdate()
        {
            //Makes sure jumpPressed is held for an entire fixed frame so if inputed outside of fixed frame won't be eaten
            jumpPressed = false;
            dashPressed = false;
            wireThrown = false;
            TimeSinceJumpWasPressed += Time.fixedDeltaTime;
        }

        public void Jump(InputAction.CallbackContext context)
        {
            jumpPressed = jumpPressed || context.started;
            if (context.started)
            {
                TimeSinceJumpWasPressed = 0;
            }
            jumpHeld = jumpPressed || context.performed;
        }

        public void Move(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
            if (context.canceled || Mathf.Abs(moveInput.x) < deadZone)
            {
                moveInput = new Vector2(0, moveInput.y);
            }
            if (context.canceled || Mathf.Abs(moveInput.y) < deadZone)
            {
                moveInput = new Vector2(moveInput.x, 0);
            }
        }

        public void Dash(InputAction.CallbackContext context)
        {
            dashPressed = dashPressed || context.started;
        }
        public void Wire(InputAction.CallbackContext context)
        {
            wireThrown = wireThrown || context.started;
        }
    }
}
