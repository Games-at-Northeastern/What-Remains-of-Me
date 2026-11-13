using System;
using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastPlayerController : MonoBehaviour
{
    [Header("settings")]
    [SerializeField] private int numRays = 2; //number of rays for horizontal and vertical displacement. total rays = numRays * 2
    private Vector2 rayDistances; //float distance between each ray. y dist is <->, x dist is for ^ v
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float buffer = 0.05f;
    private BoxCollider2D collider;
    private Bounds bounds;


    [Header("physics")]
    [SerializeField] private float Gravity;
    public Vector2 MoveDisplacement;

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
        updateBounds();
        rayDistances = new Vector2(bounds.size.x / (numRays - 1), bounds.size.y / (numRays - 1));
    }

    void updateBounds()
    {
        bounds = collider.bounds; //creates a COPY of colldiers bounds
        bounds.Expand(-buffer);
    }

    private void FixedUpdate() //physics
    {
        updateBounds();
        MoveDisplacement.y -= Gravity;

        touchingWall = false;
        for (int i = 0; i < numRays; i++) //horizontal
        {
            var hit = Physics2D.Raycast(new Vector2(MoveDisplacement.x > 0 ? bounds.max.x : bounds.min.x, bounds.min.y + i * rayDistances.y)
            , Vector2.right * Mathf.Sign(MoveDisplacement.x)
            , Mathf.Abs(MoveDisplacement.x) + buffer
            , groundLayer);
            if (hit)
            {
                MoveDisplacement.x = (hit.distance - buffer) * Mathf.Sign(MoveDisplacement.x);
                touchingWall = true;
            }
        }

        grounded = false;
        for (int i = 0; i < numRays; i++) //vertical
        {
            var hit = Physics2D.Raycast(new Vector2(bounds.min.x + i * rayDistances.x, MoveDisplacement.y > 0 ? bounds.max.y : bounds.min.y)
            , Vector2.up * Mathf.Sign(MoveDisplacement.y)
            , Mathf.Abs(MoveDisplacement.y) + buffer
            , groundLayer);
            if (hit)
            {
                MoveDisplacement.y = (hit.distance - buffer) * Mathf.Sign(MoveDisplacement.y);
                grounded = true;
            }
        }
        transform.Translate(MoveDisplacement, Space.World);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < numRays; i++) //horizontal
        {
            Gizmos.DrawRay(new Vector2(MoveDisplacement.x > 0 ? bounds.max.x : bounds.min.x, bounds.min.y + i * rayDistances.y)
            , (Mathf.Abs(MoveDisplacement.x) + buffer) * Mathf.Sign(MoveDisplacement.x) * Vector2.right);
        }
        for (int i = 0; i < numRays; i++) //vertical
        {
            Gizmos.DrawRay(new Vector2(bounds.min.x + i * rayDistances.x, MoveDisplacement.y > 0 ? bounds.max.y : bounds.min.y)
            , (Mathf.Abs(MoveDisplacement.y) + buffer) * Mathf.Sign(MoveDisplacement.y) * Vector2.up);
        }
    }



}
