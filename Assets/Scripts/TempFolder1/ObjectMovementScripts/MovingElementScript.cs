using System.Collections.Generic;
using System.Linq;
using Levels.Objects.Platform;
using PlayerController;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;
public enum MovingObjectType
{
    Platform,
    Outlet
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
public class MovingObjectScript : MonoBehaviour
{
    [SerializeField] private MovingObjectPathScript movingObjectPath;

    [Header("Initial Position")]
    [SerializeField] [Range(0f, 1f)] private float objectStartLocation; // Where in its path the object starts
    [SerializeField] private bool isMovingRight = true; // The direction the object is moving across the spline

    [SerializeField] private MovingObjectType movingObjectType;

    [ShowIf(nameof(movingObjectType), MovingObjectType.Platform)]
    [Space] [ReadOnly] [SerializeField] private string GroundHeader = "Platform Ground Detection";

    [ShowIf(nameof(movingObjectType), MovingObjectType.Platform)]
    [SerializeField] private float yOffset = 0.2f;
    [ShowIf(nameof(movingObjectType), MovingObjectType.Platform)]
    [SerializeField] private float yScale = 0.06f;

    [ShowIf(nameof(movingObjectType), MovingObjectType.Platform)] [SerializeField]
    private Color triggerColor = Color.red;

    [Header("On Activated/Deactivated")]
    [SerializeField] private UnityEvent activatedActions;

    [SerializeField] private UnityEvent deactivatedActions;

    // Enum Class Representing Moving Object Types
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<ContactPoint2D> contactPoints; // List of contact points touching the platform's collider
    private float currentPlatformLocation;
    private float currentSpeed;

    private Vector3 lastPosition; // Holds the platform's position in the last frame
    private Vector2 lastVelocity; // Holds the platform's velocity in the last frame

    private MovingObjectPathScript movingObjectPathReference;

    private int pingPongDestination;
    private Collider2D platformCollider;
    private Dictionary<Rigidbody2D, bool> platformObjects; // Dictionary containing objects touching the platform

    private PlayerController2D player;
    private Rigidbody2D rb;
    private SplineContainer splinePath;
    private MovingObjectState state;

    private Vector3 triggerPosition;
    private Vector3 triggerScale;
    private bool usingAcceleration;


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
        currentPlatformLocation = objectStartLocation;

        if (movingObjectPath != null) {
            splinePath = movingObjectPath.addMovingObject(this);
        }

        pingPongDestination = isMovingRight ? 1 : 0;
    }



    private void OnDrawGizmosSelected()
    {
        if (movingObjectType != MovingObjectType.Platform) {
            return;
        }
        if (GetComponent<Collider>() == null) {
            platformCollider = GetComponent<Collider2D>();
            CalculateTriggerTransform();
        } else {
            Debug.LogError("Platforms Require A Collider");
        }

        Gizmos.color = triggerColor;
        Gizmos.DrawWireCube(triggerPosition, triggerScale);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) {
            if (movingObjectPath == null) {
                return;
            }

            splinePath = movingObjectPath.GetSplinePath();

            if (splinePath is not null) {
                transform.position = splinePath.EvaluatePosition(0, objectStartLocation);
            }

        }
    }

    private void CalculateTriggerTransform()
    {
        Vector3 platformPos = transform.position;
        triggerScale = new Vector3(platformCollider.bounds.extents.x * 2, yScale, 1);
        triggerPosition = new Vector3(platformPos.x, platformPos.y + yOffset, platformPos.z);
    }

    private void HandleObjectPath(MovingObjectPathScript movingObjectPath)
    {
        switch (movingObjectPath.GetLoopType()) {
            // If the platform wraps, make it continuously move in a circle
            // Requires a closed spline path
            case LoopType.Wrap:
                if (isMovingRight) {
                    if (currentPlatformLocation >= 1) {
                        currentPlatformLocation -= 1;
                    }
                } else {
                    if (currentPlatformLocation <= 0) {
                        currentPlatformLocation += 1;
                    }
                }

                break;

            // If the platform ping-pongs, it will continuously move back and forth between the path's start and end
            case LoopType.Pingpong:
                if (currentPlatformLocation <= 0 && pingPongDestination == 0 || currentPlatformLocation >= 1 && pingPongDestination == 1) {
                    currentPlatformLocation = Mathf.Clamp(currentPlatformLocation, 0, 1);
                    isMovingRight = !isMovingRight;
                    pingPongDestination = isMovingRight ? 1 : 0;

                    if (movingObjectPath.GetUseAcceleration() && movingObjectPath.ResetSpeedAtPathEnds()) {
                        currentSpeed = 0;
                    }
                }

                break;
            // If the platform is a one-way, it rag dolls once it has finished its path
            case LoopType.OneWay:

                float gravityScale = movingObjectPath.GetGravityScale();
                bool keepSpeedAfterLoop = movingObjectPath.GetKeepSpeedAfterLoop();
                if (isMovingRight && currentPlatformLocation >= 1) {
                    RagdollPlatform(keepSpeedAfterLoop, gravityScale);
                } else if (!isMovingRight && currentPlatformLocation <= 0) {
                    RagdollPlatform(keepSpeedAfterLoop, gravityScale);
                }

                break;
            // If the platform is a none type, it stays in place
            case LoopType.None:
                state = MovingObjectState.Idle;
                break;
        }
    }

// Adds initial speed to objects that just starting touching the platform
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

            // With these physics, the objects currently have "sticky feet" when it comes to platforms.
            // These calculations don't take friction into account
        }
    }

// Updates the platform's speed. Only matters if acceleration is turned on
    private void updatePlatformSpeed(MovingObjectPathScript movingObjectPath)
    {
        float maxSpeed = movingObjectPath.GetMaxSpeed();
        if (currentSpeed < maxSpeed) {
            currentSpeed += movingObjectPath.GetAcceleration() * Time.fixedDeltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        Debug.Log(currentSpeed);
    }

// Calculates the next place that the platform is moving to
    private Vector3 CalculateNextLocation(MovingObjectPathScript movingObjectPath)
    {
        float distToMove = currentSpeed * Time.fixedDeltaTime / movingObjectPath.GetSplineLength();

        // If not moving right, flip the direction
        if (!isMovingRight) {
            distToMove *= -1;
        }

        Debug.Log("Start: " + currentPlatformLocation);
        currentPlatformLocation = Mathf.Clamp01(currentPlatformLocation + distToMove);
        Debug.Log("End: " + currentPlatformLocation);

        // If the platform location is essentially at 1, but isn't because of floating point values, round it to 1
        if (currentPlatformLocation > 0.9999999f) {
            currentPlatformLocation = 1f;
        }

        return movingObjectPath.GetSplinePath().EvaluatePosition(currentPlatformLocation);
    }

    public void Move(MovingObjectPathScript movingObjectPath)
    {
        // If the platform isn't in the moving state, don't move
        if (state != MovingObjectState.Moving) {
            return;
        }

        // Update the platform speed (Only matters if acceleration is turned on)
        updatePlatformSpeed(movingObjectPath);

        // Given the platform's loop type, handle its path
        HandleObjectPath(movingObjectPath);

        // If the platform started to ragdoll, skip the moving and calculations
        if (state == MovingObjectState.Ragdoll) {
            movingObjectPath.RemoveMovingObject(this);
            return;
        }

        Vector3 nextLocation = CalculateNextLocation(movingObjectPath);

        MoveObject(nextLocation);
    }

// Runs when the platform is activated
    public void Activate(MovingObjectPathScript movingObjectPath)
    {
        state = MovingObjectState.Moving;

        // If not using acceleration, set platform to max speed
        currentSpeed = movingObjectPath.GetUseAcceleration() ? 0 : movingObjectPath.GetMaxSpeed();
        activatedActions?.Invoke();
    }

// Runs when the platform is deactivated
    public void Deactivate()
    {
        // Sets the platform to be idle
        state = MovingObjectState.Idle;

        // Won't do anything if not a platform because it can't get platform objects
        foreach (KeyValuePair<Rigidbody2D, bool> platformObject in platformObjects) {
            if (platformObject.Value) {
                // Remove the last frame's velocity
                // This is so platform objects won't still keep moving from the moving platform after it has stopped.
                platformObject.Key.linearVelocity -= lastVelocity;
            }
        }

        lastPosition = transform.position;
        // Set the platform's linear velocity to 0
        rb.linearVelocity = Vector2.zero;
        lastVelocity = Vector2.zero;

        deactivatedActions?.Invoke();
    }

    public MovingObjectPathScript getPath()
    {
        return movingObjectPath;
    }

    // Turns the platform into a ragdoll
// When a ragdoll, the platform stops following the spline's path and falls freely with gravity.
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


    private void MoveObject(Vector3 nextLocation)
    {
        // Removes the platform velocity from the last frame and adds the platform velocity for this frame
        Vector2 velocity = (nextLocation - lastPosition) / Time.fixedDeltaTime;
        lastPosition = nextLocation;

        // If the object is a platform, move the platform
        if (movingObjectType == MovingObjectType.Platform) {
            MovePlatformObjects(velocity, lastVelocity);
            HandlePlayerOnPlatform();
        }

        rb.linearVelocity += velocity - lastVelocity;

        // Saves the platform's velocity this frame
        lastVelocity = velocity;
    }

    private void MovePlatformObjects(Vector2 velocity, Vector2 lastVel)
    {
        // Adds initial speed to objects that just started touching the platform
        GetPlatformConnectedObjects();
        AddInitialSpeedToNewObjs(lastVel);

        foreach (Rigidbody2D platformObject in platformObjects.Keys.ToArray()) {
            platformObject.linearVelocity += velocity - lastVel;
        }
    }

    private void HandlePlayerOnPlatform()
    {
        CalculateTriggerTransform();

        // Setting collision to look at all layers in case the Player layer ever changes
        Collider2D[] collidingObjects = Physics2D.OverlapBoxAll(triggerPosition, triggerScale, 0, ~0);

        bool playerOnPlatform = false;
        foreach (Collider2D collidingObject in collidingObjects) {
            PlayerController2D playerController = collidingObject.GetComponent<PlayerController2D>();
            if (playerController is not null) {
                player = playerController;
                player.OnMovingPlatform = true;
                playerOnPlatform = true;
                Debug.Log("Player On Platform");
            }
        }

        if (!playerOnPlatform && player != null) {
            player.RemoveForce(gameObject);
            player.OnMovingPlatform = false;
            player = null;
        }

        if (player is not null) {
            player.AddOrUpdateForce(gameObject, rb.linearVelocity);
            player.CombineCurrentVelocities();
        }
    }

// Gets and holds all the objects on the platform
    private void GetPlatformConnectedObjects()
    {
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

#if UNITY_EDITOR
[CustomEditor(typeof(MovingObjectScript))]
[CanEditMultipleObjects]
internal class MyComponentEditor : Editor
{
    // ...
}
#endif
