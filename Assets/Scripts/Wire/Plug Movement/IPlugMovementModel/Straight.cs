using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a form of wire movement in which the wire goes straight to and
/// from a certain point.
/// </summary>
public class Straight : IPlugMovementModel
{
    readonly Vector2 direction;
    readonly PlugMovementSettings pms;
    readonly Transform returnTransform;
    readonly Transform myTransform;
    private float timePassed = 0;
    private bool retracting = false;

    /// <summary>
    /// Initializes this move, setting it up with the direction that the plug
    /// should fly in at the beginning, as well as the transform it should return to,
    /// the plug movement settings to use, and the transform for this plug.
    /// </summary>
    public Straight(Vector2 dir, Transform myTransform, Transform returnTransform, PlugMovementSettings pms)
    {
        direction = dir.normalized;
        this.myTransform = myTransform;
        this.returnTransform = returnTransform;
        this.pms = pms;
    }


    /// <summary>
    /// Retracts the plug if it has been flying out past the limit amount of time
    /// </summary>
    public void AdvanceTime()
    {
        timePassed += Time.deltaTime;
        if (!retracting && timePassed > pms.StraightTimeTillRetraction)
        {
            retracting = true;
        }
    }

    /// <summary>
    /// Sets the velocity of the plug depending on whether it is flying out or retracting.
    /// </summary>
    public Vector2 Velocity()
    {
        // First Phase (moving away from origin transform)
        if (!retracting)
        {
            return direction * pms.StraightSpeed;
        }
        // Second Phase (moving towards origin transform)
        else
        {
            Vector2 distanceToReturnTransform = returnTransform.position - myTransform.position;
            return distanceToReturnTransform.normalized * pms.StraightSpeed;
        }

    }

    /// <summary>
    /// Retracts the plug.
    /// </summary>
    public void HandleCollision()
    {
        if (!retracting)
        {
            retracting = true;
        }
    }

    /// <summary> 
    /// Has the plug terminated retracting?
    /// </summary>
    public bool Terminate()
    {
        return (retracting && Vector2.Distance(myTransform.position, returnTransform.position) < 0.1f)
            || timePassed > 1;
    }
}
