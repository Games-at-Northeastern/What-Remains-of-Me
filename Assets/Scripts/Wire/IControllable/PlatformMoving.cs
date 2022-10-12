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
