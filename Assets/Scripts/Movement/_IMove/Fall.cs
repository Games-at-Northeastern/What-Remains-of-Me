using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a move whether the player is falling from some platform.
/// (Note that falling and jumping are completely different things; the player
/// never "falls" during any part of a jump.)
/// </summary>
public class Fall : AMove
{
    private float yVel;
    private float xVel;
    private float xAccel; // Don't mess with this outside of calling Mathf.SmoothDamp
    private static float coyoteTimeCounter = MS.CoyoteTime;
    private static float jumpBufferCounter = MS.JumpBuffer;

    private bool dashInput;
    private bool connectedInput;
    private bool swingInput;
    private bool damageInput;

    /// <summary>
    /// Initializes a fall, appropriately setting its vertical velocity to start
    /// with.
    /// </summary>
    public Fall(float initXVel, float initYVel)
    {
        xVel = initXVel;
        yVel = initYVel;
        CS.Player.Dash.performed += _ => dashInput = true;
        WT.onConnect.AddListener(() => connectedInput = true);
        CS.Player.Jump.performed += _ => { if (WT.ConnectedOutlet != null) { swingInput = true; } };
        PH.OnDamageTaken.AddListener(() => damageInput = true);
    }

    public Fall(bool disableCoyote)
    {
        coyoteTimeCounter = 0.0f;
    }

    public Fall() : this(0, 0)
    {
        // Default Constructor
    }

    public override void AdvanceTime()
    {
        var timeChange = Time.deltaTime;
        xVel = Mathf.SmoothDamp(xVel, MS.FallMaxSpeedX * CS.Player.Move.ReadValue<float>(), ref xAccel, MS.FallSmoothTimeX);
        yVel -= MS.FallGravity * timeChange;
        if (yVel < MS.FallMinSpeedY)
        {
            yVel = MS.FallMinSpeedY;
        }

        // Tracking Coyote Time
        coyoteTimeCounter -= timeChange;

        // Jump Buffer
        if (CS.Player.Jump.ReadValue<float>() > 0)
        {
            jumpBufferCounter -= timeChange;
        }
        else
        {
            jumpBufferCounter = MS.JumpBuffer;
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
        if (connectedInput || swingInput || WT.ConnectedOutlet != null)
        {
            return new WireSwing(xVel, yVel);
        }
        if (dashInput && AMove.dashIsReset && UpgradeHandler.DashAllowed)
        {
            coyoteTimeCounter = MS.CoyoteTime;
            return new Dash();
        }
        /*
        if ((MI.LeftWallDetector.isColliding() && MI.LeftWallDetector.CollidingWith() != null && MI.LeftWallDetector.CollidingWith().tag == "Ladder") ||
            (MI.RightWallDetector.isColliding() && MI.RightWallDetector.CollidingWith() != null && MI.RightWallDetector.CollidingWith().tag == "Ladder") &&
            yVel < 0)
        {
            return new Climb();
        }
        */


        // Deprecated code for wall jumping.
        /*if (MI.LeftWallDetector.isColliding() || MI.RightWallDetector.isColliding() && yVel < 0)
        {
            coyoteTimeCounter = MS.CoyoteTime;
            return new WallSlide();
        }*/
        if (MI.GroundDetector.isColliding() && jumpBufferCounter > 0 && jumpBufferCounter < MS.JumpBuffer)
        {
            return new Jump(xVel);
        }
        if (CS.Player.Jump.ReadValue<float>() > 0 && coyoteTimeCounter > 0.0f)
        {
            coyoteTimeCounter = MS.CoyoteTime;
            return new Jump(xVel);
        }
        if (MI.GroundDetector.isColliding() && Mathf.Abs(xVel) < MS.RunToIdleSpeed)
        {
            coyoteTimeCounter = MS.CoyoteTime;
            return new Idle();
        }
        if (MI.GroundDetector.isColliding())
        {
            coyoteTimeCounter = MS.CoyoteTime;
            return new Run(xVel);
        }
        return this;
    }

    public override AnimationType GetAnimationState() => AnimationType.JUMP_FALLING;
}
