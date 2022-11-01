using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a move in which the player is sliding down a wall, preparing
/// for a wall jump.
/// </summary>
public class WallSlide : AMove
{
    private bool wallJumpPending = false;
    private bool damageInput;

    /// <summary>
    /// Initializes the WallSlide move and completes any actions that are
    /// necessary to do at the beginning of a Wall Slide.
    /// </summary>
    public WallSlide()
    {
        dashIsReset = true;
        CS.Player.Jump.performed += _ => wallJumpPending = true;
        PH.OnDamageTaken.AddListener(() => damageInput = true);
    }

    public override void AdvanceTime()
    {
        // Nothing
    }

    public override float XSpeed() => 0;

    public override float YSpeed() => MS.WallSlideSpeed;

    public override IMove GetNextMove()
    {
        if (damageInput)
        {
            return new Knockback();
        }
        if (!MI.RightWallDetector.isColliding() && !MI.LeftWallDetector.isColliding())
        {
            return new Fall(0, MS.WallSlideSpeed);
        }
        if (wallJumpPending)
        {
            return new WallJump(MI.LeftWallDetector.isColliding());
        }
        if (MI.GroundDetector.isColliding())
        {
            return new Idle();
        }
        return this;
    }

    public override AnimationType GetAnimationState() => AnimationType.WALL_SLIDE;
}
