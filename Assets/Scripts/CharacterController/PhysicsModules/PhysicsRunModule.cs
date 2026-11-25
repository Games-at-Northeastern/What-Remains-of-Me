using UnityEngine;

namespace PlayerController
{
    public class PhysicsRunModule : PhysicsHorizontalModule
    {
        [SerializeField, Tooltip("Multiplies the grounded acceleration from PlayerSettings")]
        private float accelerationMultiplier = 1.3f;
        [SerializeField] private float groundDeceleration = 65f;
        [SerializeField, Tooltip("Acceleration used when pressing against the current direction of movement on the ground")]
        private float turnAcceleration = 110f;

        private readonly ContactPoint2D[] contacts = new ContactPoint2D[8];
        private Rigidbody2D body;
        private PhysicsSurface surface;

        protected override void OnInitialize() => body = GetComponent<Rigidbody2D>();

        protected override bool AppliesTo(PlayerPhysicsContext context) => context.IsGrounded;

        public override bool UpdateMovement(PlayerPhysicsContext context)
        {
            surface = context.IsGrounded ? GetSurface() : null;
            return base.UpdateMovement(context);
        }

        protected override float GetVelocityX(float velocityX, float moveInput, PlayerSettings stats, float dt)
        {
            float accelerationScale = surface != null ? surface.AccelerationMultiplier : 1f;
            float decelerationScale = surface != null ? surface.DecelerationMultiplier : 1f;

            float target = moveInput * stats.maxRunSpeed;
            float rate;
            if (moveInput != 0 && velocityX != 0 && Mathf.Sign(moveInput) != Mathf.Sign(velocityX))
            {
                rate = turnAcceleration * decelerationScale;
            }
            else if (Mathf.Abs(target) > Mathf.Abs(velocityX))
            {
                rate = stats.groundedAcceleration * accelerationMultiplier * accelerationScale;
            }
            else
            {
                rate = groundDeceleration * decelerationScale;
            }

            return Mathf.MoveTowards(velocityX, target, rate * dt);
        }

        private PhysicsSurface GetSurface()
        {
            int count = body.GetContacts(contacts);
            for (int i = 0; i < count; i++)
            {
                if (Mathf.Abs(contacts[i].normal.y) > 0.5f && contacts[i].point.y < body.worldCenterOfMass.y)
                {
                    Collider2D ground = contacts[i].collider.attachedRigidbody == body ? contacts[i].otherCollider : contacts[i].collider;
                    return ground.GetComponentInParent<PhysicsSurface>();
                }
            }

            return null;
        }
    }
}
