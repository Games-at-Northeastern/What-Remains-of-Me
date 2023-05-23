using Pathfinding;
using UnityEngine;

/// <summary>
/// Class that deals with enemy AI path finding towards a target (player).
/// Uses the A* Pathfinding library package included in the assets folder.
/// Code adapted from this YT video: https://www.youtube.com/watch?v=sWqRfygpl4I&ab_channel=Etredal
/// </summary>
public class EnemyAIAstar : MonoBehaviour
{
    [Header("Pathfinding")]
    // object enemey is following (should be player)
    public Transform target;
    // enemy will detect target within this distance
    public float activateDistance = 10f;
    // how often A* pathfinding algorithm is updated (in seconds)
    public float updatePathPerSecondSpeed = 0.5f;

    [Header("Physics")]
    // how fast enemy moves
    public float walkSpeed = 50f;
    // how far away the enemy needs to be to start moving to next waypoint
    public float nextWaypointDistance = 3f;
    // how vertical next node needs to be for enemy to jump
    public float minimimumHeightForJump = 5f;
    // how powerful jump is
    public float jumpForce = 40f;
    // for the collider with nodes offset 
    public float jumpColliderOffset = 0.1f;

    // add behavior modifiers as necessary for enemy ai navigation
    [Header("Custom Enemy Behavior Modifiers")]
    // toggle enable enemy to follow (should be true for script to work)
    public bool followEnabled = true;
    // toggle enable enemy to be able to jump
    public bool jumpEnabled = true;
    // toggle enable enemy to look at target
    public bool lookAtEnabled = true;

    // private variables
    // use variable provided by A* Pathfinding library package
    private Path path;
    // variable to mark current waypoint
    private int currentWaypoint = 0;

    // what layer all ground objects enemy detects should be on
    [SerializeField] private LayerMask groundLayer;
    bool reachedEndOfPath = false;
    //private Animator anim;
    Seeker seeker;
    Rigidbody2D rb;
    Collider2D col;

    /// <summary>
    /// Initializes seeker script among other private variables.
    /// Starts process of calling UpdatePath towards target.
    /// </summary>
    /// <returns></returns>
    private void Start()
    {
        // get seeker script, rigidbody, and collider from enemy object
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // repeatedly calls UpdatePath func based on update variable
        InvokeRepeating("UpdatePath", 0, updatePathPerSecondSpeed);
    }

    /// <summary>
    /// Sets the enemy to move toward player if player within activate distance and follow is enabled.
    /// </summary>
    /// <returns></returns>
    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            MoveTowardsPlayer();
        }
    }

    /// <summary>
    /// Updates the pathfinding algo to find next waypoint towards target (player) if target is in distance.
    /// If target is reached, OnPathComplete is called.
    /// </summary>
    /// <returns></returns>
    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    /// <summary>
    /// Sets path to given path and resets current waypoint. This means enemy has reached target.
    /// </summary>
    /// <returns></returns>
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    /// <summary>
    /// Path is done if it has reached the end of path, or it's nonexistant.
    /// </summary>
    /// <returns></returns>
    private bool IsPathDone() {
        if (path == null)
        {
            return true;
        }

        reachedEndOfPath = currentWaypoint >= path.vectorPath.Count;
        if (reachedEndOfPath)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Handles updating enemy movement toward target (player).
    /// </summary>
    /// <returns></returns>
    private void MoveTowardsPlayer() {

        if (IsPathDone())
        {
            return;
        }


        // checking if enemy is grounded
        RaycastHit2D isGrounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, 0.1f, groundLayer);

        // direction calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 moveForce = direction * walkSpeed * Time.deltaTime;

        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > minimimumHeightForJump)
            {
                rb.AddForce(Vector2.up * walkSpeed * jumpForce);
            }
        }

        // Movement
        if (!isGrounded)
        {
            moveForce.y = 0;
        }
        rb.AddForce(moveForce, ForceMode2D.Force);

        if (rb.velocity.x > walkSpeed)
        {
            rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
        } else if (rb.velocity.x < -walkSpeed)
        {
            rb.velocity = new Vector2(-walkSpeed, rb.velocity.y);
        }

        // move to next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // flip sprite
        if (lookAtEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    /// <summary>
    /// Checks whether target (player) is within activateDistance of enemy. Will start to follow if true.
    /// </summary>
    /// <returns>boolean</returns>
    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

}
