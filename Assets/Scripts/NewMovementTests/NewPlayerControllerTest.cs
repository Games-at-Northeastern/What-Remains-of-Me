using System.Collections.Generic;
using CharacterController;
using UnityEngine;

/// <summary>
/// Test-only player controller for the NewMovementTests scene.
/// Handles running, jumping and air control, and hands control to NewHookTest while swinging.
/// Implements CharacterController2D so the player prefab's animation, sprite flipping and sounds keep working.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class NewPlayerControllerTest : MonoBehaviour, CharacterController2D
{
    [Header("Run")]
    [SerializeField] private float maxRunSpeed = 6f;
    [SerializeField] private float groundAcceleration = 70f;
    [SerializeField] private float groundDeceleration = 55f;
    [SerializeField, Tooltip("Acceleration used when pressing against the current direction of movement")]
    private float turnAcceleration = 110f;

    [Header("Air")]
    [SerializeField] private float maxAirSpeed = 6f;
    [SerializeField] private float airAcceleration = 45f;
    [SerializeField] private float airDeceleration = 10f;
    [SerializeField] private float airTurnAcceleration = 60f;
    [SerializeField, Tooltip("How fast speed above Max Air Speed (e.g. after releasing the hook) bleeds off")]
    private float overspeedDeceleration = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2.8f;
    [SerializeField, Tooltip("Gravity while moving up and holding jump")]
    private float risingGravity = 20f;
    [SerializeField] private float fallGravity = 40f;
    [SerializeField] private float maxFallSpeed = 20f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Collision")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float contactCheckDistance = 0.06f;
    [SerializeField, Range(0f, 1f)] private float minGroundNormalY = 0.65f;

    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private NewHookTest hook;
    private ControlSchemes controls;
    private ContactFilter2D contactFilter;
    private readonly RaycastHit2D[] contactHits = new RaycastHit2D[4];
    private readonly Dictionary<GameObject, Vector2> externalVelocity = new Dictionary<GameObject, Vector2>();
    private Vector2 appliedExternalVelocity;

    private Vector2 moveInput;
    private bool jumpHeld;
    private bool isLocked;
    private Facing facing = Facing.right;
    private float timeSinceJumpPressed = Mathf.Infinity;
    private float timeSinceGrounded = Mathf.Infinity;
    private bool isGrounded;
    private bool isJumping;

    public bool InputsLocked => isLocked;

    public Vector2 InternalVelocity
    {
        get => rb.linearVelocity - appliedExternalVelocity;
        set => rb.linearVelocity = value + appliedExternalVelocity;
    }

    public Vector2 Velocity => rb.linearVelocity;

    public Vector3 position => transform.position;

    public bool Grounded => isGrounded;

    public Vector2 Direction => transform.forward;

    public Facing LeftOrRight => facing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        hook = GetComponent<NewHookTest>();
        controls = new ControlSchemes();

        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(groundMask);
        contactFilter.useTriggers = false;
    }

    private void OnEnable() => controls.Player.Enable();

    private void OnDisable() => controls.Player.Disable();

    private void OnDestroy() => controls.Dispose();

    private void Update()
    {
        if (isLocked)
        {
            return;
        }

        moveInput = controls.Player.Move.ReadValue<Vector2>();
        jumpHeld = controls.Player.Jump.IsPressed();

        if (controls.Player.Jump.WasPressedThisFrame())
        {
            timeSinceJumpPressed = 0;
        }

        if (moveInput.x != 0)
        {
            facing = moveInput.x < 0 ? Facing.left : Facing.right;
        }
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        UpdateGrounded(dt);

        Vector2 velocity = InternalVelocity;

        if (hook != null && hook.IsAttached && !isGrounded)
        {
            if (timeSinceJumpPressed <= jumpBufferTime)
            {
                timeSinceJumpPressed = Mathf.Infinity;
                hook.Release();
                velocity = InternalVelocity;
            }
            else
            {
                velocity = hook.Swing(velocity, moveInput.x, fallGravity, dt);
            }
            isJumping = false;
        }
        else
        {
            velocity.x = GetHorizontalVelocity(velocity.x, dt);
            velocity.y = GetVerticalVelocity(velocity.y, dt);
        }

        if (hook != null && hook.IsAttached)
        {
            velocity = hook.ConstrainToRope(velocity, dt);
        }

        appliedExternalVelocity = Vector2.zero;
        foreach (Vector2 force in externalVelocity.Values)
        {
            appliedExternalVelocity += force;
        }

        rb.linearVelocity = velocity + appliedExternalVelocity;
        timeSinceJumpPressed += dt;
    }

    private void UpdateGrounded(float dt)
    {
        isGrounded = InternalVelocity.y <= 0.01f && TouchingGround();

        if (isGrounded)
        {
            timeSinceGrounded = 0;
            isJumping = false;
        }
        else
        {
            timeSinceGrounded += dt;
        }
    }

    private float GetHorizontalVelocity(float velocityX, float dt)
    {
        float input = moveInput.x;
        float maxSpeed = isGrounded ? maxRunSpeed : maxAirSpeed;
        bool isTurning = input != 0 && velocityX != 0 && Mathf.Sign(input) != Mathf.Sign(velocityX);

        if (!isGrounded && !isTurning && Mathf.Abs(velocityX) > maxAirSpeed)
        {
            return Mathf.MoveTowards(velocityX, Mathf.Sign(velocityX) * maxAirSpeed, overspeedDeceleration * dt);
        }

        float target = input * maxSpeed;
        float rate;
        if (isTurning)
        {
            rate = isGrounded ? turnAcceleration : airTurnAcceleration;
        }
        else if (Mathf.Abs(target) > Mathf.Abs(velocityX))
        {
            rate = isGrounded ? groundAcceleration : airAcceleration;
        }
        else
        {
            rate = isGrounded ? groundDeceleration : airDeceleration;
        }

        return Mathf.MoveTowards(velocityX, target, rate * dt);
    }

    private float GetVerticalVelocity(float velocityY, float dt)
    {
        bool canJump = isGrounded || timeSinceGrounded <= coyoteTime;
        if (timeSinceJumpPressed <= jumpBufferTime && canJump && !isJumping)
        {
            velocityY = Mathf.Sqrt(2f * risingGravity * jumpHeight);
            isJumping = true;
            isGrounded = false;
            timeSinceJumpPressed = Mathf.Infinity;
            timeSinceGrounded = Mathf.Infinity;
        }

        bool useRisingGravity = velocityY > 0 && (!isJumping || jumpHeld);
        float gravity = useRisingGravity ? risingGravity : fallGravity;
        return Mathf.Max(velocityY - gravity * dt, -maxFallSpeed);
    }

    private bool IsTouching(Vector2 direction)
    {
        int count = bodyCollider.Cast(direction, contactFilter, contactHits, contactCheckDistance);
        for (int i = 0; i < count; i++)
        {
            if (Vector2.Dot(contactHits[i].normal, -direction) >= minGroundNormalY)
            {
                return true;
            }
        }
        return false;
    }

    public bool TouchingGround() => IsTouching(Vector2.down);

    public bool TouchingCeiling() => IsTouching(Vector2.up);

    public bool TouchingLeftWall() => IsTouching(Vector2.left);

    public bool TouchingRightWall() => IsTouching(Vector2.right);

    public void AddOrUpdateForce(GameObject ID, Vector2 force) => externalVelocity[ID] = force;

    public void RemoveForce(GameObject ID) => externalVelocity.Remove(ID);

    public void LockInputs()
    {
        isLocked = true;
        moveInput = Vector2.zero;
        jumpHeld = false;
        timeSinceJumpPressed = Mathf.Infinity;
    }

    public void UnlockInputs() => isLocked = false;

    public AnimationType GetAnimationState()
    {
        if (hook != null && hook.IsAttached && !isGrounded)
        {
            return AnimationType.WIRE_SWING;
        }

        if (isGrounded)
        {
            return Mathf.Abs(InternalVelocity.x) > 0.01f ? AnimationType.RUN : AnimationType.IDLE;
        }

        return InternalVelocity.y > 0 ? AnimationType.JUMP_RISING : AnimationType.JUMP_FALLING;
    }
}
