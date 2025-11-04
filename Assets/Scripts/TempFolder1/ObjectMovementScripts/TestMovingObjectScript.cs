
using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Objects.Platform;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

// Enum Class Representing the Current State of a Moving Platform
public enum PlatformState
{
    Idle, // Platform standing still
    Moving, // Platform moving across the path
    Ragdoll // Platform in free fall (Once a platform rag dolls, it detaches from the path)
}

// Requires a Spline Container as it sets the path the platform will move on
[RequireComponent(typeof(SplineContainer))]
public class TestMovingObjectScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private SplineContainer splineContainer;

    private float currentSpeed; // Platform's current speed
    private Dictionary<Rigidbody2D, bool> platformObjects; // Dictionary containing objects touching the platform
    private List<ContactPoint2D> contactPoints; // List of contact points touching the platform's collider

    private PlatformState state; // Moving platform's state
    private float splineLength; // Length of the current spline

    [Space]

    // Platform object. While not stated, it does require a rigidbody2D
    [SerializeField] private GameObject platform;

    [SerializeField] private bool activeByDefault = true; // Sets if the platform is active on start
    [SerializeField] private bool isMovingRight = true; // The direction the platform is moving across the spline

    [Header("Initial Position")]
    [SerializeField, Range(0f, 1f)] private float platformStartLocation; // Where in its path the platform starts
    private float currentPlatformLocation;

    [Header("Platform Speed")]
    [SerializeField] protected float maxSpeed = 1f; // Maximum platform speed
    [SerializeField] private bool useAcceleration = false; // If the platform uses acceleration or starts at max speed

    // Handles Acceleration. Can toggle extra variables by clicking on useAcceleration
    [SerializeField] private float acceleration = 0.25f; // The platform's acceleration

    [Header("Loop Type")]
    [SerializeField] private LoopType loopType; // The loop pathing type

    // LoopType OneWay Specific Variables
    [SerializeField] private float gravity;
    [SerializeField] private bool keepSpeedAfterLoop;

    [Header("On Activated/Deactivated")]
    [SerializeField] private UnityEvent activatedActions;
    [SerializeField] private UnityEvent deactivatedActions;

    // Dictionary of lambda functions that hide/show variables if certain values are selected
    // This is used in conjunction with the TestMovingPlatformEditor so conditional variables aren't always shown
    public static readonly Dictionary<string, Func<TestMovingObjectScript, bool>> conditionalSerializedVars = new()
    {
        // Variables Only If loopType == LoopType.OneWay
        {nameof(gravity), script => script.loopType == LoopType.OneWay},
        {nameof(keepSpeedAfterLoop), script => script.loopType == LoopType.OneWay},

        // Variables Only If useAcceleration == true
        {nameof(acceleration), script => script.useAcceleration},
        {nameof(currentSpeed), script => script.useAcceleration}
    };

    // Runs after any change in the inspector and the game is not running
    private void OnValidate()
    {
        if(!Application.isPlaying)
        {
            // Updates the platform position depending on the platformStartLocation's variable
            splineContainer = GetComponent<SplineContainer>();
            platform.transform.position = splineContainer.EvaluatePosition(0, platformStartLocation);

            // Depending on the loop type, this sets if the spline should have a closed loop type or not
            splineContainer.Splines[0].Closed = ShouldCloseSplineLoop(loopType);
        }
    }

    // Sets up all the starting values for this script to execute
    private void Awake()
    {
        GetComponents();
        InitializeVariables();
        EnsureValidSpline();
    }

    // Gets all the necessary components for this script
    private void GetComponents()
    {
        rb = platform.GetComponent<Rigidbody2D>();
        splineContainer = GetComponent<SplineContainer>();
    }

    // Initializes variables to be the correct starting values and initializes lists
    private void InitializeVariables()
    {
        state = PlatformState.Idle;
        contactPoints = new List<ContactPoint2D>();
        platformObjects = new Dictionary<Rigidbody2D, bool>();
        lastVelocity = Vector2.zero;
        lastPosition = platform.transform.position;
        currentPlatformLocation = platformStartLocation;
    }

    // Given a loop type, returns if that type should have a closed spline knot
    private bool ShouldCloseSplineLoop(LoopType lType)
    {
        switch (lType)
        {
            case LoopType.Wrap:
                return true;
            default:
                return false;
        }
    }

    // Given the current loop type, this tells the user that the spline knot is incorrectly closed/open
    private void EnsureValidSpline()
    {
        bool shouldBeClosed = ShouldCloseSplineLoop(loopType);
        if (splineContainer.Splines[0].Closed != shouldBeClosed)
        {
            if (shouldBeClosed)
            {
                Debug.LogError("Loop Type \"" + loopType + "\" Only Works With A Closed Spline");
            }
            else
            {
                Debug.LogError("Loop Type \"" + loopType + "\" Doesn't Work With A Closed Spline");
            }
        }
    }

    private void Start()
    {
        // Calculates the length of the spline aka the platform's path
        splineLength = splineContainer.CalculateLength();

        // Activates or deactivates that platform depending on activeByDefault
        if (activeByDefault)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }

        // If the loop type is none, make the platform just sit in place
        if (loopType == LoopType.None)
        {
            state = PlatformState.Idle;
        }
    }

    // Move the platform each FixedUpdate
    protected void FixedUpdate() => MovePlatform();

    // Calculates the next place that the platform is moving to
    private Vector3 CalculateNextLocation()
    {
        float distToMove = (currentSpeed * Time.fixedDeltaTime) / splineLength;

        // If not moving right, flip the direction
        if (!isMovingRight)
        {
            distToMove *= -1;
        }

        currentPlatformLocation = Mathf.Clamp01(currentPlatformLocation + distToMove);

        // If the platform location is essentially at 1, but isn't because of floating point values, round it to 1
        if (currentPlatformLocation > 0.999f)
        {
            currentPlatformLocation = 1f;
        }

        return splineContainer.EvaluatePosition(currentPlatformLocation);
    }

    // TODO -- Look at the logic of the ending velocity

    // Turns the platform into a ragdoll
    // When a ragdoll, the platform stops following the spline's path and falls freely with gravity.
    private void RagdollPlatform()
    {
        state = PlatformState.Ragdoll;

        // Makes the platform dynamic (from kinematic) and turns on gravity
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravity;

        // Runs if you want the platform to keep the speed it was currently at when finishing its path
        if (keepSpeedAfterLoop)
        {
            // Gets the ending velocity based on the direction of the last velocity
            Vector2 endingVelocity = lastVelocity.normalized * currentSpeed;
            foreach (KeyValuePair<Rigidbody2D, bool> platformObject in platformObjects)
            {
                if (platformObject.Value)
                {
                    // Removes last frames velocity so we can apply the ending velocity
                    platformObject.Key.linearVelocity -= lastVelocity;
                }

                // Applies an ending velocity to the platform objects
                platformObject.Key.linearVelocity += endingVelocity;
            }

            rb.linearVelocity -= lastVelocity;
            rb.linearVelocity += endingVelocity;
        }
        else
        {
            // If you don't want it to keep its current speed, set its linear velocity to 0
            // However, let the objects on the platform keep moving
            rb.linearVelocity = new Vector2(0f, 0f);
        }

        // Sets the platform to the ragdoll state
        state = PlatformState.Ragdoll;
    }

    // Depending on the loop type, sets how the platform will handle pathing
    private void HandlePlatformPath()
    {
        switch (loopType)
        {
            // If the platform wraps, make it continuously move in a circle
            // Requires a closed spline path
            case LoopType.Wrap:
                if (isMovingRight)
                {
                    if (currentPlatformLocation >= 1)
                    {
                        currentPlatformLocation -= 1;
                    }
                }
                else
                {
                    if (currentPlatformLocation <= 0)
                    {
                        currentPlatformLocation += 1;
                    }
                }

                break;

            // If the platform ping-pongs, it will continuously move back and forth between the path's start and end
            case LoopType.Pingpong:
                if (currentPlatformLocation <= 0 || currentPlatformLocation >= 1)
                {
                    isMovingRight = !isMovingRight;
                }
                break;

            // If the platform is a one-way, it rag dolls once it has finished its path
            case LoopType.OneWay:
                if (isMovingRight && currentPlatformLocation >= 1)
                {
                    RagdollPlatform();
                }
                else if (!isMovingRight && currentPlatformLocation <= 0)
                {
                    RagdollPlatform();
                }
                break;

            // If the platform is a none type, it stays in place
            case LoopType.None :
                state = PlatformState.Idle;
                break;
        }
    }

    // Adds initial speed to objects that just starting touching the platform
    private void AddInitialSpeedToNewObjs(Vector2 initialVelocity)
    {
        foreach (KeyValuePair<Rigidbody2D, bool> platformObject in platformObjects)
        {
            // If the platformObject boolean is true, ignore. If not, continue.
            // Go to ConnectedObjects() for info why the newly added platform objects have their boolean set to false
            if (platformObject.Value)
            {
                continue;
            }

            // Assumes the platform has a positive x velocity
            // If the object's velocity > platform's velocity, keep the object's velocity the same.
            if (initialVelocity.x >= 0 && platformObject.Key.linearVelocity.x >= initialVelocity.x)
            {
                continue;
            }

            // Assumes the platform has a negative x velocity
            // If the object's velocity < platform's velocity, keep the object's velocity the same.
            if (initialVelocity.x < 0 && platformObject.Key.linearVelocity.x <= initialVelocity.x)
            {
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
    void updatePlatformSpeed()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
    }


    private Vector3 lastPosition; // Holds the platform's position in the last frame
    private Vector2 lastVelocity; // Holds the platform's velocity in the last frame
    private void MovePlatformAndObjects(Vector3 nextLocation)
    {
        // Adds initial speed to objects that just started touching the platform
        AddInitialSpeedToNewObjs(lastVelocity);

        // Removes the platform velocity from the last frame and adds the platform velocity for this frame
        Vector2 velocity = (nextLocation - lastPosition) / Time.fixedDeltaTime;
        lastPosition = nextLocation;
        foreach(Rigidbody2D platformObject in platformObjects.Keys.ToArray())
        {
            platformObject.linearVelocity += velocity - lastVelocity;
        }

        rb.linearVelocity += velocity - lastVelocity;

        // Saves the platform's velocity this frame
        lastVelocity = velocity;
    }


    // Main function that moves the platform
    private void MovePlatform()
    {
        // If the platform isn't in the moving state, don't move
        if (state != PlatformState.Moving)
        {
            return;
        }

        // Gets the objects connected to the platform
        GetConnectedObjects();

        // Update the platform speed (Only matters if acceleration is turned on)
        updatePlatformSpeed();

        // Given the platform's loop type, handle its path
        HandlePlatformPath();

        // If the platform started to ragdoll, skip the moving and calculations
        if (state == PlatformState.Ragdoll)
        {
            return;
        }

        Vector3 nextLocation = CalculateNextLocation();

        MovePlatformAndObjects(nextLocation);
    }

    // Gets and holds all the objects on the platform
    private void GetConnectedObjects()
    {
        rb.GetContacts(contactPoints);

        // Sets the boolean values of all platform objects currently touching the platform
        foreach (Rigidbody2D platformObject in platformObjects.Keys.ToArray())
        {
            platformObjects[platformObject] = false;
        }

        // Temporary list holding rigidbodies to add to the platformObjects dictionary
        List<Rigidbody2D> rigidbodiesToAdd = new List<Rigidbody2D>();

        // Goes through all contact points to get the objects touching the platform
        foreach(ContactPoint2D contactPoint in contactPoints)
        {
            // If the object doesn't have a rigidbody, continue
            Rigidbody2D contactRB = contactPoint.rigidbody;
            if (!contactRB)
            {
                continue;
            }

            // If the object was touching the platform last frame, set its boolean value to true
            if (platformObjects.ContainsKey(contactRB))
            {
                platformObjects[contactRB] = true;
            }
            else
            {
                // If the object wasn't touching the platform last frame, add it to the rigidbodiesToAdd list
                rigidbodiesToAdd.Add(contactRB);
            }
        }

        // For all objects that are no longer touching the platform (their boolean value is false), remove them
        foreach (Rigidbody2D platformObject in platformObjects.Keys.ToArray())
        {
            if (platformObjects[platformObject] == false)
            {
                platformObject.linearVelocity -= lastVelocity;
                platformObjects.Remove(platformObject);
            }
        }

        // Add the new rigidbodies to the platformObject's dictionary and set their boolean values to false.
        // They are set to false because of the later section where you apply initial velocity to recent platform objs.
        foreach (Rigidbody2D rigidbody in rigidbodiesToAdd)
        {
            platformObjects[rigidbody] = false;
        }
    }


    // Runs when the platform is activated
    public void Activate()
    {
        // Ignore if rag dolled
        if (state == PlatformState.Ragdoll)
        {
            return;
        }

        state = PlatformState.Moving;

        // If not using acceleration, set platform to max speed
        currentSpeed = useAcceleration ? 0 : maxSpeed;
        activatedActions?.Invoke();
    }

    // Runs when the platform is deactivated
    public void Deactivate()
    {
        // Ignore if rag dolled
        if (state == PlatformState.Ragdoll)
        {
            return;
        }

        // Sets the platform to be idle
        state = PlatformState.Idle;
        foreach (KeyValuePair<Rigidbody2D, bool> platformObject in platformObjects)
        {
            if (platformObject.Value)
            {
                // Remove the last frame's velocity
                // This is so platform objects won't still keep moving from the moving platform after it has stopped.
                platformObject.Key.linearVelocity -= lastVelocity;
            }


        }

        // Set the platform's linear velocity to 0
        rb.linearVelocity = Vector2.zero;
        lastVelocity = Vector2.zero;

        deactivatedActions?.Invoke();
    }


}

