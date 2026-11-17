using UnityEngine;

namespace PlayerController
{
    public class PhysicsJumpModule : PlayerPhysicsModule
    {
        [SerializeField, Tooltip("Gravity while moving up and holding jump")]
        private float risingGravity = 20f;
        [SerializeField] private float fallGravity = 40f;

        public override int Order => 200;

        public override bool UpdateMovement(PlayerPhysicsContext context)
        {
            PlayerSettings stats = context.Stats;
            float velocityY = context.Velocity.y;

            if (context.JumpTriggered || CanStartJump(context))
            {
                context.UseJump();
                context.IsJumping = true;
                context.IsGrounded = false;
                context.TimeSinceGrounded = Mathf.Infinity;
                context.State = PlayerController2D.PlayerState.Aerial;
                Player.OnMovingPlatform = false;
                velocityY = Mathf.Sqrt(2f * risingGravity * stats.jumpHeight);
            }

            if (velocityY > 0 && Player.TouchingCeiling())
            {
                velocityY = 0;
            }

            bool useRisingGravity = velocityY > 0 && (!context.IsJumping || context.JumpHeld);
            float gravity = useRisingGravity ? risingGravity : fallGravity;
            context.Velocity.y = Mathf.Max(velocityY - gravity * context.DeltaTime, -Mathf.Abs(stats.terminalVelocity));
            return false;
        }

        private static bool CanStartJump(PlayerPhysicsContext context)
        {
            bool canJump = context.IsGrounded || context.TimeSinceGrounded <= context.Stats.coyoteTime;
            return canJump && !context.IsJumping && context.JumpBuffered;
        }
    }
}
