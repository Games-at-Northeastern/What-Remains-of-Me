using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class for giving whether or not this gameObject is colliding
/// with a certain set of layers.
/// </summary>
public class CollisionDetector : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [SerializeField] LayerMask collidesWith;
    [SerializeField] bool collidesWithTriggers;

    /// <summary>
    /// Is the collider for this gameObject touching any of the layers in the provided mask?
    /// </summary>
    /// <returns>the state of this collider</returns>
    public bool isColliding()
    {
        return CollidingWith() != null;
    }

    /// <summary>
    /// What gameObject is this gameObject colliding with? If multiple, chooses any one.
    /// </summary>
    /// <returns>A game object that this collider is colliding with on one of its detected layers</returns>
    public GameObject CollidingWith()
    {
        // Set-up our buffer list to stop overlapped colliders. In this case, we are just looking for one?
        // TODO: Check if we need to collect multiple... if we do, just change the size of the array
        Collider2D[] overlappingColliders = new Collider2D[1];

        // Set-up the filter we will use when detecting overlapping colliders for this object.
        // We only want to detect colliders in the LayerMask specified, and we want to make
        // sure we aren't detecting triggers unless specified
        ContactFilter2D colliderFilter = new ContactFilter2D();
        colliderFilter.SetLayerMask(collidesWith);
        colliderFilter.useTriggers = collidesWithTriggers;
        int numOverlap = col.OverlapCollider(colliderFilter, overlappingColliders);

        // Return null if no valid overlaps were found; otherwise, return the first returned collider's game object
        return numOverlap == 0 ? null : overlappingColliders[0].gameObject;
    }
}
