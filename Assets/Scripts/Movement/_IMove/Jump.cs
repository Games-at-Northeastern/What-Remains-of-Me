using System;
using UnityEngine;

/// <summary>
///     Represents a move in which the player is jumping into the air.
/// </summary>
public class Jump : AMove
{
    private bool connectedInput;
    private bool damageInput;

    private bool dashInput;
    private float gravity;
    private bool jumpCanceled;
    private bool swingInput; // Jump input while connecting to an outlet
    private float timePassed;
    private float xAccel; // Don't mess with this outside of calling Mathf.SmoothDamp
    private float xVel;
    private float yVel;

    /// <summary>
    ///     Initializes a jump, appropriately setting its horizontal velocity to start
    ///     with.
    /// </summary>
    public Jump(float initXVel)
    {
        xVel = initXVel;
        yVel = MS.JumpInitVelY;
        gravity = MS.JumpInitGravity;
        timePassed = 0;
        CS.Player.Dash.performed += _ => dashInput = true;
        WT.onConnect.AddListener(() => connectedInput = true);
        CS.Player.Jump.performed += _ =>
        {
            if (WT.ConnectedOutlet != null) { swingInput = true; }
        };
        PH.OnDamageTaken.AddListener(() => damageInput = true);
    }

    public Jump() : this(0)
    {
        // Default Constructor
    }

    public override void AdvanceTime()
    {
        // General
        timePassed += Time.deltaTime;
        // Horizontal
        if (xVel > MS.FallMaxSpeedX * 0.85 && !connectedInput)
        {
            xVel = Mathf.SmoothDamp(xVel, MS.FallMaxSpeedX * CS.Player.Move.ReadValue<float>(),
                ref xAccel, MS.FallSmoothTimeX * 5);
        }
        else
        {
            xVel = Mathf.SmoothDamp(xVel, MS.FallMaxSpeedX * CS.Player.Move.ReadValue<float>(),
            ref xAccel, MS.FallSmoothTimeX);
        }
        // Vertical
        gravity = Mathf.Clamp(gravity + (MS.JumpGravityIncRate * Time.deltaTime),
            MS.JumpInitGravity, MS.JumpMaxGravity);
        yVel -= gravity * Time.deltaTime;
        if (yVel < MS.JumpMinSpeedY)
        {
            yVel = MS.JumpMinSpeedY;
        }

        // Cancellation
        if (!jumpCanceled && CS.Player.Jump.ReadValue<float>() == 0)
        {
            CancelJump(false);
        }

        if (!jumpCanceled && MI.CeilingDetector.isColliding())
        {
            CancelJump(true);
        }
    }

    /// <summary>
    ///     Causes any consequences that should come from the jump being cancelled at
    ///     this point in time. Takes in whether a cancellation would mean the jump going
    ///     straight to zero in Y Velocity or not.
    /// </summary>
    private void CancelJump(bool straightToZeroVel)
    {
        jumpCanceled = true;
        // yVel dipping below zero is the "point of no return" (as in, canceling the jump does nothing)
        if (yVel > 0)
        {
            gravity = MS.JumpMaxGravity;
            yVel = straightToZeroVel ? 0 : yVel * MS.JumpCancelYVelMultiplier;
        }
    }

    public override float XSpeed() => xVel;

    public override float YSpeed() => yVel;

    public override IMove GetNextMove()
    {
        if (damageInput)
        {
            return new Knockback();
        }

        if (connectedInput || swingInput)
        {
            return new WireSwing(xVel, yVel);
        }

        if (dashInput && dashIsReset && UpgradeHandler.DashAllowed)
        {
            return new Dash();
        }

        if ((MI.LeftWallDetector.isColliding() || MI.RightWallDetector.isColliding()) && yVel < 0)
        {
            return new WallSlide();
        }

        if (timePassed > MS.JumpLandableTimer && MI.GroundDetector.isColliding() &&
            Mathf.Abs(xVel) < MS.RunToIdleSpeed)
        {
            return new Idle();
        }

        if (MI.GroundDetector.isColliding() && timePassed > MS.JumpLandableTimer )
        {
            return new Run(xVel);
        }

        return this;
    }

    public override AnimationType GetAnimationState() =>
        yVel >= 0 ? AnimationType.JUMP_RISING : AnimationType.JUMP_FALLING;
}
