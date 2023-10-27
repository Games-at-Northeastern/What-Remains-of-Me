
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a move in which the player is on the ground and intentionally
/// staying still.
/// </summary>
public class Idle : AMove
{
    private bool dashInput;
    private bool damageInput;

    private bool jumpPending; // is true if space still being held after last jump
    private float currDistFromOutlet; // Current position from the player to the outlet, if wire connected

    /// <summary>
    /// Initializes the idle move, and does any actions that need to be completed
    /// as an idle move begins.
    /// </summary>
    public Idle()
    {
        AMove.dashIsReset = true;
        CS.Player.Dash.performed += _ => dashInput = true;
        jumpPending = (CS.Player.Jump.ReadValue<float>() > 0);
        PH.OnDamageTaken.AddListener(() => damageInput = true);
    }

    public override void AdvanceTime()
    {
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

    public override float XSpeed() => 0;

    public override float YSpeed() => 0;

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
        if (!jumpPending && CS.Player.Jump.ReadValue<float>() > 0)
        {
            return new Jump();
        }
        if (CS.Player.Move.ReadValue<float>() != 0)
        {
            return new Run();
        }
        return this;
    }

    public override AnimationType GetAnimationState() => AnimationType.IDLE;
}
