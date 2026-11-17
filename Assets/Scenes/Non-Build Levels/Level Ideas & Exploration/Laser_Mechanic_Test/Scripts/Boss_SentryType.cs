using System;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
///     Sentry module: decides how the laser moves and aims.
///     There are 4 sentry types:
///     1. Static: does not move.
///     2. Patrol: moves between two points on a rail, and locks on to the player when it sees them.
///     3. Tracking: follows the player.
///     4. Swivel: stays in place (e.g. on the ceiling) and turns back and forth to guard, then locks on to the player.
///     Use together with Boss_LaserType (how it shoots) and Boss_AmmunitionType (what the shots do).
///     If this component is turned off, the laser acts like a Static sentry.
///     The laser shoots along its -up, the same as the death laser turrets.
/// </summary>
public class Boss_SentryType : MonoBehaviour
{
    private const int PlayerHierarchyLayerIndex = 3;

    [SerializeField]
    [Tooltip("Static: does not move.\nPatrol: moves between two points, and locks on to the player.\nTracking: follows the player.\nSwivel: turns back and forth to guard, and locks on to the player.")]
    private SentryType sentryType;

    [Header("Targeting")]
    [SerializeField]
    [Tooltip("How close the player has to be for the sentry to notice them (Patrol and Tracking only).")]
    private float detectRange = 14f;
    [SerializeField]
    [Tooltip("If true, walls and crates hide the player from the sentry.")]
    private bool requireLineOfSight = true;
    [SerializeField]
    [Tooltip("What blocks the sentry's sight (make sure default is always included!)")]
    private LayerMask solidLayers = 1;
    [SerializeField]
    [Tooltip("If true, the sentry turns to aim at the player. If false, it keeps facing the same way.")]
    private bool aimAtPlayer = true;
    [SerializeField]
    [Tooltip("How fast the sentry turns, in degrees per second.")]
    private float turnSpeed = 120f;
    [SerializeField]
    [Tooltip("The sentry counts as aimed when it is within this many degrees of the player.")]
    private float aimTolerance = 8f;
    [SerializeField]
    [Tooltip("How long the player has to stay hidden before the sentry stops locking on.")]
    private float loseTargetDelay = 1f;

    [Header("Patrol")]
    [SerializeField]
    [Tooltip("First patrol point, relative to the starting position.")]
    private Vector2 patrolOffsetA = new Vector2(0f, -3f);
    [SerializeField]
    [Tooltip("Second patrol point, relative to the starting position.")]
    private Vector2 patrolOffsetB = new Vector2(0f, 3f);
    [SerializeField]
    private float patrolSpeed = 2f;
    [SerializeField]
    [Tooltip("If true, the sentry stops patrolling (or swiveling) and aims at the player when it sees them.")]
    private bool lockOnWhenSeen = true;

    [Header("Swivel")]
    [SerializeField]
    [Tooltip("How far the sentry turns to each side of its starting direction, in degrees.")]
    private float swivelAngle = 50f;
    [SerializeField]
    [Tooltip("How fast the sentry turns while guarding, in degrees per second.")]
    private float swivelSpeed = 40f;

    [Header("Tracking")]
    [SerializeField]
    [Tooltip("Moves left and right to follow the player.")]
    private bool followPlayerX;
    [SerializeField]
    [Tooltip("Moves up and down to follow the player.")]
    private bool followPlayerY;
    [SerializeField]
    private float followSpeed = 2.5f;
    [SerializeField]
    [Tooltip("The lowest position the sentry can move to (world space).")]
    private Vector2 followMin = new Vector2(-10000f, -10000f);
    [SerializeField]
    [Tooltip("The highest position the sentry can move to (world space).")]
    private Vector2 followMax = new Vector2(10000f, 10000f);

    private Transform player;
    private bool hasTarget; // Is the sentry locked on to the player
    private float lostTimer; // How long the player has been hidden

    private Vector2 patrolA;
    private Vector2 patrolB;
    private bool movingToB = true;
    private float restAngle; // The direction the sentry started in
    private float swivelOffset; // Current guard angle, relative to restAngle
    private bool swivelingRight = true;

    private ContactFilter2D solidFilter;
    private readonly RaycastHit2D[] hits = new RaycastHit2D[8];
    private readonly string tag = "Player";

    /// <summary>
    ///     Is the sentry allowed to shoot right now? Static sentries are always active.
    /// </summary>
    public bool HasTarget => !enabled || sentryType == SentryType.Static || hasTarget;

    /// <summary>
    ///     Set by Boss_LaserType to stop the sentry from turning (e.g. right before a sniper shot).
    /// </summary>
    public bool AimFrozen { get; set; }

    private void Awake()
    {
        // The turret prefab has a rotation constraint that fights with our rotation
        RotationConstraint constraint = GetComponent<RotationConstraint>();
        if (constraint != null)
        {
            constraint.constraintActive = false;
        }

        // Only solid objects, no triggers (checkpoints and camera bounds are triggers)
        solidFilter = new ContactFilter2D();
        solidFilter.SetLayerMask(solidLayers);
        solidFilter.useTriggers = false;
    }

    private void Start()
    {
        Vector2 startPosition = transform.position;
        patrolA = startPosition + patrolOffsetA;
        patrolB = startPosition + patrolOffsetB;
        restAngle = transform.eulerAngles.z;
    }

    /// <summary>
    ///     Updates the target, then moves and turns the sentry.
    /// </summary>
    private void Update()
    {
        if (sentryType == SentryType.Static)
        {
            return;
        }

        UpdateTarget();

        if (sentryType == SentryType.Patrol)
        {
            if (hasTarget && lockOnWhenSeen)
            {
                if (aimAtPlayer && !AimFrozen)
                {
                    RotateTowardsPlayer(); // Stop and lock on
                }
            }
            else
            {
                Patrol();
                if (!AimFrozen)
                {
                    RotateTowards(restAngle);
                }
            }
        }
        else if (sentryType == SentryType.Swivel)
        {
            if (hasTarget && lockOnWhenSeen)
            {
                if (aimAtPlayer && !AimFrozen)
                {
                    RotateTowardsPlayer(); // Lock on
                }
            }
            else if (!AimFrozen)
            {
                Swivel();
            }
        }
        else if (sentryType == SentryType.Tracking && hasTarget)
        {
            FollowPlayer();
            if (aimAtPlayer && !AimFrozen)
            {
                RotateTowardsPlayer();
            }
        }
    }

    /// <summary>
    ///     Checks if the sentry is aimed close enough to the player to shoot.
    /// </summary>
    /// <returns> Returns true if aimed, or if this sentry does not aim at all </returns>
    public bool IsAimedAtPlayer()
    {
        if (!enabled || sentryType == SentryType.Static || !aimAtPlayer)
        {
            return true;
        }

        if ((sentryType == SentryType.Patrol || sentryType == SentryType.Swivel) && !lockOnWhenSeen)
        {
            return true;
        }

        Transform target = GetPlayer();
        if (target == null)
        {
            return false;
        }

        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        return Vector2.Angle(-transform.up, direction) <= aimTolerance;
    }

    /// <summary>
    ///     Finds the player by tag (cached after the first time).
    /// </summary>
    public Transform GetPlayer()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag(tag);
            player = found != null ? found.transform : null;
        }
        return player;
    }

    /// <summary>
    ///     Locks on when the player is seen, and lets go after the player has been hidden for a while.
    /// </summary>
    private void UpdateTarget()
    {
        if (CanSeePlayer())
        {
            hasTarget = true;
            lostTimer = 0f;
            return;
        }

        if (!hasTarget)
        {
            return;
        }

        lostTimer += Time.deltaTime;
        if (lostTimer >= loseTargetDelay)
        {
            hasTarget = false;
        }
    }

    /// <summary>
    ///     Checks if the player is in range, and if nothing solid is in the way.
    /// </summary>
    /// <returns> Returns whether the sentry can see the player </returns>
    private bool CanSeePlayer()
    {
        Transform target = GetPlayer();
        if (target == null)
        {
            return false;
        }

        Vector2 from = transform.position;
        Vector2 to = target.position;
        if ((to - from).sqrMagnitude > detectRange * detectRange)
        {
            return false;
        }

        if (!requireLineOfSight)
        {
            return true;
        }

        int count = Physics2D.Linecast(from, to, solidFilter, hits);
        for (int i = 0; i < count; i++)
        {
            Collider2D other = hits[i].collider;
            if (other != null && !other.transform.IsChildOf(transform)
                && !UtilityFunctions.CompareTagOfHierarchy(other.transform, tag, out _, PlayerHierarchyLayerIndex))
            {
                return false; // Something is blocking the view
            }
        }

        return true;
    }

    /// <summary>
    ///     Moves back and forth between the two patrol points.
    /// </summary>
    private void Patrol()
    {
        Vector2 target = movingToB ? patrolB : patrolA;
        Vector2 next = Vector2.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);
        transform.position = new Vector3(next.x, next.y, transform.position.z);

        if ((next - target).sqrMagnitude < 0.0001f)
        {
            movingToB = !movingToB;
        }
    }

    /// <summary>
    ///     Turns back and forth around the starting direction to guard an area.
    ///     After losing the player, it turns back to its guard path first.
    /// </summary>
    private void Swivel()
    {
        float current = Mathf.DeltaAngle(restAngle, transform.eulerAngles.z);
        if (Mathf.Abs(current - swivelOffset) > 1f)
        {
            swivelOffset = Mathf.Clamp(current, -swivelAngle, swivelAngle); // Continue from where it is now
        }

        float target = swivelingRight ? -swivelAngle : swivelAngle;
        swivelOffset = Mathf.MoveTowards(swivelOffset, target, swivelSpeed * Time.deltaTime);
        if (Mathf.Approximately(swivelOffset, target))
        {
            swivelingRight = !swivelingRight;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, restAngle + swivelOffset);
    }

    /// <summary>
    ///     Moves towards the player on the chosen axes, staying between followMin and followMax.
    /// </summary>
    private void FollowPlayer()
    {
        Transform target = GetPlayer();
        if (target == null || (!followPlayerX && !followPlayerY))
        {
            return;
        }

        Vector3 position = transform.position;
        Vector2 goal = new Vector2(followPlayerX ? target.position.x : position.x, followPlayerY ? target.position.y : position.y);
        goal.x = Mathf.Clamp(goal.x, followMin.x, followMax.x);
        goal.y = Mathf.Clamp(goal.y, followMin.y, followMax.y);

        Vector2 next = Vector2.MoveTowards(position, goal, followSpeed * Time.deltaTime);
        transform.position = new Vector3(next.x, next.y, position.z);
    }

    private void RotateTowardsPlayer()
    {
        Transform target = GetPlayer();
        if (target == null)
        {
            return;
        }

        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        if (direction.sqrMagnitude > 0.0001f)
        {
            // -up of a rotation z is (sin z, -cos z), so this angle points -up at the player
            RotateTowards(Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg);
        }
    }

    private void RotateTowards(float targetAngle)
    {
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnDrawGizmosSelected()
    {
        if (sentryType == SentryType.Static)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        if (sentryType == SentryType.Patrol && !Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position + (Vector3)patrolOffsetA, transform.position + (Vector3)patrolOffsetB);
        }

        if (sentryType == SentryType.Swivel && !Application.isPlaying)
        {
            // Show both ends of the guard path
            Gizmos.color = Color.cyan;
            float baseAngle = transform.eulerAngles.z;
            foreach (float side in new[] { -swivelAngle, swivelAngle })
            {
                float radians = (baseAngle + side) * Mathf.Deg2Rad;
                Vector3 direction = new Vector3(Mathf.Sin(radians), -Mathf.Cos(radians), 0f);
                Gizmos.DrawLine(transform.position, transform.position + direction * 4f);
            }
        }
    }

    [Serializable]
    private enum SentryType
    {
        Static, Patrol, Tracking, Swivel
    }
}
