using UnityEngine;

namespace PlayerController
{
    public class PhysicsWireSwingModule : PlayerPhysicsModule
    {
        [SerializeField] private float minWireLength = 1.5f;
        [SerializeField, Tooltip("How close to full length the wire has to be before swing input is applied")]
        private float tautTolerance = 0.1f;
        [SerializeField, Tooltip("Fastest speed the wire pulls the player back in after being stretched (e.g. by a collision)")]
        private float maxPullInSpeed = 6f;
        [SerializeField, Tooltip("Gravity while hanging from the wire")]
        private float swingGravity = 40f;
        [SerializeField, Tooltip("Acceleration along the swing from holding left/right")]
        private float swingAcceleration = 8f;
        [SerializeField, Tooltip("Speed above which holding the direction of the swing no longer speeds it up. Holding the other way always slows it down")]
        private float maxSwingSpeed = 16f;
        [SerializeField, Range(0f, 1f), Tooltip("Highest point above the outlet, as a part of the wire length, that holding left/right can swing the player up to")]
        private float maxPumpHeight = 0.5f;
        [SerializeField, Range(0f, 2f)] private float swingDrag = 0.1f;
        [SerializeField] private float maxReleaseSpeed = 18f;

        private Rigidbody2D body;
        private WireThrower wireThrower;
        private DistanceJoint2D wireJoint;
        private bool wasConnected;
        private bool wireCaught;
        private float wireLength;

        public override int Order => 0;

        public bool IsConnected => wireThrower != null && wireThrower.IsConnected();

        protected override void OnInitialize()
        {
            body = GetComponent<Rigidbody2D>();
            wireThrower = GetComponentInChildren<WireThrower>();
            wireJoint = GetComponent<DistanceJoint2D>();
        }

        public override bool UpdateMovement(PlayerPhysicsContext context)
        {
            UpdateConnection(context.Stats);

            if (!IsConnected || context.IsGrounded)
            {
                return false;
            }

            context.State = PlayerController2D.PlayerState.Swinging;
            context.IsJumping = false;

            if (Player.ExtraSpeed != Vector2.zero && !Player.OnMovingPlatform)
            {
                context.Velocity.y += Player.ExtraSpeed.y;
                Player.ExtraSpeed = Vector2.zero;
            }

            if (context.JumpBuffered)
            {
                context.UseJump();
                context.IsJumping = true;
                context.Velocity = Release(context.Velocity);
            }
            else
            {
                context.Velocity = Swing(context.Velocity, context.MoveInput, context.DeltaTime);
            }

            return true;
        }

        public override void ConstrainMovement(PlayerPhysicsContext context)
        {
            if (IsConnected)
            {
                context.Velocity = ConstrainToWire(context.Velocity, context.IsGrounded, context.DeltaTime);
            }
        }

        public override void ResetState()
        {
            wasConnected = false;
            wireCaught = false;
        }

        public override void OnNewPhysicsDisabled()
        {
            if (IsConnected && wireJoint != null)
            {
                wireJoint.enabled = true;
                wireThrower.SetMaxWireLength(Vector2.Distance(body.position, wireThrower.ConnectedOutlet.transform.position));
            }
        }

        private void UpdateConnection(PlayerSettings stats)
        {
            bool connected = IsConnected;

            if (connected && !wasConnected)
            {
                float distance = Vector2.Distance(body.position, wireThrower.ConnectedOutlet.transform.position);
                wireLength = Mathf.Clamp(distance, minWireLength, Mathf.Max(minWireLength, stats.wireLength));
                wireCaught = false;
            }

            if (connected && wireJoint != null)
            {
                wireJoint.enabled = false;
            }

            wasConnected = connected;
        }

        private Vector2 Release(Vector2 velocity)
        {
            wireThrower.DisconnectWire();
            return Vector2.ClampMagnitude(velocity, maxReleaseSpeed);
        }

        /// <summary>
        /// Applies gravity and swing input for one physics step while hanging from the wire.
        /// </summary>
        private Vector2 Swing(Vector2 velocity, float moveInput, float dt)
        {
            wireThrower.ConnectedOutlet.UpdateOutletForce(Player);
            velocity.y -= swingGravity * dt;

            Vector2 toPlayer = body.position - (Vector2)wireThrower.ConnectedOutlet.transform.position;
            float distance = toPlayer.magnitude;
            if (moveInput != 0 && distance > 0.01f && distance >= wireLength - tautTolerance)
            {
                Vector2 radial = toPlayer / distance;
                Vector2 tangent = new Vector2(-radial.y, radial.x);
                if (Mathf.Sign(tangent.x) != Mathf.Sign(moveInput))
                {
                    tangent = -tangent;
                }

                bool speedingUp = Vector2.Dot(velocity, tangent) > 0;
                if (!speedingUp || CanSpeedUp(velocity, toPlayer))
                {
                    velocity += tangent * (Mathf.Abs(moveInput) * Mathf.Abs(tangent.x) * swingAcceleration * dt);
                }
            }

            velocity *= Mathf.Max(0f, 1f - swingDrag * dt);

            return velocity;
        }

        private bool CanSpeedUp(Vector2 velocity, Vector2 toPlayer)
        {
            float energy = 0.5f * velocity.sqrMagnitude + swingGravity * toPlayer.y;
            return velocity.magnitude < maxSwingSpeed && energy < swingGravity * maxPumpHeight * wireLength;
        }

        /// <summary>
        /// Stops the velocity from carrying the player past the wire length. Only the outward part of the velocity
        /// is removed, so the swing keeps its speed along the arc.
        /// </summary>
        private Vector2 ConstrainToWire(Vector2 velocity, bool grounded, float dt)
        {
            Vector2 toPlayer = body.position - (Vector2)wireThrower.ConnectedOutlet.transform.position;
            float distance = toPlayer.magnitude;
            if (distance < 0.0001f)
            {
                return velocity;
            }

            Vector2 radial = toPlayer / distance;
            float allowedOutwardSpeed = Mathf.Max((wireLength - distance) / dt, -maxPullInSpeed);

            if (grounded && allowedOutwardSpeed >= 0f)
            {
                if (velocity.x * radial.x > allowedOutwardSpeed && Mathf.Abs(radial.x) > 0.01f)
                {
                    velocity.x = allowedOutwardSpeed / radial.x;
                }

                return velocity;
            }

            Vector2 constrained = grounded ? new Vector2(velocity.x, Mathf.Max(velocity.y, 0f)) : velocity;
            float outwardSpeed = Vector2.Dot(constrained, radial);
            float targetOutwardSpeed = Mathf.Min(outwardSpeed, allowedOutwardSpeed);

            bool holdTaut = !grounded && wireCaught && distance >= wireLength - tautTolerance && targetOutwardSpeed < 0;
            if (holdTaut)
            {
                targetOutwardSpeed = Mathf.Min(0f, allowedOutwardSpeed);
            }

            if (targetOutwardSpeed < outwardSpeed || holdTaut)
            {
                wireCaught = true;
            }

            if (targetOutwardSpeed == outwardSpeed)
            {
                return velocity;
            }

            return constrained + radial * (targetOutwardSpeed - outwardSpeed);
        }
    }
}
