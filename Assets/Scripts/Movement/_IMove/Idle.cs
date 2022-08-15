
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a move in which the player is on the ground and intentionally
/// staying still.
/// </summary>
public class Idle : AMove
{
    bool dashInput;
    bool damageInput;

    float currDistFromOutlet; // Current position from the player to the outlet, if wire connected

    /// <summary>
    /// Initializes the idle move, and does any actions that need to be completed
    /// as an idle move begins.
    /// </summary>
    public Idle()
    {
        AMove.dashIsReset = true;
        CS.Player.Dash.performed += _ => dashInput = true;
        PH.OnDamageTaken.AddListener(() => damageInput = true);
    }

    public override void AdvanceTime()
    {
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
        return 0;
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
        if (!MI.GroundDetector.isColliding())
        {
            return new Fall();
        }
        if (dashInput && AMove.dashIsReset && UpgradeHandler.DashAllowed)
        {
            return new Dash();
        }
        if (CS.Player.Jump.ReadValue<float>() > 0)
        {
            return new Jump();
        }
        if (CS.Player.Move.ReadValue<float>() != 0)
        {
            return new Run();
        }
        return this;
    }

    public override AnimationType GetAnimationState()
    {
        return AnimationType.IDLE;
    }
}
