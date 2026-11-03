using UnityEngine;

namespace PlayerController
{
    /// <summary>
    /// Player controller with the reworked movement physics.
    /// Adds faster turning, separate air control, momentum that carries over in the air, a height-based jump with
    /// heavier falling gravity and a wire swing that only limits the player's velocity, so collisions always win over
    /// the wire and releasing it keeps exactly the speed the player had.
    /// Run and air speed, ground acceleration, jump height, coyote time, jump buffer, fall speed limit and wire length
    /// are read from the active PlayerSettings every physics step, so god mode and upgrades still apply.
    /// </summary>
    public class PlayerPhysicsController2D : PlayerController2D
    {
        [Header("Physics: Run")]
        [SerializeField] private float groundDeceleration = 55f;
        [SerializeField, Tooltip("Acceleration used when pressing against the current direction of movement on the ground")]
        private float turnAcceleration = 110f;

        [Header("Physics: Air")]
        [SerializeField] private float airAcceleration = 45f;
        [SerializeField] private float airDeceleration = 10f;
        [SerializeField] private float airTurnAcceleration = 60f;
        [SerializeField, Tooltip("How fast speed above the max air speed (e.g. after releasing the wire) bleeds off")]
        private float overspeedDeceleration = 5f;

        [Header("Physics: Jump")]
        [SerializeField, Tooltip("Gravity while moving up and holding jump")]
        private float risingGravity = 20f;
        [SerializeField] private float fallGravity = 40f;

        [Header("Physics: Wire Swing")]
        [SerializeField] private float minWireLength = 1.5f;
        [SerializeField, Tooltip("How close to full length the wire has to be before swing input is applied")]
        private float tautTolerance = 0.1f;
        [SerializeField, Tooltip("Fastest speed the wire pulls the player back in after being stretched (e.g. by a collision)")]
        private float maxPullInSpeed = 6f;
        [SerializeField, Tooltip("Acceleration along the swing from holding left/right")]
        private float swingAcceleration = 14f;
        [SerializeField, Tooltip("Speed above which the swing is gently slowed down")]
        private float maxSwingSpeed = 16f;
        [SerializeField] private float overspeedDamping = 20f;
        [SerializeField, Range(0f, 2f)] private float swingDrag = 0.1f;
        [SerializeField] private float maxReleaseSpeed = 18f;

        private Rigidbody2D body;
        private PlayerInputHandler inputHandler;
        private WireThrower wireThrower;
        private DistanceJoint2D wireJoint;
        private bool initialized;

        private bool isGrounded;
        private bool isJumping;
        private float timeSinceGrounded = Mathf.Infinity;
        private float timeSinceJumpUsed = Mathf.Infinity;
        private bool wasConnected;
        private float wireLength;

        private bool IsWireConnected => wireThrower != null && wireThrower.IsConnected();

        private void FixedUpdate()
        {
            if (!initialized)
            {
                Initialize();
            }

            FlagUpdates();

            PlayerSettings stats = PlayerRef.PlayerManager.PlayerStats;
            float dt = Time.fixedDeltaTime;

            Vector2 velocity = body.linearVelocity - ExternalVelocity();
            UpdateGrounded(velocity.y, dt);
            UpdateWireConnection(stats);

            if (IsWireConnected && !isGrounded)
            {
                currentState = PlayerState.Swinging;
                isJumping = false;

                if (JumpBuffered(stats))
                {
                    timeSinceJumpUsed = 0;
                    wireThrower.DisconnectWire();
                    velocity = Vector2.ClampMagnitude(velocity, maxReleaseSpeed);
                }
                else
                {
                    wireThrower.ConnectedOutlet.UpdateOutletForce(this);
                    velocity = Swing(velocity, inputHandler.GetMoveInput().x, dt);
                }
            }
            else
            {
                currentState = isGrounded ? PlayerState.Grounded : PlayerState.Aerial;
                velocity.x = GetHorizontalVelocity(velocity.x, stats, dt);
                velocity.y = GetVerticalVelocity(velocity.y, stats, dt);
            }

            if (IsWireConnected)
            {
                velocity = ConstrainToWire(velocity, dt);
            }

            InternalVelocity = velocity;
            CombineCurrentVelocities();
            timeSinceJumpUsed += dt;
        }

        private void Initialize()
        {
            body = GetComponent<Rigidbody2D>();
            inputHandler = GetComponent<PlayerInputHandler>();
            wireThrower = GetComponentInChildren<WireThrower>();
            wireJoint = GetComponent<DistanceJoint2D>();

            LevelManager.OnPlayerReset.AddListener(ResetMovement);
            LevelManager.OnPlayerDeath.AddListener(ResetMovement);

            initialized = true;
        }

        private void ResetMovement()
        {
            isGrounded = false;
            isJumping = false;
            timeSinceGrounded = Mathf.Infinity;
            timeSinceJumpUsed = Mathf.Infinity;
            wasConnected = false;
        }

        private void UpdateGrounded(float velocityY, float dt)
        {
            bool wasGrounded = isGrounded;
            isGrounded = velocityY <= 0.1f && TouchingGround();

            if (isGrounded)
            {
                timeSinceGrounded = 0;
                isJumping = false;

                if (!wasGrounded)
                {
                    PlayLandingNoise();
                }
            }
            else
            {
                timeSinceGrounded += dt;
            }
        }

        private void UpdateWireConnection(PlayerSettings stats)
        {
            bool connected = IsWireConnected;

            if (connected && !wasConnected)
            {
                float distance = Vector2.Distance(body.position, wireThrower.ConnectedOutlet.transform.position);
                wireLength = Mathf.Clamp(distance, minWireLength, Mathf.Max(minWireLength, stats.wireLength));
            }

            if (connected && wireJoint != null)
            {
                wireJoint.enabled = false;
            }

            wasConnected = connected;
        }

        private bool JumpBuffered(PlayerSettings stats)
        {
            float timeSinceJumpPressed = inputHandler.TimeSinceJumpWasPressed;
            return timeSinceJumpPressed < stats.jumpBuffer && timeSinceJumpPressed < timeSinceJumpUsed;
        }

        private float GetHorizontalVelocity(float velocityX, PlayerSettings stats, float dt)
        {
            float moveInput = inputHandler.GetMoveInput().x;
            float maxSpeed = isGrounded ? stats.maxRunSpeed : stats.maxAirSpeed;
            bool isTurning = moveInput != 0 && velocityX != 0 && Mathf.Sign(moveInput) != Mathf.Sign(velocityX);

            if (!isGrounded && !isTurning && Mathf.Abs(velocityX) > maxSpeed)
            {
                velocityX = Mathf.MoveTowards(velocityX, Mathf.Sign(velocityX) * maxSpeed, overspeedDeceleration * dt);
            }
            else
            {
                float target = moveInput * maxSpeed;
                float rate;
                if (isTurning)
                {
                    rate = isGrounded ? turnAcceleration : airTurnAcceleration;
                }
                else if (Mathf.Abs(target) > Mathf.Abs(velocityX))
                {
                    rate = isGrounded ? stats.groundedAcceleration : airAcceleration;
                }
                else
                {
                    rate = isGrounded ? groundDeceleration : airDeceleration;
                }

                velocityX = Mathf.MoveTowards(velocityX, target, rate * dt);
            }

            if ((velocityX < 0 && TouchingLeftWall()) || (velocityX > 0 && TouchingRightWall()))
            {
                velocityX = 0;
            }

            return velocityX;
        }

        private float GetVerticalVelocity(float velocityY, PlayerSettings stats, float dt)
        {
            bool canJump = isGrounded || timeSinceGrounded <= stats.coyoteTime;
            if (canJump && !isJumping && JumpBuffered(stats))
            {
                velocityY = Mathf.Sqrt(2f * risingGravity * stats.jumpHeight);
                isJumping = true;
                isGrounded = false;
                timeSinceJumpUsed = 0;
                timeSinceGrounded = Mathf.Infinity;
                OnMovingPlatform = false;
                currentState = PlayerState.Aerial;
            }

            if (velocityY > 0 && TouchingCeiling())
            {
                velocityY = 0;
            }

            bool useRisingGravity = velocityY > 0 && (!isJumping || inputHandler.IsJumpHeld());
            float gravity = useRisingGravity ? risingGravity : fallGravity;
            return Mathf.Max(velocityY - gravity * dt, -Mathf.Abs(stats.terminalVelocity));
        }

        /// <summary>
        /// Applies gravity and swing input for one physics step while hanging from the wire.
        /// </summary>
        private Vector2 Swing(Vector2 velocity, float moveInput, float dt)
        {
            velocity.y -= fallGravity * dt;

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

                velocity += tangent * (Mathf.Abs(moveInput) * Mathf.Abs(tangent.x) * swingAcceleration * dt);
            }

            velocity *= Mathf.Max(0f, 1f - swingDrag * dt);

            float speed = velocity.magnitude;
            if (speed > maxSwingSpeed)
            {
                velocity *= Mathf.MoveTowards(speed, maxSwingSpeed, overspeedDamping * dt) / speed;
            }

            return velocity;
        }

        /// <summary>
        /// Stops the velocity from carrying the player past the wire length. Only the outward part of the velocity
        /// is removed, so the swing keeps its speed along the arc.
        /// </summary>
        private Vector2 ConstrainToWire(Vector2 velocity, float dt)
        {
            Vector2 toPlayer = body.position - (Vector2)wireThrower.ConnectedOutlet.transform.position;
            float distance = toPlayer.magnitude;
            if (distance < 0.0001f)
            {
                return velocity;
            }

            Vector2 radial = toPlayer / distance;
            float outwardSpeed = Vector2.Dot(velocity, radial);
            float allowedOutwardSpeed = Mathf.Max((wireLength - distance) / dt, -maxPullInSpeed);
            if (outwardSpeed > allowedOutwardSpeed)
            {
                velocity -= radial * (outwardSpeed - allowedOutwardSpeed);
            }

            return velocity;
        }
    }
}
