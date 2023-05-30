using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives constants which allow moves to be customized from the editor. The
/// settings in this script should be accessible to the move scripts, so that
/// they can be used.
/// Note: Serialized fields and accessible fields are kept separate so that
/// the information here can be customized from the editor, but not changed
/// from any other script.
/// </summary>
[CreateAssetMenu]
public class MovementSettings : ScriptableObject
{
    // SERIALIZED FIELDS

    [Header("General")]
    [SerializeField] private float runToIdleSpeed; // Speed which the player must be below to go from run to idle

    [Header("Jump")]
    [SerializeField] private float jumpLandableTimer;
    [SerializeField] private float jumpInitVelY;
    [SerializeField] private float jumpInitGravity;
    [SerializeField] private float jumpMaxGravity;
    [SerializeField] private float jumpGravityIncRate;
    [SerializeField] private float jumpMinSpeedY;
    [SerializeField] private float jumpCancelYVelMultiplier;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBuffer;
    [SerializeField] private float jumpToSwingMinYVel;

    [Header("Fall")]
    [SerializeField] private float fallGravity;
    [SerializeField] private float fallMaxSpeedX;
    [SerializeField] private float fallSmoothTimeX;
    [SerializeField] private float fallMinSpeedY;

    [Header("Run")]
    [SerializeField] private float runMaxSpeed;
    [SerializeField] private float runSmoothTime;

    [Header("Wall Slide")]
    [SerializeField] private float wallSlideSpeed;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpLandableTimer;
    [SerializeField] private float wallJumpInitVelX;
    [SerializeField] private float wallJumpMaxSpeedX;
    [SerializeField] private float wallJumpSmoothTimeX;
    [SerializeField] private float wallJumpInitVelY;
    [SerializeField] private float wallJumpMinSpeedY;
    [SerializeField] private float wallJumpInitGravity;
    [SerializeField] private float wallJumpMaxGravity;
    [SerializeField] private float wallJumpGravityIncRate;
    [SerializeField] private float wallJumpCancelYVelMultiplier;

    [Header("Dash")]
    [SerializeField] private float dashSpeedX;
    [SerializeField] private float dashTime;

    [Header("Wire Swing")]
    [SerializeField] private float wireSwingBounceDecayMultiplier;
    [SerializeField] private float wireSwingDecayMultiplier;
    [SerializeField] private float wireSwingNaturalAccelMultiplier;
    [SerializeField] private float wireSwingManualAccelMultiplier;
    [SerializeField] private float wireSwingAngularVelOfDash;
    [SerializeField] private float wireSwingReferenceWireLength;
    [SerializeField] private float wireSwingMaxAngularVelocity;

    [Header("Wire Swing Release")]
    [SerializeField] private float wsrGravity;
    [SerializeField] private float wsrMaxSpeedX;
    [SerializeField] private float wsrSmoothTimeX;
    [SerializeField] private float wsrMinSpeedY;

    [Header("Wire General")]
    [SerializeField] private float wireGeneralMaxDistance;

    [Header("Knockback")]
    [SerializeField] private float knockbackVertVel;
    [SerializeField] private float absKnockbackHorizVel;
    [SerializeField] private float knockbackGravity;
    [SerializeField] private float knockbackGravityHorizOnRecovery;

    [SerializeField] private bool isOnPlatform = false;
    [SerializeField] private Rigidbody2D platromRb;

    // ACCESSIBLE FIELDS

    // General
    public float RunToIdleSpeed => runToIdleSpeed; // Speed which the player must be below to go from run to idle

    // Jump
    public float JumpLandableTimer => jumpLandableTimer;
    public float JumpInitVelY => jumpInitVelY;
    public float JumpInitGravity => jumpInitGravity;
    public float JumpMaxGravity => jumpMaxGravity;
    public float JumpGravityIncRate => jumpGravityIncRate;
    public float JumpMinSpeedY => jumpMinSpeedY;
    public float JumpCancelYVelMultiplier => jumpCancelYVelMultiplier;
    public float CoyoteTime => coyoteTime;
    public float JumpBuffer => jumpBuffer;
    /// <summary>
    /// When a player begins a jump already connected to an outlet,
    /// set the minimum vertical falling velocity for when a wire swing should begin
    /// </summary>
    public float JumpToSwingMinYVel => jumpToSwingMinYVel;

    // Fall
    public float FallGravity => fallGravity;
    public float FallMaxSpeedX => fallMaxSpeedX;
    public float FallSmoothTimeX => fallSmoothTimeX;
    public float FallMinSpeedY => fallMinSpeedY;

    // Run
    public float RunMaxSpeed => runMaxSpeed;
    public float RunSmoothTime => runSmoothTime;

    // Wall Slide
    public float WallSlideSpeed => wallSlideSpeed;

    // Wall Jump
    public float WallJumpLandableTimer => wallJumpLandableTimer;
    public float WallJumpInitVelX => wallJumpInitVelX;
    public float WallJumpMaxSpeedX => wallJumpMaxSpeedX;
    public float WallJumpSmoothTimeX => wallJumpSmoothTimeX;
    public float WallJumpInitVelY => wallJumpInitVelY;
    public float WallJumpMinSpeedY => wallJumpMinSpeedY;
    public float WallJumpInitGravity => wallJumpInitGravity;
    public float WallJumpMaxGravity => wallJumpMaxGravity;
    public float WallJumpGravityIncRate => wallJumpGravityIncRate;
    public float WallJumpCancelYVelMultiplier => wallJumpCancelYVelMultiplier;

    // Dash
    public float DashSpeedX => dashSpeedX;
    public float DashTime => dashTime;

    // Wire Swing
    public float WireSwingBounceDecayMultiplier => wireSwingBounceDecayMultiplier;
    public float WireSwingDecayMultiplier => wireSwingDecayMultiplier;
    public float WireSwingNaturalAccelMultiplier => wireSwingNaturalAccelMultiplier;
    public float WireSwingManualAccelMultiplier => wireSwingManualAccelMultiplier;
    public float WireSwingAngularVelOfDash => wireSwingAngularVelOfDash;
    public float WireSwingReferenceWireLength => wireSwingReferenceWireLength;
    public float WireSwingMaxAngularVelocity => wireSwingMaxAngularVelocity;

    // Wire Swing Release
    public float WsrGravity => wsrGravity;
    public float WsrMaxSpeedX => wsrMaxSpeedX;
    public float WsrSmoothTimeX => wsrSmoothTimeX;
    public float WsrMinSpeedY => wsrMinSpeedY;

    // Wire General
    public float WireGeneralMaxDistance => wireGeneralMaxDistance;

    // Knockback
    public float KnockbackVertVel => knockbackVertVel;
    public float AbsKnockbackHorizVel => absKnockbackHorizVel;
    public float KnockbackGravity => knockbackGravity;
    public float KnockbackGravityHorizOnRecovery => knockbackGravityHorizOnRecovery;

    // platform stuff

    public bool IsOnPlatform => isOnPlatform;
    public Rigidbody2D PlatromRb => platromRb;
}
