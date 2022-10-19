using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class for giving whether or not this gameObject is colliding
/// with a certain set of layers.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CollisionDetector : MonoBehaviour
{
    Collider2D col;
    [SerializeField] LayerMask collidesWith;
    [SerializeField] bool collidesWithTriggers;

    /// <summary>
    /// Initializes col to the attached object's collider.
    /// </summary>
    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Is the collider for this gameObject touching any of the layers in the provided mask?
    /// </summary>
    /// <returns>the state of this collider</returns>
    public bool isColliding()
    {
        return col.IsTouchingLayers(collidesWith);
    }

    /// <summary>
    /// What gameObject is this gameObject colliding with? If multiple, chooses any one.
    /// </summary>
    /// <returns>A game object that this collider is colliding with</returns>
    public GameObject CollidingWith()
    {
        Collider2D collidingCol = Physics2D.OverlapArea(col.bounds.min, col.bounds.max, 
                                                        collidesWith);
        bool invalidCol = (collidingCol == null) 
            || (!collidesWithTriggers && collidingCol.isTrigger);
        return invalidCol ? null : collidingCol.gameObject;
    }
}
