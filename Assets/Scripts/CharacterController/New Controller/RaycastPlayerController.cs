using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastPlayerController : MonoBehaviour
{
    [Header("settings")]
    //[SerializeField] private int numRays = 2; //number of rays for horizontal and vertical displacement. total rays = numRays * 2
    //private Vector2 rayDistances; //float distance between each ray. y dist is <->, x dist is for ^ v
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float buffer = 0.05f;
    private BoxCollider2D collider;
    //private Bounds bounds;


    [Header("physics")]
    [SerializeField] private float Gravity;
    public Vector2 MoveDisplacement;

    private ContactFilter2D filter;
    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>();
    //[SerializeField] Module[] modules;

    private bool inAir;
    private bool grounded;
    private bool touchingWall;

    public LayerMask GroundLayer => groundLayer;  // so the rope can check line of sight
    public bool OnRope { get; set; }
    public float GetGravity() => Mathf.Abs(Gravity);
    public bool isInAir() => inAir;
    public bool Grounded() => grounded;
    public bool TouchingWall() => touchingWall;
    // so WalkModule knows to stay out of the way

    [ContextMenu("Awake")]
    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        // updateBounds();
        // rayDistances = new Vector2(bounds.size.x / (numRays - 1), bounds.size.y / (numRays - 1));

        filter = new ContactFilter2D();
        filter.SetLayerMask(groundLayer);
        filter.useTriggers = false;
    }

    // void updateBounds()
    // {
    //     bounds = collider.bounds; //creates a COPY of colldiers bounds
    //     bounds.Expand(-buffer);
    // }

    float Sweep(Vector2 center, Vector2 size, Vector2 dir, float distance, out RaycastHit2D hit)
    {
        int count = Physics2D.BoxCast(center, size, 0f, dir, filter, hits, distance + buffer);

        hit = default;
        float closest = float.MaxValue;
        for (int i = 0; i < count; i++) //hits should be per collider hit
        {
            if (hits[i].distance < closest)
            {
                closest = hits[i].distance;
                hit = hits[i];
            }
        }

        if (count == 0)
            return distance;
        return Mathf.Clamp(closest - buffer, 0f, distance);
    }

    private void FixedUpdate()
    {
        MoveDisplacement.y -= Gravity;

        Vector2 center = transform.TransformPoint(collider.offset); //temp holder for position
        Vector2 size = Vector2.Scale(collider.size, transform.lossyScale);
        size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
        size -= Vector2.one * (2f * buffer);

        // Horizontal
        touchingWall = false;
        if (MoveDisplacement.x != 0f)
        {
            Vector2 dir = new Vector2(Mathf.Sign(MoveDisplacement.x), 0f);
            float wanted = Mathf.Abs(MoveDisplacement.x);
            float allowed = Sweep(center, size, dir, wanted, out RaycastHit2D hit);

            if (allowed < wanted)
            {
                MoveDisplacement.x = allowed * dir.x;
                touchingWall = true;
            }
            center.x += MoveDisplacement.x; //move the posotion so the collider is always in the most accurate spot
        }

        // Vertical, cast from where the horizontal move ended
        grounded = false;
        if (MoveDisplacement.y != 0f)
        {
            Vector2 dir = new Vector2(0f, Mathf.Sign(MoveDisplacement.y));
            float wanted = Mathf.Abs(MoveDisplacement.y);
            float allowed = Sweep(center, size, dir, wanted, out RaycastHit2D hit);

            if (allowed < wanted)
            {
                if (dir.y < 0f && hit.normal.y > 0.5f)
                    grounded = true;

                MoveDisplacement.y = allowed * dir.y;
            }
        }

        transform.Translate(MoveDisplacement, Space.World);
    }

    // private void FixedUpdate() //physics
    // {
    //     updateBounds();
    //     MoveDisplacement.y -= Gravity;

    //     touchingWall = false;
    //     for (int i = 0; i < numRays; i++) //horizontal
    //     {
    //         var hit = Physics2D.Raycast(new Vector2(MoveDisplacement.x > 0 ? bounds.max.x : bounds.min.x, bounds.min.y + i * rayDistances.y)
    //         , Vector2.right * Mathf.Sign(MoveDisplacement.x)
    //         , Mathf.Abs(MoveDisplacement.x) + buffer
    //         , groundLayer);
    //         if (hit)
    //         {
    //             MoveDisplacement.x = (hit.distance - buffer) * Mathf.Sign(MoveDisplacement.x);
    //             touchingWall = true;
    //         }
    //     }

    //     grounded = false;
    //     for (int i = 0; i < numRays; i++) //vertical
    //     {
    //         var hit = Physics2D.Raycast(new Vector2(bounds.min.x + i * rayDistances.x, MoveDisplacement.y > 0 ? bounds.max.y : bounds.min.y)
    //         , Vector2.up * Mathf.Sign(MoveDisplacement.y)
    //         , Mathf.Abs(MoveDisplacement.y) + buffer
    //         , groundLayer);
    //         if (hit)
    //         {
    //             MoveDisplacement.y = (hit.distance - buffer) * Mathf.Sign(MoveDisplacement.y);
    //             grounded = true;
    //         }
    //     }
    //     transform.Translate(MoveDisplacement, Space.World);
    // }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.yellow;
    //     for (int i = 0; i < numRays; i++) //horizontal
    //     {
    //         Gizmos.DrawRay(new Vector2(MoveDisplacement.x > 0 ? bounds.max.x : bounds.min.x, bounds.min.y + i * rayDistances.y)
    //         , (Mathf.Abs(MoveDisplacement.x) + buffer) * Mathf.Sign(MoveDisplacement.x) * Vector2.right);
    //     }
    //     for (int i = 0; i < numRays; i++) //vertical
    //     {
    //         Gizmos.DrawRay(new Vector2(bounds.min.x + i * rayDistances.x, MoveDisplacement.y > 0 ? bounds.max.y : bounds.min.y)
    //         , (Mathf.Abs(MoveDisplacement.y) + buffer) * Mathf.Sign(MoveDisplacement.y) * Vector2.up);
    //     }
    // }



}
