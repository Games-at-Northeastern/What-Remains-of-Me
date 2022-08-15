using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a movement style that a plug maintains as it flies through the air.
/// </summary>
public interface IPlugMovementModel
{
    /// <summary>
    /// Moves time in the model forward by one frame.
    /// </summary>
    public void AdvanceTime();

    /// <summary>
    /// How fast should the plug move horizontally and vertically this frame?
    /// </summary>
    public Vector2 Velocity();

    /// <summary>
    /// React to hitting a wall or some other surface.
    /// </summary>
    public void HandleCollision();

    /// <summary>
    /// Should the plug cease to exist at this point in the move? 
    /// </summary>
    public bool Terminate();
}
