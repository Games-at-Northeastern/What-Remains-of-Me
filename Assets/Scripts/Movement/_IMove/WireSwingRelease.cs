using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a move whether the player is falling from the wire swing.
/// </summary>
public class WireSwingRelease : AMove
{
    float yVel;
    float xVel;
    float xAccel; // Don't mess with this outside of calling Mathf.SmoothDamp

    bool dashInput;
    bool connectedInput;
    bool swingInput;
    bool damageInput;

    /// <summary>
    /// Initializes a wire swing release, appropriately setting its vertical velocity to start
    /// with.
    /// </summary>
    public WireSwingRelease(float initXVel, float initYVel)
    {
        xVel = initXVel;
        yVel = initYVel;
        CS.Player.Dash.performed += _ => dashInput = true;
        WT.onConnect.AddListener(() => connectedInput = true);
        CS.Player.Jump.performed += _ => { if (WT.connectedOutlet != null) { swingInput = true; } };
        PH.OnDamageTaken.AddListener(() => damageInput = true);
    }

    public WireSwingRelease() : this(0, 0)
    {
        // Default Constructor
    }

    public override void AdvanceTime()
    {
        xVel = Mathf.SmoothDamp(xVel, MS.WsrMaxSpeedX * CS.Player.Move.ReadValue<float>(), ref xAccel, MS.WsrSmoothTimeX);
        yVel -= MS.WsrGravity * Time.deltaTime;
        if (yVel < MS.WsrMinSpeedY)
        {
            yVel = MS.WsrMinSpeedY;
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
        if (connectedInput || swingInput)
        {
            return new WireSwing(xVel, yVel);
        }
        if (dashInput && AMove.dashIsReset && UpgradeHandler.DashAllowed)
        {
            return new Dash();
        }
        if (MI.LeftWallDetector.isColliding() || MI.RightWallDetector.isColliding() && yVel < 0)
        {
            return new WallSlide();
        }
        if (MI.GroundDetector.isColliding() && Mathf.Abs(xVel) < MS.RunToIdleSpeed)
        {
            return new Idle();
        }
        else if (MI.GroundDetector.isColliding())
        {
            return new Run(xVel);
        }
        return this;
    }

    public override AnimationType GetAnimationState()
    {
        return AnimationType.JUMP_FALLING;
    }
}
