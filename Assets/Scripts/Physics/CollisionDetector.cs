using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for giving whether or not this gameObject is colliding
/// with a certain set of layers.
///
/// At the moment, only works with box colliders, which should be good
/// enough for most purposes.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CollisionDetector : MonoBehaviour
{
    BoxCollider2D col;
    [SerializeField] LayerMask collidesWith;
    [SerializeField] bool collidesWithTriggers;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Is the collider for this gameObject touching any of the layers
    /// in the provided mask?
    /// </summary>
    public bool isColliding()
    {
        return col.IsTouchingLayers(collidesWith);
    }

    /// <summary>
    /// What gameObject is this gameObject colliding with? If multiple, chooses
    /// any one.
    /// </summary>
    public GameObject CollidingWith()
    {
        Collider2D collidingCol = Physics2D.OverlapArea(col.bounds.min, col.bounds.max, collidesWith);
        bool invalidCol = (collidingCol == null) || (!collidesWithTriggers && collidingCol.isTrigger);
        return invalidCol ? null : collidingCol.gameObject;
    }
}
