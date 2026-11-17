using UnityEngine;

namespace PlayerController
{
    public class PhysicsAirModule : PhysicsHorizontalModule
    {
        [SerializeField] private float airAcceleration = 45f;
        [SerializeField] private float airDeceleration = 10f;
        [SerializeField] private float airTurnAcceleration = 60f;
        [SerializeField, Tooltip("How fast speed above the max air speed (e.g. after releasing the wire) bleeds off while holding that direction")]
        private float overspeedDeceleration = 5f;
        [SerializeField, Tooltip("How fast speed above the max air speed bleeds off when no direction is held")]
        private float overspeedDecelerationWithoutInput = 20f;

        protected override bool AppliesTo(PlayerPhysicsContext context) => !context.IsGrounded;

        protected override float GetVelocityX(float velocityX, float moveInput, PlayerSettings stats, float dt)
        {
            float maxSpeed = stats.maxAirSpeed;
            bool isTurning = moveInput != 0 && velocityX != 0 && Mathf.Sign(moveInput) != Mathf.Sign(velocityX);

            if (!isTurning && Mathf.Abs(velocityX) > maxSpeed)
            {
                float deceleration = moveInput != 0 ? overspeedDeceleration : overspeedDecelerationWithoutInput;
                return Mathf.MoveTowards(velocityX, Mathf.Sign(velocityX) * maxSpeed, deceleration * dt);
            }

            float target = moveInput * maxSpeed;
            float rate;
            if (isTurning)
            {
                rate = airTurnAcceleration;
            }
            else if (Mathf.Abs(target) > Mathf.Abs(velocityX))
            {
                rate = airAcceleration;
            }
            else
            {
                rate = airDeceleration;
            }

            return Mathf.MoveTowards(velocityX, target, rate * dt);
        }
    }
}
