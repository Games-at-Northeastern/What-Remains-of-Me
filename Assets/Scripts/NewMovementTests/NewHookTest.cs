using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Test-only grappling hook for the NewMovementTests scene.
/// The rope only limits the velocity before each physics step, so collisions always win over the rope
/// and the player can't be pulled through level geometry. Releasing keeps the velocity the player had on the rope.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class NewHookTest : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private string hookPointTag = "Outlet";
    [SerializeField] private float maxHookDistance = 10f;
    [SerializeField, Range(0f, 180f), Tooltip("How far from the aim direction a hook point can be and still get picked")]
    private float aimAssistAngle = 40f;
    [SerializeField] private LayerMask obstacleMask = 1;

    [Header("Rope")]
    [SerializeField] private float minRopeLength = 1.5f;
    [SerializeField, Tooltip("How close to full length the rope has to be before swing input is applied")]
    private float tautTolerance = 0.1f;
    [SerializeField, Tooltip("Fastest speed the rope pulls the player back in after being stretched (e.g. by a collision)")]
    private float maxPullInSpeed = 6f;
    [SerializeField] private LineRenderer rope;

    [Header("Swing")]
    [SerializeField, Tooltip("Acceleration along the swing from holding left/right")]
    private float swingAcceleration = 14f;
    [SerializeField, Tooltip("Speed above which the swing is gently slowed down")]
    private float maxSwingSpeed = 16f;
    [SerializeField] private float overspeedDamping = 20f;
    [SerializeField, Range(0f, 2f)] private float swingDrag = 0.1f;

    [Header("Release")]
    [SerializeField, Range(0.5f, 1f), Tooltip("1 keeps exactly the speed the player had on the rope")]
    private float releaseVelocityMultiplier = 1f;
    [SerializeField] private float maxReleaseSpeed = 18f;

    private Rigidbody2D rb;
    private NewPlayerControllerTest controller;
    private ControlSchemes controls;
    private Transform anchor;
    private float ropeLength;
    private readonly RaycastHit2D[] sightHits = new RaycastHit2D[8];
    private ContactFilter2D sightFilter;

    public bool IsAttached => anchor != null;

    public Vector3 AnchorPosition => anchor.position;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<NewPlayerControllerTest>();
        controls = new ControlSchemes();

        sightFilter = new ContactFilter2D();
        sightFilter.SetLayerMask(obstacleMask);
        sightFilter.useTriggers = false;

        if (rope != null)
        {
            rope.enabled = false;
        }
    }

    private void OnEnable() => controls.Player.Enable();

    private void OnDisable() => controls.Player.Disable();

    private void OnDestroy() => controls.Dispose();

    private void Update()
    {
        if (!controls.Player.Throw.WasPressedThisFrame() || (controller != null && controller.InputsLocked))
        {
            return;
        }

        if (IsAttached)
        {
            Release();
        }
        else
        {
            TryAttach(GetAimDirection());
        }
    }

    private void FixedUpdate()
    {
        if (IsAttached && !HasLineOfSight(anchor))
        {
            Release();
        }
    }

    private void LateUpdate()
    {
        if (rope == null)
        {
            return;
        }

        rope.enabled = IsAttached;
        if (IsAttached)
        {
            rope.SetPosition(0, transform.position);
            rope.SetPosition(1, anchor.position);
        }
    }

    /// <summary>
    /// Applies gravity and swing input for one physics step while hanging from the rope.
    /// </summary>
    public Vector2 Swing(Vector2 velocity, float input, float gravity, float dt)
    {
        velocity.y -= gravity * dt;

        Vector2 toPlayer = rb.position - (Vector2)anchor.position;
        float distance = toPlayer.magnitude;
        if (input != 0 && distance > 0.01f && distance >= ropeLength - tautTolerance)
        {
            Vector2 radial = toPlayer / distance;
            Vector2 tangent = new Vector2(-radial.y, radial.x);
            if (Mathf.Sign(tangent.x) != Mathf.Sign(input))
            {
                tangent = -tangent;
            }

            velocity += tangent * (Mathf.Abs(input) * Mathf.Abs(tangent.x) * swingAcceleration * dt);
        }

        velocity *= Mathf.Max(0f, 1f - swingDrag * dt);

        float speed = velocity.magnitude;
        if (speed > maxSwingSpeed)
        {
            velocity *= Mathf.MoveTowards(speed, maxSwingSpeed, overspeedDamping * dt) / speed;
        }

        return velocity;
    }

    /// <summary>
    /// Stops the velocity from carrying the player past the rope length. Only the outward part of the velocity
    /// is removed, so the swing keeps its speed along the arc.
    /// </summary>
    public Vector2 ConstrainToRope(Vector2 velocity, float dt)
    {
        Vector2 toPlayer = rb.position - (Vector2)anchor.position;
        float distance = toPlayer.magnitude;
        if (distance < 0.0001f)
        {
            return velocity;
        }

        Vector2 radial = toPlayer / distance;
        float outwardSpeed = Vector2.Dot(velocity, radial);
        float allowedOutwardSpeed = Mathf.Max((ropeLength - distance) / dt, -maxPullInSpeed);
        if (outwardSpeed > allowedOutwardSpeed)
        {
            velocity -= radial * (outwardSpeed - allowedOutwardSpeed);
        }

        return velocity;
    }

    public void Release()
    {
        if (!IsAttached)
        {
            return;
        }

        anchor = null;
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity * releaseVelocityMultiplier, maxReleaseSpeed);
    }

    private void TryAttach(Vector2 aimDirection)
    {
        Transform target = FindHookPoint(aimDirection);
        if (target == null)
        {
            return;
        }

        anchor = target;
        ropeLength = Mathf.Clamp(Vector2.Distance(rb.position, anchor.position), minRopeLength, maxHookDistance);
    }

    private Vector2 GetAimDirection()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Camera.main != null)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            return mouseWorld - rb.position;
        }

        return controls.Player.AimController.ReadValue<Vector2>();
    }

    private Transform FindHookPoint(Vector2 aimDirection)
    {
        Transform best = null;
        float bestScore = float.MaxValue;
        bool useAim = aimDirection.sqrMagnitude > 0.04f;

        foreach (GameObject point in GameObject.FindGameObjectsWithTag(hookPointTag))
        {
            Vector2 toPoint = (Vector2)point.transform.position - rb.position;
            float distance = toPoint.magnitude;
            if (distance > maxHookDistance)
            {
                continue;
            }

            float score = distance;
            if (useAim)
            {
                score = Vector2.Angle(aimDirection, toPoint);
                if (score > aimAssistAngle)
                {
                    continue;
                }
            }

            if (score < bestScore && HasLineOfSight(point.transform))
            {
                best = point.transform;
                bestScore = score;
            }
        }

        return best;
    }

    private bool HasLineOfSight(Transform target)
    {
        Vector2 toTarget = (Vector2)target.position - rb.position;
        int count = Physics2D.Raycast(rb.position, toTarget.normalized, sightFilter, sightHits, toTarget.magnitude);
        for (int i = 0; i < count; i++)
        {
            Transform hit = sightHits[i].collider.transform;
            if (sightHits[i].rigidbody != rb && !hit.IsChildOf(target))
            {
                return false;
            }
        }
        return true;
    }
}
