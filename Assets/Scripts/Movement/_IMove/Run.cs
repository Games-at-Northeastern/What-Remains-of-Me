using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a move in which the player is moving across the ground.
/// </summary>w
///

public class Run : AMove
{
    private float xVel;
    private float xAccel; // Don't mess with this outside of calling Mathf.SmoothDamp

    private bool dashInput;
    private bool jumpPending; // is true if space still being held after last jump
    private bool damageInput;

    private float currDistFromOutlet; // Current position from the player to the outlet, if wire connected
    public bool isOnPlatform;
    public Rigidbody2D platformRB;

    /// <summary>
    /// Constructs a Run move, starting it off with the given horizontal speed.
    /// </summary>
    public Run(float initXVel)
    {
        AMove.dashIsReset = true;
        CS.Player.Dash.performed += _ => dashInput = true;
        jumpPending = (CS.Player.Jump.ReadValue<float>() > 0);
        PH.OnDamageTaken.AddListener(() => damageInput = true);
        xVel = initXVel;
    }

    public Run() : this(0)
    {
        // Default Constructor
    }

    public override void AdvanceTime()
    {

            if (xVel > MS.RunMaxSpeed)
            {
                xVel = Mathf.SmoothDamp(xVel, MS.RunMaxSpeed * CS.Player.Move.ReadValue<float>(),
                    ref xAccel, (float)0.3);
            }
            else
            {
                xVel = Mathf.SmoothDamp(xVel, MS.RunMaxSpeed * CS.Player.Move.ReadValue<float>(),
                    ref xAccel, MS.RunSmoothTime);
            }

            if (WT.ConnectedOutlet != null)
            {
                Vector2 origPos = MI.transform.position;
                Vector2 connectedOutletPos = WT.ConnectedOutlet.transform.position;
                float newDistFromOutlet = Vector2.Distance(origPos, connectedOutletPos);
                // The code doesn't let the wire 'stretch' to the default max length, should check with
                // design team on the ideal wire movement while grounded
                /*if (newDistFromOutlet < currDistFromOutlet)
                {
                    WT.SetMaxWireLength(newDistFromOutlet);
                }*/
            currDistFromOutlet = newDistFromOutlet;
            }

            // ready the player for another jump once space has been released
            if (jumpPending && CS.Player.Jump.ReadValue<float>() == 0)
            {
                jumpPending = false;
            }

    }

    public override float XSpeed() => xVel;

    public override float YSpeed() => 0;

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
        if (!jumpPending && CS.Player.Jump.ReadValue<float>() > 0)
        {
            return new Jump(xVel);
        }
        if (!MI.GroundDetector.isColliding())
        {
            return new Fall(xVel, 0);
        }
        if (CS.Player.Move.ReadValue<float>() == 0 && Mathf.Abs(xVel) < MS.RunToIdleSpeed)
        {
            return new Idle();
        }
        return this;
    }

    public override AnimationType GetAnimationState() => AnimationType.RUN;
}
