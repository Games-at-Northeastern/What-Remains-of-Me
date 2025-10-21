
using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Objects.Platform;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Splines;

/// <summary>
/// A custom component that moves the gameobject it's on along a given path.
/// </summary>
[RequireComponent(typeof(SplineContainer))]
public class TestMovingObjectScript : MonoBehaviour
{
    // Kept Hidden because of Inspector GUI Additions

    private Rigidbody2D rb;
    private SplineContainer splineContainer;
    private float splineLength;



    private int totalDest;
    private bool moving;
    private bool _completed = false;

    private LoopType _runtimeLoopType;

    [SerializeField] private GameObject platform;
    private List<Rigidbody2D> platformObjects;

    [Space(15)]

    [SerializeField] private bool _activeByDefault;
    [SerializeField] private bool _rotateToDirection = false;

    [Header("Initial Position")]
    [SerializeField, Range(0f, 1f)] private float platformStartLocation;
    private float platformLocation;

    [Header("Speed")]
    [SerializeField] protected float maxSpeed = 1f;
    [SerializeField] protected float acceleration = 0.25f;
    [SerializeField] private float currentSpeed;

    [Header("Loop Info")]
    [SerializeField] private LoopType _loopType = LoopType.Wrap;
    [SerializeField] private bool _isMovingRight = true;

    [Header("On Activated/Deactivated")]
    [SerializeField] UnityEvent _activatedActions;
    [SerializeField] UnityEvent _deactivatedActions;

    private void OnValidate()
    {
        if(!Application.isPlaying)
        {
            splineContainer = GetComponent<SplineContainer>();
            platform.transform.position = splineContainer.EvaluatePosition(0, platformStartLocation);
        }
    }

    private void Awake()
    {
        lastVelocity = Vector2.zero;
        lastPosition = platform.transform.position;
        objNum = 0;
    }

    private void Start()
    {
        rb = platform.GetComponent<Rigidbody2D>();

        splineContainer = GetComponent<SplineContainer>();
        splineLength = splineContainer.CalculateLength();

        platformObjects = platform.GetComponent<MovingPlatformCollision>().objectsOnPlatform;

        platformLocation = platformStartLocation;
        currentSpeed = maxSpeed;

        if (_activeByDefault)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
        Debug.Log(moving);

        /*if (TryGetComponent(out Outlet outlet))
        {
            outlet.SetMovement(this);
        }*/
    }

    // TODO There are a lot of leftover debug logs bc i think there's still a bug here. It just kinda went away
    // last time so I don't know if it's fixed or just hiding. Essentially, the moving element would skip a point
    // sometimes when affected by a HecticMovementEffector. I let atlas swing from a super-fast, rapidly-changing moving
    // outlet for like 3 minutes and saw nothing, so im assuming the bug just vanished. Definitely wrong, but whatever.

    /// <summary>
    /// Sets up an element to start at the initial point going to the next.
    /// </summary>


    protected void FixedUpdate() => MovePlatform();



    private Vector3 lastPosition;
    private Vector3 CalculateNextLocation()
    {
        float distToMove = (currentSpeed * Time.fixedDeltaTime) / splineLength;

        if (!_isMovingRight)
        {
            distToMove *= -1;
        }
        platformLocation = Mathf.Clamp(platformLocation + distToMove, 0f, 1f);

        return splineContainer.EvaluatePosition(platformLocation);
    }

    private int objNum = 0;


    private void AddInitialSpeedToNewObjs(Vector2 initialVelocity)
    {
        for (int i = objNum; i < platformObjects.Count; i++)
        {
            if (platformObjects[i].linearVelocity.x >= initialVelocity.x)
            {
                break;
            }

            // Player currently has "sticky feet" when it comes to platforms as it ignores friction.
            // The player just matches the speed of the platform and nothing else
            platformObjects[i].linearVelocity = new Vector2(initialVelocity.x, platformObjects[i].linearVelocity.y);
        }

        objNum = platformObjects.Count;
    }


    private Vector2 lastVelocity;
    private void MovePlatformAndObjects(Vector3 nextLocation)
    {
        AddInitialSpeedToNewObjs(lastVelocity);

        Vector2 velocity = (nextLocation - lastPosition) / Time.fixedDeltaTime;
        lastPosition = nextLocation;
        for(int i = 0; i < platformObjects.Count; i++)
        {
           platformObjects[i].linearVelocity += velocity - lastVelocity;
        }

        Debug.Log(lastVelocity);
        rb.linearVelocity += velocity - lastVelocity;

        lastVelocity = velocity;
    }


    private void MovePlatform()
    {
        if (!moving)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector3 nextLocation = CalculateNextLocation();

        MovePlatformAndObjects(nextLocation);

        // If the platform has completed the path, go the other direction
        if (platformLocation <= 0 || platformLocation >= 1)
        {
            _isMovingRight = !_isMovingRight;
        }
    }


    /// <summary>
    /// Enables the element's ability to move.
    /// </summary>
    public void Activate()
    {
        moving = true;
        _activatedActions?.Invoke();
    }

    /// <summary>
    /// Deactivates the element by stopping the movement immediately.
    /// </summary>
    public void Deactivate()
    {
        moving = false;
        rb.linearVelocity = Vector2.zero;

         _deactivatedActions?.Invoke();
    }

    // TODO: These setters (excluding setTrack and setDir) are remnants of old code. For future developers, find a way
    // to avoid having to make new setters every time you want to add new behavior to the platform via
    // a virus effector. Also, get rid of the speed modifiers and just make one for modifying a platform's
    // speed directly.
    //
    // Cut out the middleman, please. He makes me sad.

    /// <summary>
    /// Sets the platform's track to the given track, starting at the point closest to the track's current position.
    /// If shouldRevert is true, ignores the given argument and reassigns the original path.
    /// If type is empty, skips the assignment.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="type"></param>
    /// <param name="shouldRevert"></param>


    /// <summary>
    /// Gets the direction of movement of the element.
    /// </summary>
    /// <returns></returns>
    public bool GetDir() => _isMovingRight;

}

