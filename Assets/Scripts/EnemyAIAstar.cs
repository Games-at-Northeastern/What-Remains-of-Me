using Pathfinding;
using UnityEngine;

public class EnemyAIAstar : MonoBehaviour
{

    public Transform target;
    public float walkSpeed = 2f;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0, .5f);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
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
        MoveTowardsPlayer();
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

        //pathfinding output direction.
        Vector2 direction = ((Vector2)(path.vectorPath[currentWaypoint] - transform.position)).normalized;

        //Determining VELOCITY:
        bool isGrounded = Physics2D.OverlapCircle(transform.position - (Vector3.up * .3f), .1f);
        bool shouldJump = Vector2.Dot(direction, Vector2.up) > .5f && isGrounded;
        float yVel = shouldJump ? 5f : rb.velocity.y;

        Vector2 velocity = direction * walkSpeed;
        //velocity.y = yVel;
        //print("velocity.y: " + velocity.y);

        rb.velocity = velocity;

        //incrementing next waypoint.
        float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

}
