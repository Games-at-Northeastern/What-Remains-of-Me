using UnityEngine;
namespace PlayerController
{
    /// <summary>
    ///     Settings for the playerMovement
    /// </summary>
    [CreateAssetMenu]
    public class PlayerSettings : ScriptableObject
    {
        // SERIALIZED FIELDS
        [Header("Energy")]
        public int maxEnergy;
        public int maxVirus;

        [Header("Grounded")]
        [Tooltip("the maximum speed the player can move on the ground")]
        public float maxRunSpeed;
        [Tooltip("the acceleration to reach maxRunSpeed")]
        [Min(0)]
        public float groundedAcceleration;
        [Tooltip("how fast the player slowdowns when the let go of a movement key")]
        public float groundedDeceleration;
        public float ExternalVelocityDecay = 1;
        [Header("Air")]
        [Tooltip("the max speed the player can be moving in the air")]
        [Min(0)]
        public float maxAirSpeed;
        [Tooltip("how fast the player speeds up in the air")]
        [Min(0)]
        public float airAcceleration;
        [Min(0)]
        [Tooltip("rate which playerslows down in air")]
        public float airDeceleration;
        [Tooltip("the fastest speed the player can fall")]
        public float terminalVelocity;
        [Tooltip("acceleration due to gravity while falling")]
        public float fallGravity;
        [Header("Jump")]
        [Tooltip("How high the player can jump")]
        public float jumpHeight;
        [Tooltip("The gravity exerted on the player while jumping")]
        public float risingGravity;
        [Tooltip("")]
        public float jumpEndedEarlyMultiplier;
        [Tooltip("how long the player has to jump after walking off a ledge")]
        public float coyoteTime;
        [Tooltip("how long a jump input is held for if pressed right before landing")]
        public float jumpBuffer;
        [Tooltip("Time at which maximum jump landing volume is played.")]
        public float landingVolumeTime;

        [Header("Wall Slide")]
        [Tooltip("the acceleration the player experiences when sliding on a wall")]
        public float wallSlideGravity;
        [Tooltip("The maximum speed the player will slide down a wall")]
        [Min(0)]
        public float maxWallSlideSpeed;

        [Header("Wall Jump")]
        [Tooltip("How high the player will go after a wallJump and the distance they have traveled horizontally at the apex")]
        public Vector2 WallJumpDistance;
        [Tooltip("amount of time to temporarily take away movement control from the player so they don't immediately guide the character back to the wall")]
        public float takeControlAwayTime;
        [Tooltip("")]
        public Bounds wallDetection;

        [Header("Dash")]
        [Tooltip("")]
        public float dashSpeedX;
        [Tooltip("")]
        public float dashTime;

        [Header("Wire Swing")]
        [Tooltip("")]
        public float wireSwingBounceDecayMultiplier;
        [Tooltip("")]
        public float wireSwingDecayMultiplier;
        [Tooltip("")]
        public float wireSwingNaturalAccelMultiplier;
        [Tooltip("The amount of acceleration to player swaying causes")]
        public float PlayerSwayAccel;
        [Tooltip("")]
        public float SwingMaxAngularVelocity;


        [Header("Wire General")]
        [Tooltip("")]
        public float wireLength;

        [Header("Knockback")]
        [Tooltip("")]
        public Vector2 defaultKnockBack;
        [Header("CollisionDetectors")]
        [Tooltip("X is the width Y is the height")]
        public Vector2 ceilingBounds;
        [Min(0)]
        public float ceilingOffset;

        [Tooltip("X is the width Y is the height")]
        public Vector2 sideBounds;
        [Min(0)]
        public float leftOffset;
        public float rightOffset;

        [Tooltip("X is the width Y is the height")]
        public Vector2 groundBounds;
        [Min(0)]
        public float groundOffset;
        [Tooltip("Layers to ignore collision with")]
        public LayerMask IgnoreLayers;
    }
}
