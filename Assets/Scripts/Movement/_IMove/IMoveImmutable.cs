using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents information about a move in a particular frame. In obtaining
/// this information, nothing about the move itself can be modified.
/// </summary>
public interface IMoveImmutable
{
    /// <summary>
    /// Gives the movement speed the player should have in the horizontal
    /// direction this frame (in units per second). Negative represents left movement.
    /// </summary>
    public float XSpeed();

    /// <summary>
    /// Gives the movement speed the player should have in the vertical
    /// direction this frame (in units per second). Negative represents down movement.
    /// </summary>
    public float YSpeed();

    /// <summary>
    /// Is the player flipped to the left currently (true) or is it facing right
    /// (false)?
    /// NOTE: This function needs to be called at least once every frame in
    /// order to work properly. Otherwise, the stored "flip status"
    /// may not update when expected.
    /// </summary>
    public bool Flipped();

    /// <summary>
    /// If connected to the wire during this move, is it okay to disconnect
    /// by pressing jump?
    /// </summary>
    public bool DisconnectByJumpOkay();

    /// <summary>
    /// What animation type should the player currently be in?
    /// </summary>
    public AnimationType GetAnimationState();
}
