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
public class MovementSettings : MonoBehaviour
{
    // SERIALIZED FIELDS

    [Header("General")]
    [SerializeField] float runToIdleSpeed; // Speed which the player must be below to go from run to idle

    [Header("Jump")]
    [SerializeField] float jumpLandableTimer;
    [SerializeField] float jumpInitVelY;
    [SerializeField] float jumpInitGravity;
    [SerializeField] float jumpMaxGravity;
    [SerializeField] float jumpGravityIncRate;
    [SerializeField] float jumpMinSpeedY;
    [SerializeField] float jumpCancelYVelMultiplier;

    [Header("Fall")]
    [SerializeField] float fallGravity;
    [SerializeField] float fallMaxSpeedX;
    [SerializeField] float fallSmoothTimeX;
    [SerializeField] float fallMinSpeedY;

    [Header("Run")]
    [SerializeField] float runMaxSpeed;
    [SerializeField] float runSmoothTime;

    [Header("Wall Slide")]
    [SerializeField] float wallSlideSpeed;

    [Header("Wall Jump")]
    [SerializeField] float wallJumpLandableTimer;
    [SerializeField] float wallJumpInitVelX;
    [SerializeField] float wallJumpMaxSpeedX;
    [SerializeField] float wallJumpSmoothTimeX;
    [SerializeField] float wallJumpInitVelY;
    [SerializeField] float wallJumpMinSpeedY;
    [SerializeField] float wallJumpInitGravity;
    [SerializeField] float wallJumpMaxGravity;
    [SerializeField] float wallJumpGravityIncRate;
    [SerializeField] float wallJumpCancelYVelMultiplier;

    [Header("Dash")]
    [SerializeField] float dashSpeedX;
    [SerializeField] float dashTime;

    [Header("Wire Swing")]
    [SerializeField] float wireSwingBounceDecayMultiplier;
    [SerializeField] float wireSwingDecayMultiplier;
    [SerializeField] float wireSwingNaturalAccelMultiplier;
    [SerializeField] float wireSwingManualAccelMultiplier;
    [SerializeField] float wireSwingAngularVelOfDash;
    [SerializeField] float wireSwingReferenceWireLength;

    [Header("Wire Swing Release")]
    [SerializeField] float wsrGravity;
    [SerializeField] float wsrMaxSpeedX;
    [SerializeField] float wsrSmoothTimeX;
    [SerializeField] float wsrMinSpeedY;

    [Header("Wire General")]
    [SerializeField] float wireGeneralMaxDistance;

    [Header("Knockback")]
    [SerializeField] float knockbackVertVel;
    [SerializeField] float absKnockbackHorizVel;
    [SerializeField] float knockbackGravity;
    [SerializeField] float knockbackGravityHorizOnRecovery;

    // ACCESSIBLE FIELDS

    // General
    public float RunToIdleSpeed { get { return runToIdleSpeed; } } // Speed which the player must be below to go from run to idle

    // Jump
    public float JumpLandableTimer { get { return jumpLandableTimer; } }
    public float JumpInitVelY { get { return jumpInitVelY; } }
    public float JumpInitGravity { get { return jumpInitGravity; } }
    public float JumpMaxGravity { get { return jumpMaxGravity; } }
    public float JumpGravityIncRate { get { return jumpGravityIncRate; } }
    public float JumpMinSpeedY { get { return jumpMinSpeedY; } }
    public float JumpCancelYVelMultiplier { get { return jumpCancelYVelMultiplier; } }

    // Fall
    public float FallGravity { get { return fallGravity; } }
    public float FallMaxSpeedX { get { return fallMaxSpeedX; } }
    public float FallSmoothTimeX { get { return fallSmoothTimeX; } }
    public float FallMinSpeedY { get { return fallMinSpeedY; } }

    // Run
    public float RunMaxSpeed { get { return runMaxSpeed; } }
    public float RunSmoothTime { get { return runSmoothTime; } }

    // Wall Slide
    public float WallSlideSpeed { get { return wallSlideSpeed; } }

    // Wall Jump
    public float WallJumpLandableTimer { get { return wallJumpLandableTimer; } }
    public float WallJumpInitVelX { get { return wallJumpInitVelX; } }
    public float WallJumpMaxSpeedX { get { return wallJumpMaxSpeedX; } }
    public float WallJumpSmoothTimeX { get { return wallJumpSmoothTimeX; } }
    public float WallJumpInitVelY { get { return wallJumpInitVelY; } }
    public float WallJumpMinSpeedY { get { return wallJumpMinSpeedY; } }
    public float WallJumpInitGravity { get { return wallJumpInitGravity; } }
    public float WallJumpMaxGravity { get { return wallJumpMaxGravity; } }
    public float WallJumpGravityIncRate { get { return wallJumpGravityIncRate; } }
    public float WallJumpCancelYVelMultiplier { get { return wallJumpCancelYVelMultiplier; } }

    // Dash
    public float DashSpeedX { get { return dashSpeedX; } }
    public float DashTime { get { return dashTime; } }

    // Wire Swing
    public float WireSwingBounceDecayMultiplier { get { return wireSwingBounceDecayMultiplier; } }
    public float WireSwingDecayMultiplier { get { return wireSwingDecayMultiplier; } }
    public float WireSwingNaturalAccelMultiplier { get { return wireSwingNaturalAccelMultiplier; } }
    public float WireSwingManualAccelMultiplier { get { return wireSwingManualAccelMultiplier; } }
    public float WireSwingAngularVelOfDash { get { return wireSwingAngularVelOfDash; } }
    public float WireSwingReferenceWireLength { get { return wireSwingReferenceWireLength; } }

    // Wire Swing Release
    public float WsrGravity { get { return wsrGravity; } }
    public float WsrMaxSpeedX { get { return wsrMaxSpeedX; } }
    public float WsrSmoothTimeX { get { return wsrSmoothTimeX; } }
    public float WsrMinSpeedY { get { return wsrMinSpeedY; } }

    // Wire General
    public float WireGeneralMaxDistance { get { return wireGeneralMaxDistance; } }

    // Knockback
    public float KnockbackVertVel { get { return knockbackVertVel; } }
    public float AbsKnockbackHorizVel { get { return absKnockbackHorizVel; } }
    public float KnockbackGravity { get { return knockbackGravity; } }
    public float KnockbackGravityHorizOnRecovery { get { return knockbackGravityHorizOnRecovery; } }
}
