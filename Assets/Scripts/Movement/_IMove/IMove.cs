using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives access to all the methods in a move, including ones capable of
/// changing its contents.
/// </summary>
public interface IMove : IMoveImmutable
{
    /// <summary>
    /// Advances time forward by one frame. The expectation is that this
    /// function will be called exactly one time per Update() call.
    /// </summary>
    public void AdvanceTime();

    /// <summary>
    /// What should the move next frame be? If the move should not change,
    /// the current move should be returned. Should never return null.
    /// </summary>
    public IMove GetNextMove();
}

