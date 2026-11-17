namespace PlayerController
{
    /// <summary>
    /// Shared part of horizontal movement: moves relative to the speed carried over from a moving platform and stops
    /// at walls.
    /// </summary>
    public abstract class PhysicsHorizontalModule : PlayerPhysicsModule
    {
        public override int Order => 100;

        protected abstract bool AppliesTo(PlayerPhysicsContext context);

        protected abstract float GetVelocityX(float velocityX, float moveInput, PlayerSettings stats, float dt);

        public override bool UpdateMovement(PlayerPhysicsContext context)
        {
            if (!AppliesTo(context))
            {
                return false;
            }

            float carriedSpeed = Player.ExtraSpeed.x;
            float velocityX = GetVelocityX(context.Velocity.x - carriedSpeed, context.MoveInput, context.Stats, context.DeltaTime) + carriedSpeed;

            if ((velocityX < 0 && Player.TouchingLeftWall()) || (velocityX > 0 && Player.TouchingRightWall()))
            {
                velocityX = 0;
            }

            context.Velocity.x = velocityX;
            return false;
        }
    }
}
