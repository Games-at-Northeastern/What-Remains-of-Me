using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a moving platform to be activated by an Outlet.
/// </summary>
public class PlatformMoving : AControllable
{
    public List<Animator> platforms;

    private bool hasMoved = false;

    /// <summary>
    /// Updates the platforms by moving them when the battery percentage of the platform is full or empty.
    /// </summary>
    private void Update()
    {
        if ((this.GetPercentFull() == 1 || this.GetPercentFull() == 0) && !hasMoved)
        {
            foreach (Animator platform in platforms)
            {
                platform.SetTrigger("Move");
            }
            hasMoved = true;
        }
        else if (!(this.GetPercentFull() == 1 || this.GetPercentFull() == 0))
        {
            hasMoved = false;
        }
    }
}
