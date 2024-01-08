using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the move where the player is being knocked back by some object
/// that causes damage.
/// </summary>
public class Knockback : AMove
{
    private float vertVel;
    private float horizVel;

    /// <summary>
    /// Initializes a knockback move, starting the player at a specific vertical
    /// and horizontal speed.
    /// </summary>
    public Knockback()
    {
        vertVel = MS.KnockbackVertVel;
        // Direction of knockback depends on input right now
        horizVel = (CS.Player.Move.ReadValue<float>() >= 0) ? -MS.AbsKnockbackHorizVel : MS.AbsKnockbackHorizVel;
    }

    public override void AdvanceTime()
    {
        if (MI.GroundDetector.isColliding())
        {
            if (vertVel < 0) { vertVel = 0; }
            horizVel -= Mathf.Sign(horizVel) * MS.KnockbackGravityHorizOnRecovery * Time.deltaTime;
        }
        else
        {
            vertVel -= MS.KnockbackGravity * Time.deltaTime;
        }
    }

    public override float XSpeed() => horizVel;

    public override float YSpeed() => vertVel;

    public override IMove GetNextMove()
    {
        if (Mathf.Abs(horizVel) < MS.RunToIdleSpeed && MI.GroundDetector.isColliding())
        {
            return new Idle();
        }
        return this;
    }

    public override AnimationType GetAnimationState() => AnimationType.KNOCKBACK;
}
