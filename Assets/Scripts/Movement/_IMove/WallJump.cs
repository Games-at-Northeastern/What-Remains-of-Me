using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a move in which the player is jumping away from a wall.
/// </summary>
public class WallJump : AMove
{
    float xVel;
    float xAccel; // Don't mess with this outside of calling Mathf.SmoothDamp
    float yVel;
    float gravity;
    float timePassed;
    bool jumpCanceled = false;

    bool dashInput;
    bool connectInput;
    bool damageInput;

    /// <summary>
    /// Constructs a wall jump, initializing whether the jump should happen
    /// to the right (true) or left (false)
    /// </summary>
    public WallJump(bool toRight)
    {
        CS.Player.Dash.performed += _ => dashInput = true;
        xVel = toRight ? MS.WallJumpInitVelX : -MS.WallJumpInitVelX;
        yVel = MS.WallJumpInitVelY;
        gravity = MS.WallJumpInitGravity;
        WT.onConnect.AddListener(() => connectInput = true);
        PH.OnDamageTaken.AddListener(() => damageInput = true);
        timePassed = 0;
    }

    public override void AdvanceTime()
    {
        // General
        timePassed += Time.deltaTime;
        // Horizontal
        xVel = Mathf.SmoothDamp(xVel, MS.WallJumpMaxSpeedX * CS.Player.Move.ReadValue<float>(), ref xAccel, MS.WallJumpSmoothTimeX);
        // Vertical
        gravity = Mathf.Clamp(gravity + (MS.WallJumpGravityIncRate * Time.deltaTime), MS.WallJumpInitGravity, MS.WallJumpMaxGravity);
        yVel -= gravity * Time.deltaTime;
        if (yVel < MS.WallJumpMinSpeedY)
        {
            yVel = MS.WallJumpMinSpeedY;
        }
        // Cancellation
        if (!jumpCanceled && CS.Player.Jump.ReadValue<float>() == 0)
        {
            CancelJump();
        }
    }

    /// <summary>
    /// Causes any consequences that should come from the jump being cancelled at
    /// this point in time. Takes in whether a cancellation would mean the jump going
    /// straight to zero in Y Velocity or not.
    /// </summary>
    private void CancelJump()
    {
        jumpCanceled = true;
        // yVel dipping below zero is the "point of no return" (as in, canceling the jump does nothing)
        if (yVel > 0)
        {
            gravity = MS.JumpMaxGravity;
            yVel *= MS.WallJumpCancelYVelMultiplier;
        }
    }

    public override float XSpeed()
    {
        return xVel;
    }

    public override float YSpeed()
    {
        return yVel;
    }

    public override IMove GetNextMove()
    {
        if (damageInput)
        {
            return new Knockback();
        }
        if (connectInput)
        {
            return new WireSwing(xVel, yVel);
        }
        if (dashInput && AMove.dashIsReset && UpgradeHandler.DashAllowed)
        {
            return new Dash();
        }
        if (MI.GroundDetector.isColliding() && Mathf.Abs(xVel) < MS.RunToIdleSpeed)
        {
            return new Idle();
        }
        else if (MI.GroundDetector.isColliding())
        {
            return new Run(xVel);
        }
        if ((MI.LeftWallDetector.isColliding() || MI.RightWallDetector.isColliding()) && timePassed > MS.WallJumpLandableTimer)
        {
            return new WallSlide();
        }
        return this;
    }

    public override AnimationType GetAnimationState()
    {
        return AnimationType.WALL_JUMP;
    }
}
