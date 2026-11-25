using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastPlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float buffer = 0.05f;
    private BoxCollider2D collider;
    //private Bounds bounds;


    [Header("Physics")]
    [SerializeField] private float Gravity;
    public Vector2 MoveDisplacement;

    private ContactFilter2D filter;
    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>();

    private bool inAir;
    private bool grounded;
    private bool touchingWall;

    public LayerMask GroundLayer => groundLayer;  // so the rope can check line of sight
    public bool OnRope { get; set; }
    public float GetGravity() => Mathf.Abs(Gravity);
    public bool isInAir() => inAir;
    public bool Grounded() => grounded;
    public bool TouchingWall() => touchingWall;

    [ContextMenu("Awake")]
    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();

        filter = new ContactFilter2D();
        filter.SetLayerMask(groundLayer);
        filter.useTriggers = false;
    }


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




}
