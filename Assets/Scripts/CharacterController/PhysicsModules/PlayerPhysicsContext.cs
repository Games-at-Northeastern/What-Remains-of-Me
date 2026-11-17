using UnityEngine;

namespace PlayerController
{
    /// <summary>
    /// Player state that the movement modules read and change during a physics step.
    /// </summary>
    public class PlayerPhysicsContext
    {
        public Vector2 Velocity;
        public PlayerController2D.PlayerState State;
        public bool IsGrounded;
        public bool IsJumping;
        public float TimeSinceGrounded = Mathf.Infinity;

        private float timeSinceJumpPressed = Mathf.Infinity;
        private float timeSinceJumpUsed = Mathf.Infinity;

        public PlayerSettings Stats { get; private set; }
        public float DeltaTime { get; private set; }
        public float MoveInput { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool JumpTriggered { get; private set; }

        public bool JumpBuffered => timeSinceJumpPressed < Stats.jumpBuffer && timeSinceJumpPressed < timeSinceJumpUsed;

        public void UseJump()
        {
            timeSinceJumpUsed = 0;
            JumpTriggered = false;
        }

        public void BeginStep(Vector2 velocity, bool grounded, PlayerSettings stats, PlayerInputHandler input, bool jumpTriggered, float deltaTime)
        {
            Velocity = velocity;
            IsGrounded = grounded;
            Stats = stats;
            DeltaTime = deltaTime;
            MoveInput = input.GetMoveInput().x;
            JumpHeld = input.IsJumpHeld();
            JumpTriggered = jumpTriggered;
            timeSinceJumpPressed = input.TimeSinceJumpWasPressed;
            State = grounded ? PlayerController2D.PlayerState.Grounded : PlayerController2D.PlayerState.Aerial;

            if (grounded)
            {
                TimeSinceGrounded = 0;
                IsJumping = false;
            }
            else
            {
                TimeSinceGrounded += deltaTime;
            }
        }

        public void EndStep() => timeSinceJumpUsed += DeltaTime;

        public void Reset()
        {
            IsGrounded = false;
            IsJumping = false;
            JumpTriggered = false;
            TimeSinceGrounded = Mathf.Infinity;
            timeSinceJumpUsed = Mathf.Infinity;
        }
    }
}
