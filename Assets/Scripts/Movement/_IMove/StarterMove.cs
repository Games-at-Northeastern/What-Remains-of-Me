using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move class that only exists to initialize AMove and to decide what the first
/// (real) move should be. This should always be the very first move that
/// occurs, and should only ever take place for one frame.
/// </summary>
public class StarterMove : AMove
{
    /// <summary>
    /// Given a MovementInfo and MovementSettings instance, gives these
    /// to the base class so that they can be accessed by any move classes
    /// in the future.
    /// </summary>
    public StarterMove(MovementInfo mi, MovementSettings ms, ControlSchemes cs, WireThrower wt, PlayerHealth ph)
    {
        base.Initialize(mi, ms, cs, wt, ph);
    }

    public override void AdvanceTime() { /* Nothing */ }

    public override float XSpeed() { return 0; }

    public override float YSpeed() { return 0; }

    public override IMove GetNextMove()
    {
        return new Fall();
    }

    public override AnimationType GetAnimationState()
    {
        return AnimationType.NONE;
    }
}
