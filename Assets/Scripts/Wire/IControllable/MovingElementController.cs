using System;
using System.Collections;
using System.Collections.Generic;
using Levels.Objects.Platform;
using UnityEngine;

/// <summary>
/// If this controllable gets enough energy, it tells the given moving elements to activate.
/// </summary>
public class MovingElementController : AControllable
{
    [SerializeField] private MovingElement[] _movingElements;

    private bool hasMoved = false;

    /// <summary>
    /// Updates the platforms by moving them when the battery percentage of the platform is full or empty.
    /// </summary>
    private void Update()
    {
        if (Math.Abs(GetPercentFull() - 1) < 0.01f && !hasMoved)
        {
            Array.ForEach(_movingElements, movingElement => movingElement.Activate());
            hasMoved = true;
        }
        else if (GetPercentFull() < 0.99f && hasMoved)
        {
            Array.ForEach(_movingElements, movingElement => movingElement.Deactivate());
            hasMoved = false;
        }
    }

    public void ApplyToAll(Action<MovingElement> meFunc) => Array.ForEach(_movingElements, movingElement => meFunc(movingElement));
}
