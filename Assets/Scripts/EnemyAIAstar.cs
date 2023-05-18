using Pathfinding;
using UnityEngine;

public class EnemyAIAstar : MonoBehaviour
{

    // ADAPTED FROM THIS YT VID: https://www.youtube.com/watch?v=sWqRfygpl4I
    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float updatePathPerSecondSpeed = 0.5f;

    [Header("Physics")]
    public float walkSpeed = 2f;
    public float nextWaypointDistance = 3f;
    public float minimimumHeightForJump = 0.8f;
    public float jumpForce = 0.3f;
    public float jumpColliderOffset = 0.1f;

    // add behavior modifiers as necessary for enemy ai navigation
    [Header("Custom Enemy Behavior Modifiers")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool lookAtEnabled = true;

    // private variables
    private Path path;
    private int currentWaypoint = 0;
    bool isGrounded = false;
    bool reachedEndOfPath = false;
    Seeker seeker;
    Rigidbody2D rb;

    private void Start()
    {
        // get seeker script and rigidbody from enemy object
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0, updatePathPerSecondSpeed);
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            MoveTowardsPlayer();
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

    private void MoveTowardsPlayer() {

        if (IsPathDone())
        {
            return;
        }


        // checking if enemy is grounded 
        isGrounded = Physics2D.Raycast(transform.position, Vector3.up, GetComponent<Collider2D>().bounds.extents.y + jumpColliderOffset);

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
        rb.AddForce(moveForce);

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

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

}
