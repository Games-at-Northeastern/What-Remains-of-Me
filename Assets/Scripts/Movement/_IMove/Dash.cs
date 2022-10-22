using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a move in which the player dashes, on air or on ground,
/// at a quick speed.
/// </summary>
public class Dash : AMove
{
    float xVel;
    float timePassed;

    bool connectedInput;
    bool swingInput;
    bool damageInput;

    /// <summary>
    /// Initializes a dash, taking in whether it is going to the right (true)
    /// or left (false).
    /// </summary>
    public Dash()
    {
        xVel = Flipped() ? -MS.DashSpeedX : MS.DashSpeedX;
        timePassed = 0;
        WT.onConnect.AddListener(() => connectedInput = true);
        CS.Player.Jump.performed += _ => { if (WT.ConnectedOutlet != null) { swingInput = true; } };
        PH.OnDamageTaken.AddListener(() => damageInput = true);
    }

    public override void AdvanceTime()
    {
        timePassed += Time.deltaTime;
    }

    public override float XSpeed()
    {
        return xVel;
    }

    public override float YSpeed()
    {
        return 0;
    }

    public override IMove GetNextMove()
    {
        if (damageInput)
        {
            return new Knockback();
        }
        if (connectedInput || swingInput)
        {
            return new WireSwing(xVel, 0);
        }
        if (timePassed > MS.DashTime)
        {
            AMove.dashIsReset = false;
            return new Fall();
        }
        if (MI.LeftWallDetector.isColliding() || MI.RightWallDetector.isColliding())
        {
            AMove.dashIsReset = false;
            return new WallSlide();
        }
        return this;
    }

    public override AnimationType GetAnimationState()
    {
        return AnimationType.DASH;
    }
}
