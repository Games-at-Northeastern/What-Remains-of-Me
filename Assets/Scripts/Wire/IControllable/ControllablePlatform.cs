using System;
using System.Collections;
using System.Collections.Generic;
using Levels.Objects.Platform;
using UnityEngine;

/// <summary>
/// If this controllable gets enough energy, it tells the given platforms to activate.
/// </summary>
public class ControllablePlatform : AControllable
{
    [SerializeField] private Platform[] _platforms;

    private bool hasMoved = false;

    /// <summary>
    /// Updates the platforms by moving them when the battery percentage of the platform is full or empty.
    /// </summary>
    private void Update()
    {
        if (Math.Abs(GetPercentFull() - 1) < 0.01f && !hasMoved)
        {
            Array.ForEach(_platforms, platform => platform.Activate());
            hasMoved = true;
        }
        else if (GetPercentFull() < 0.99f && hasMoved)
        {
            Array.ForEach(_platforms, platform => platform.Deactivate());
            hasMoved = false;
        }
    }

    public void ApplyToPlatforms(Action<Platform> platformFunc) => Array.ForEach(_platforms, platform => platformFunc(platform));
}
