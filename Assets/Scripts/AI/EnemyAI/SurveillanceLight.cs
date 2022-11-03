using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the surveilance light and its collisions.
/// </summary>
public class SurveillanceLight : MonoBehaviour
{
    private bool isColliding;


    /// <summary>
    /// Checks if the object is in collision currently
    /// </summary>
    /// <returns></returns>
    public bool GetCollisionStatus()
    {
        return isColliding;
    }

    /// <summary>
    /// Update the object to be in collision if the given collision
    /// is with the player
    /// </summary>
    /// <param name="collision">Collision opject to be passed in</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isColliding = true;
        }
    }

    /// <summary>
    /// On exit if the collision is with the player, end the collision state
    /// </summary>
    /// <param name="collision">Collision object to be passed in</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isColliding = false;
        }
    }
}
