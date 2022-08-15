using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a move in which the player is moving across the ground.
/// </summary>
public class Run : AMove
{
    float xVel;
    float xAccel; // Don't mess with this outside of calling Mathf.SmoothDamp

    bool dashInput;
    bool damageInput;

    float currDistFromOutlet; // Current position from the player to the outlet, if wire connected

    /// <summary>
    /// Constructs a Run move, starting it off with the given horizontal speed.
    /// </summary>
    public Run(float initXVel)
    {
        AMove.dashIsReset = true;
        CS.Player.Dash.performed += _ => dashInput = true;
        PH.OnDamageTaken.AddListener(() => damageInput = true);
        xVel = initXVel;
    }

    public Run() : this(0)
    {
        // Default Constructor
    }

    public override void AdvanceTime()
    {
        xVel = Mathf.SmoothDamp(xVel, MS.RunMaxSpeed * CS.Player.Move.ReadValue<float>(), ref xAccel, MS.RunSmoothTime);
        if (WT.connectedOutlet != null)
        {
            Vector2 origPos = MI.transform.position;
            Vector2 connectedOutletPos = WT.connectedOutlet.transform.position;
            float newDistFromOutlet = Vector2.Distance(origPos, connectedOutletPos);
            if (newDistFromOutlet < currDistFromOutlet)
            {
                WT.SetMaxWireLength(newDistFromOutlet);
            }
            currDistFromOutlet = newDistFromOutlet;
        }
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
        if (dashInput && AMove.dashIsReset && UpgradeHandler.DashAllowed)
        {
            return new Dash();
        }
        if (CS.Player.Jump.ReadValue<float>() > 0)
        {
            return new Jump(xVel);
        }
        if (!MI.GroundDetector.isColliding())
        {
            return new Fall(xVel, 0);
        }
        /*
        if ((MI.LeftWallDetector.isColliding() && MI.LeftWallDetector.CollidingWith() != null && MI.LeftWallDetector.CollidingWith().tag == "Ladder") ||
            (MI.RightWallDetector.isColliding() && MI.RightWallDetector.CollidingWith() != null && MI.RightWallDetector.CollidingWith().tag == "Ladder"))
        {
            return new Climb();
        }
        */
        if (CS.Player.Move.ReadValue<float>() == 0 && Mathf.Abs(xVel) < MS.RunToIdleSpeed)
        {
            return new Idle();
        }
        return this;
    }

    public override AnimationType GetAnimationState()
    {
        return AnimationType.RUN;
    }
}
