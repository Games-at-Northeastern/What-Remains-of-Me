using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace PlayerControllerRefresh
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public float TimeSinceJumpWasPressed { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool JumpCanceled { get; private set; }
        public Vector2 MoveInput { get; private set; }
        public bool DashPressed { get; private set; }

        public bool WireThrown { get; private set; }
        public float deadZone = 0.1f;

        // Update is called once per frame
        private void FixedUpdate()
        {
            //Makes sure jumpPressed is held for an entire fixed frame so if inputed outside of fixed frame won't be eaten
            JumpPressed = false;
            DashPressed = false;
            WireThrown = false;
            TimeSinceJumpWasPressed += Time.fixedDeltaTime;
        }


        public void Jump(InputAction.CallbackContext context)
        {
            JumpPressed = JumpPressed || context.started;
            if (context.started)
            {
                TimeSinceJumpWasPressed = 0;
            }
            JumpHeld = JumpPressed || context.performed;
        }

        public void Move(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            if (context.canceled || Mathf.Abs(MoveInput.x) < deadZone)
            {
                MoveInput = new Vector2(0, MoveInput.y);
            }
            if (context.canceled || Mathf.Abs(MoveInput.y) < deadZone)
            {
                MoveInput = new Vector2(MoveInput.x, 0);
            }
        }

        public void Dash(InputAction.CallbackContext context)
        {
            DashPressed = DashPressed || context.started;
        }

        public void Wire(InputAction.CallbackContext context)
        {
            WireThrown = WireThrown || context.started;
        }
    }
}
