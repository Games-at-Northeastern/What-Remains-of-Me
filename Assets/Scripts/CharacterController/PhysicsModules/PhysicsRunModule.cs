using UnityEngine;

namespace PlayerController
{
    public class PhysicsRunModule : PhysicsHorizontalModule
    {
        [SerializeField] private float groundDeceleration = 55f;
        [SerializeField, Tooltip("Acceleration used when pressing against the current direction of movement on the ground")]
        private float turnAcceleration = 110f;

        protected override bool AppliesTo(PlayerPhysicsContext context) => context.IsGrounded;

        protected override float GetVelocityX(float velocityX, float moveInput, PlayerSettings stats, float dt)
        {
            float target = moveInput * stats.maxRunSpeed;
            float rate;
            if (moveInput != 0 && velocityX != 0 && Mathf.Sign(moveInput) != Mathf.Sign(velocityX))
            {
                rate = turnAcceleration;
            }
            else if (Mathf.Abs(target) > Mathf.Abs(velocityX))
            {
                rate = stats.groundedAcceleration;
            }
            else
            {
                rate = groundDeceleration;
            }

            return Mathf.MoveTowards(velocityX, target, rate * dt);
        }
    }
}
