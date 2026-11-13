using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)] // run before RaycastPlayerController's FixedUpdate
public class GrappleModule : MonoBehaviour, IPlayerModule
{
    public RaycastPlayerController Controller { get; set; }
    public float HealthPercentile { get; private set; } = 1f;

    [SerializeField] string hookTag = "Outlet";
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float swingAcceleration = 20f;    // units/s^2
    [SerializeField] float ropeGravityMultiplier = 2f; // 2 = double gravity on the rope
    [SerializeField] LineRenderer rope;                // optional

    Transform anchor;
    float ropeLength;
    float swingInput; // cached from Update, used in FixedUpdate

    public bool IsAttached => anchor != null;

    void OnEnable() => Controller = GetComponent<RaycastPlayerController>();

    void OnDisable()
    {
        anchor = null;
        if (Controller != null)
            Controller.OnRope = false;
        if (rope != null)
            rope.enabled = false;
    }

    public void Damage(float amount)
    {
        HealthPercentile = Mathf.Clamp01(HealthPercentile - amount);
        if (HealthPercentile <= 0f)
            enabled = false;
    }

    void Update()
    {
        var mouse = Mouse.current;
        var kb = Keyboard.current;

        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            if (IsAttached)
                anchor = null;
            else
                TryAttach(mouse);
        }

        swingInput = 0f;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)
                swingInput -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed)
                swingInput += 1f;
        }

        Controller.OnRope = IsAttached;
    }

    void FixedUpdate()
    {
        if (IsAttached && !HasLineOfSight(anchor))
            anchor = null;

        Controller.OnRope = IsAttached;
        if (!IsAttached)
            return;

        Vector2 toPlayer = (Vector2)transform.position - (Vector2)anchor.position;
        float distance = toPlayer.magnitude;
        if (distance < 0.0001f)
            return;

        Vector2 radial = toPlayer / distance;      // anchor to player
        Vector2 v = Controller.MoveDisplacement;   // units per step
        float g = Controller.GetGravity();         // units per step^2
        float dt = Time.fixedDeltaTime;

        // 1. Swing input: push along the arc (strongest at the bottom, fades at the sides)
        if (swingInput != 0f)
        {
            Vector2 tangent = new Vector2(-radial.y, radial.x);
            if (Mathf.Sign(tangent.x) != Mathf.Sign(swingInput))
                tangent = -tangent;
            v += tangent * (Mathf.Abs(tangent.x) * swingAcceleration * dt * dt);
        }

        // Extra gravity while on the rope
        //v.y -= g * (ropeGravityMultiplier - 1f);

        // Rope constraint. outward and allowed variables are to ensure that the string only exerts pull forces, not pushes
        v.y -= g;
        float outward = Vector2.Dot(v, radial); //negative if velcotiy is towards the anchor
        float allowed = ropeLength - distance; //negative if distance further than rope length
        if (outward > allowed)
            v -= radial * (outward - allowed); // remove only the outward part
        v.y += g;

        Controller.MoveDisplacement = v;
    }

    void LateUpdate()
    {
        if (rope == null)
            return;
        rope.enabled = IsAttached;
        if (IsAttached)
        {
            rope.positionCount = 2;
            rope.SetPosition(0, transform.position);
            rope.SetPosition(1, anchor.position);
        }
    }

    void TryAttach(Mouse mouse)
    {
        if (Camera.main == null)
            return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
        Vector2 pos = transform.position;
        float best = float.MaxValue;

        foreach (GameObject point in GameObject.FindGameObjectsWithTag(hookTag))
        {
            Vector2 p = point.transform.position;
            if (Vector2.Distance(pos, p) > maxDistance)
                continue;

            float d = Vector2.Distance(mouseWorld, p);
            if (d < best && HasLineOfSight(point.transform))
            {
                best = d;
                anchor = point.transform;
            }
        }

        if (IsAttached)
            ropeLength = Vector2.Distance(pos, anchor.position);
    }

    bool HasLineOfSight(Transform target)
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, Controller.GroundLayer);
        return !hit || hit.transform.IsChildOf(target);
    }
}