using System.Collections.Generic;
using System.Linq;
using Levels.Objects.Platform;
using PlayerController;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

// TODO -----> MAJOR ISSUE, Because of how rigidbodies work, this script has issues with nested rigidbodies
// For example, a parent having a rigidbody with a child having a rigidbody. The child rigidbody would now
// calculate its current transformed position differently, as it now has to adhere to its parent's physics.
// The problem is if the child is Kinematic, the child can't move using direct velocity setting since it
// would be overwritten by the parent. So, in the future, someone should change this program to handle
// rb.MovePosition instead of setting the velocity directly. Not sure why I didn't do this at the start, but
// I did not think this would be an issue.

// NOTE: The above issue only matters if you really want to nest paths.
// Ex: An Element moves along the Inner Path, and the Inner Path moves along the Outer Path.
// The quick solution is making the elements not children of rigidbodies or setting them to Dynamic.
// The problem with the Dynamic setting is that platforms can be affected by outside forces
// The parent-child problem is more just an issue with hierarchy organization


// Enum Class representing the type of moving object
public enum MovingObjectType
{
    Platform, // Any object that is a platform
    NonPlatform // Any object that is not a platform
}

// Enum Class Representing the Current State of a Moving Platform
public enum MovingObjectState
{
    Idle, // Platform standing still
    Moving, // Platform moving across the path
    Ragdoll // Platform in free fall (Once a platform rag dolls, it detaches from the path)
}

// Requires a Rigidbody2D because that it is handling the moving object's physics
[RequireComponent(typeof(Rigidbody2D))]

// Class representing a moving element on a moving element path
public class MovingElementScript : MonoBehaviour
{
    // The path the moving element is subscribed to
    [SerializeField] private MovingElementPathScript movingElementPath;

    [Header("Initial Position")]
    [SerializeField] [Range(0f, 1f)] private float objectStartLocation; // Where in its path the object starts
    [SerializeField] private bool isMovingRight = true; // The direction the object is moving across the spline

    // Object type, either a platform or not
    [SerializeField] private MovingObjectType movingObjectType = MovingObjectType.NonPlatform;

    // Platform-Specific Variables (These variables check if the player is on this platform):

    // If MovingObjectType.Platform is the movingObjectType, show this header
    [ShowIf(nameof(movingObjectType), MovingObjectType.Platform)]
    [Space] [ReadOnly] [SerializeField] private string GroundHeader = "Platform Ground Detection";

    // Y-offset for the moving platform grounded trigger (platform ground collision is for the player only)
    [ShowIf(nameof(movingObjectType), MovingObjectType.Platform)]
    [SerializeField] private float yOffset = 0.2f;

    // Y-scale for the platform grounded trigger (X-scale is decided by the 2D collider width)
    [ShowIf(nameof(movingObjectType), MovingObjectType.Platform)]
    [SerializeField] private float yScale = 0.06f;

    // Color for the platform grounded trigger
    [ShowIf(nameof(movingObjectType), MovingObjectType.Platform)]
    [SerializeField] private Color triggerColor = Color.red;

    // Activated/Deactivated Actions

    [Header("On Activated/Deactivated")]
    [SerializeField] private UnityEvent activatedActions;

    [SerializeField] private UnityEvent deactivatedActions;

    private List<ContactPoint2D> contactPoints; // List of contact points touching the platform's collider
    private float currentElementLocation;
    private float currentSpeed;

    private Vector3 lastPosition; // Holds the platform's position in the last frame
    private Vector2 lastVelocity; // Holds the platform's velocity in the last frame

    private int pingPongDestination; // Variable used to fix the issue that the platform would keep changing direction if on an edge
    private Collider2D platformCollider; // Reference to the object's collider (ONLY MATTERS IF OBJECT TYPE == Platform)
    private Dictionary<Rigidbody2D, bool> platformObjects; // Dictionary containing objects touching the platform

    private PlayerController2D player; // Reference to the player if they are on the platform
    private Rigidbody2D rb; // Reference to this script's rigidbody
    private SplineContainer splinePath; // Path that the platform is moving on
    private MovingObjectState state; // Current movement state of the platform

    private Vector3 triggerPosition; // Position of a platform's ground trigger
    private Vector3 triggerScale; // Scale of a platform's ground trigger
    private bool usingAcceleration; // Boolean holding the usingAcceleration info from parent path


    // Get all required components and set variables to default values
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (movingObjectType == MovingObjectType.Platform) {
            platformCollider = GetComponent<Collider2D>();
        }

        platformObjects = new Dictionary<Rigidbody2D, bool>();
        contactPoints = new List<ContactPoint2D>();

        state = MovingObjectState.Idle;
        lastVelocity = Vector2.zero;
        lastPosition = transform.position;
        currentElementLocation = objectStartLocation;

        // Subscribes this moving element to a path
        if (movingElementPath != null) {
            splinePath = movingElementPath.addMovingObject(this);
        }

        // Ensures that in ping-pong mode the platform moves correctly on path edges
        pingPongDestination = isMovingRight ? 1 : 0;
    }


    // Graphical representation for the moving platform grounded triggers
    private void OnDrawGizmosSelected()
    {
        // Only show this graphic if the moving object is a platform
        if (movingObjectType != MovingObjectType.Platform) {
            return;
        }

        // Platforms require a collider to show this grounded trigger graphic
        if (GetComponent<Collider>() == null) {
            platformCollider = GetComponent<Collider2D>();
            CalculateTriggerTransform();
        } else {
            Debug.LogError("Platforms Require A Collider");
        }

        Gizmos.color = triggerColor;
        Gizmos.DrawWireCube(triggerPosition, triggerScale);
    }

    // Runs only in edit mode
    private void OnValidate()
    {
        if (movingElementPath == null) {
            return;
        }

        // Get the spline path from the moving element's path
        splinePath = movingElementPath.GetSplinePath();

        // This handles the element start location during edit mode
        // This is to help the user see where the platform should start
        if (splinePath is not null) {
            transform.position = splinePath.EvaluatePosition(0, objectStartLocation);
        }
    }

    // Calculates the bounds for the platform grounded trigger
    private void CalculateTriggerTransform()
    {
        Vector3 platformPos = transform.position;
        triggerScale = new Vector3(platformCollider.bounds.extents.x * 2, yScale, 1);
        triggerPosition = new Vector3(platformPos.x, platformPos.y + yOffset, platformPos.z);
    }

    // Handles what the element will be doing depending on the LoopType
    private void HandleObjectPath(MovingElementPathScript movingElementPath)
    {
        switch (movingElementPath.GetLoopType()) {
            // If the element wraps, make it continuously move in a circle
            // Requires a closed spline path
            case LoopType.Wrap:
                if (isMovingRight) {
                    if (currentElementLocation >= 1) {
                        currentElementLocation -= 1;
                    }
                } else {
                    if (currentElementLocation <= 0) {
                        currentElementLocation += 1;
                    }
                }

                break;

            // If the element ping-pongs, it will continuously move back and forth between the path's start and end
            case LoopType.Pingpong:
                if (currentElementLocation <= 0 && pingPongDestination == 0 ||
                    currentElementLocation >= 1 && pingPongDestination == 1) {

                    currentElementLocation = Mathf.Clamp(currentElementLocation, 0, 1);
                    isMovingRight = !isMovingRight;
                    pingPongDestination = isMovingRight ? 1 : 0;

                    if (movingElementPath.GetUseAcceleration() && movingElementPath.ResetSpeedAtPathEnds()) {
                        currentSpeed = 0;
                    }
                }

                break;
            // If the element is a one-way, it rag dolls once it has finished its path
            case LoopType.OneWay:

                float gravityScale = movingElementPath.GetGravityScale();
                bool keepSpeedAfterLoop = movingElementPath.GetKeepSpeedAfterLoop();
                if (isMovingRight && currentElementLocation >= 1) {
                    RagdollPlatform(keepSpeedAfterLoop, gravityScale);
                } else if (!isMovingRight && currentElementLocation <= 0) {
                    RagdollPlatform(keepSpeedAfterLoop, gravityScale);
                }

                break;
            // If the element is a none type, it stays in place
            case LoopType.None:
                state = MovingObjectState.Idle;
                break;
        }
    }

    // Adds initial speed to objects that just starting touching the platform
    // NOTE: This treats all objects as having "sticky feet", meaning they always move at the same speed as the platform
    // These calculations essentially ignore friction
    private void AddInitialSpeedToNewObjs(Vector2 initialVelocity)
    {
        foreach (KeyValuePair<Rigidbody2D, bool> platformObject in platformObjects) {
            // If the platformObject boolean is true, ignore. If not, continue.
            // Go to ConnectedObjects() for info why the newly added platform objects have their boolean set to false
            if (platformObject.Value) {
                continue;
            }

            // Assumes the platform has a positive x velocity
            // If the object's velocity > platform's velocity, keep the object's velocity the same.
            if (initialVelocity.x >= 0 && platformObject.Key.linearVelocity.x >= initialVelocity.x) {
                continue;
            }

            // Assumes the platform has a negative x velocity
            // If the object's velocity < platform's velocity, keep the object's velocity the same.
            if (initialVelocity.x < 0 && platformObject.Key.linearVelocity.x <= initialVelocity.x) {
                continue;
            }

            // If needed, add the platform's initial x velocity to the platform object
            platformObject.Key.linearVelocity =
                new Vector2(initialVelocity.x, platformObject.Key.linearVelocity.y);

        }
    }

    // Updates the element's speed. Only matters if acceleration is turned on
    private void UpdateElementSpeed(MovingElementPathScript movingElementPath)
    {
        float maxSpeed = movingElementPath.GetMaxSpeed();
        if (currentSpeed < maxSpeed) {
            currentSpeed += movingElementPath.GetAcceleration() * Time.fixedDeltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
    }

    // Calculates the next place that the element is moving to
    private Vector3 CalculateNextLocation(MovingElementPathScript movingElementPath)
    {
        float distToMove = currentSpeed * Time.fixedDeltaTime / movingElementPath.GetSplineLength();

        // If not moving right, flip the direction
        if (!isMovingRight) {
            distToMove *= -1;
        }

        currentElementLocation = Mathf.Clamp01(currentElementLocation + distToMove);

        // If the element location is essentially at 1, but isn't because of floating point values, round it to 1
        if (currentElementLocation > 0.9999999f) {
            currentElementLocation = 1f;
        }

        return movingElementPath.GetSplinePath().EvaluatePosition(currentElementLocation);
    }

    // Main function that moves the element
    public void Move(MovingElementPathScript movingElementPath)
    {
        // If the element isn't in the moving state, don't move
        if (state != MovingObjectState.Moving) {
            return;
        }

        // Update the element speed (Only matters if acceleration is turned on)
        UpdateElementSpeed(movingElementPath);

        // Given the element's loop type, handle its path
        HandleObjectPath(movingElementPath);

        // If the element started to ragdoll, skip the moving and calculations
        if (state == MovingObjectState.Ragdoll) {
            movingElementPath.RemoveMovingObject(this);
            return;
        }

        Vector3 nextLocation = CalculateNextLocation(movingElementPath);

        MoveObject(nextLocation);
    }

    // Runs when the element is activated
    public void Activate(MovingElementPathScript movingElementPath)
    {
        state = MovingObjectState.Moving;

        // If not using acceleration, set the element to max speed
        currentSpeed = movingElementPath.GetUseAcceleration() ? 0 : movingElementPath.GetMaxSpeed();
        activatedActions?.Invoke();
    }

    // Runs when the element is deactivated
    public void Deactivate()
    {
        // Sets the element to be idle
        state = MovingObjectState.Idle;

        // Won't do anything if not a platform because a non-platform can't carry objects
        foreach (KeyValuePair<Rigidbody2D, bool> platformObject in platformObjects) {
            if (platformObject.Value) {
                // Remove the last frame's velocity
                // This is so platform objects won't still keep moving from the moving platform after it has stopped.
                platformObject.Key.linearVelocity -= lastVelocity;
            }
        }

        lastPosition = transform.position;
        lastVelocity = Vector2.zero;

        // Set the element's linear velocity to 0
        rb.linearVelocity = Vector2.zero;

        deactivatedActions?.Invoke();
    }

    // Turns the element into a ragdoll
    // When a ragdoll, the element stops following its assigned path and falls freely with gravity.
    private void RagdollPlatform(bool keepSpeedAfterLoop, float gravity)
    {
        state = MovingObjectState.Ragdoll;

        // Makes the platform dynamic (from kinematic) and turns on gravity
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravity;

        // Runs if you want the platform to keep the speed it was currently at when finishing its path
        if (keepSpeedAfterLoop) {
            // Gets the ending velocity based on the direction of the last velocity
            Vector2 endingVelocity = lastVelocity.normalized * currentSpeed;

            foreach (KeyValuePair<Rigidbody2D, bool> platformObject in platformObjects) {
                if (platformObject.Value) {
                    // Removes last frames velocity so we can apply the ending velocity
                    platformObject.Key.linearVelocity -= lastVelocity;
                }

                // Applies an ending velocity to the platform objects
                platformObject.Key.linearVelocity += endingVelocity;
            }

            rb.linearVelocity -= lastVelocity;
            rb.linearVelocity += endingVelocity;
        } else {
            // If you don't want it to keep its current speed, set its linear velocity to 0
            // However, let the objects on the platform keep moving
            rb.linearVelocity = new Vector2(0f, 0f);
        }

        // Sets the platform to the ragdoll state
        state = MovingObjectState.Ragdoll;
    }


    // Function that physically moves this element 
    private void MoveObject(Vector3 nextLocation)
    {
        // Removes the element velocity from the last frame and adds the element velocity for this frame
        Vector2 velocity = (nextLocation - lastPosition) / Time.fixedDeltaTime;
        lastPosition = nextLocation;

        // If the object is a platform, move the objects on the platform
        if (movingObjectType == MovingObjectType.Platform) {
            MovePlatformObjects(velocity, lastVelocity);
            HandlePlayerOnPlatform();
        }

        rb.linearVelocity += velocity - lastVelocity;

        // Saves the element's velocity this frame
        lastVelocity = velocity;
    }

    // Moves the objects on top of the platform so they move with the platform
    private void MovePlatformObjects(Vector2 velocity, Vector2 lastVel)
    {
        // Adds initial speed to objects that just started touching the platform
        GetPlatformConnectedObjects();
        AddInitialSpeedToNewObjs(lastVel);

        foreach (Rigidbody2D platformObject in platformObjects.Keys.ToArray()) {
            platformObject.linearVelocity += velocity - lastVel;
        }
    }

    // Adds forces to the player if on top of the platform.
    // This is done separately from the other objects on the platform because of how the player's physics is programmed
    private void HandlePlayerOnPlatform()
    {
        // Sets up the location of the grounded trigger
        CalculateTriggerTransform();

        // Checks what is colliding with the grounded trigger.
        // Sets collision to look at all layers in case the Player layer ever changes
        Collider2D[] collidingObjects = Physics2D.OverlapBoxAll(triggerPosition, triggerScale, 0, ~0);

        bool playerOnPlatform = false;
        foreach (Collider2D collidingObject in collidingObjects) {
            // Find any object that has the playerController component
            PlayerController2D playerController = collidingObject.GetComponent<PlayerController2D>();
            if (playerController is not null) {
                // If the player is on the platform, edit its OnMovingPlatform status
                player = playerController;
                player.OnMovingPlatform = true;
                playerOnPlatform = true;
            }
        }

        // Remove the platform force on the player if they leave the grounded trigger
        if (!playerOnPlatform && player != null) {
            player.RemoveForce(gameObject);
            player.OnMovingPlatform = false;
            player = null;
        }

        // Add force each frame if the player is grounded on the platform
        if (player is not null) {
            player.AddOrUpdateForce(gameObject, rb.linearVelocity);
            player.CombineCurrentVelocities();
        }
    }

    // Gets and holds all the objects on the platform
    private void GetPlatformConnectedObjects()
    {
        // Get everything colliding with the platform
        rb.GetContacts(contactPoints);

        // Sets the boolean values of all platform objects currently touching the platform
        foreach (Rigidbody2D platformObject in platformObjects.Keys.ToArray()) {
            platformObjects[platformObject] = false;
        }

        // Temporary list holding rigidbodies to add to the platformObjects dictionary
        List<Rigidbody2D> rigidbodiesToAdd = new List<Rigidbody2D>();

        // Goes through all contact points to get the objects touching the platform
        foreach (ContactPoint2D contactPoint in contactPoints) {
            // If the object doesn't have a rigidbody, continue
            Rigidbody2D contactRB = contactPoint.rigidbody;
            if (!contactRB) {
                continue;
            }

            // If the object was touching the platform last frame, set its boolean value to true
            if (platformObjects.ContainsKey(contactRB)) {
                platformObjects[contactRB] = true;
            } else {
                // If the object wasn't touching the platform last frame, add it to the rigidbodiesToAdd list
                rigidbodiesToAdd.Add(contactRB);
            }
        }

        // For all objects that are no longer touching the platform (their boolean value is false), remove them
        foreach (Rigidbody2D platformObject in platformObjects.Keys.ToArray()) {
            if (platformObjects[platformObject] == false) {
                platformObject.linearVelocity -= lastVelocity;
                platformObjects.Remove(platformObject);
            }
        }

        // Add the new rigidbodies to the platformObject's dictionary and set their boolean values to false.
        // They are set to false because of the later section where you apply initial velocity to recent platform objs.
        foreach (Rigidbody2D rigidbody in rigidbodiesToAdd) {
            platformObjects[rigidbody] = false;
        }
    }
}
