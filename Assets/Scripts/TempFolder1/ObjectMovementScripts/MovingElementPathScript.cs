using System.Collections.Generic;
using Levels.Objects.Platform;
using UnityEngine;
using UnityEngine.Splines;

// Requires a Spline Container as it sets the path the platform will move on
[RequireComponent(typeof(SplineContainer))]

// Class that handles all the moving elements on a spline path
public class MovingElementPathScript : MonoBehaviour
{
    [SerializeField] private bool activeByDefault = true; // Sets if the platform is active on start

    [Header("Platform Movement")]
    [SerializeField] private float maxSpeed = 1f; // Maximum platform speed

    [SerializeField] private bool useAcceleration; // Sets the platform uses acceleration or stays at max speed

    // Handles Acceleration. Can toggle extra variables by clicking on useAcceleration
    [ShowIf(nameof(useAcceleration), true)] [SerializeField]
    private float acceleration = 0.25f; // The platform's acceleration

    // If the loop type is LoopType.Pingpong, show this variable
    // Resets the platform's speed once it reaches the end of its path
    [ShowIf(nameof(pingPongAccelRequirements), true)]
    [SerializeField] private bool resetSpeedAtPathEnds;

    [Header("Loop Type")]
    [SerializeField] private LoopType loopType; // The loop pathing type

    // LoopType.OneWay Specific Variables

    // Make the platform keep its current speed after detaching from the path
    [ShowIf(nameof(loopType), LoopType.OneWay)]
    [SerializeField] private bool keepSpeedAfterLoop;

    // The gravity scale of the platform
    [ShowIf(nameof(loopType), LoopType.OneWay)]
    [SerializeField] private float gravityScale;

    [Space]

    // List of moving elements on the path
    private List<MovingElementScript> movingObjects;

    private float splineLength; // Length of the current spline

    private SplineContainer splinePath; // Spline path the elements will be moving on

    // Represents the requirements for the resetSpeedAtPathEnds variable to show in the inspector
    private bool pingPongAccelRequirements {
        get {
            return loopType == LoopType.Pingpong && useAcceleration;
        }
    }

    // Sets up all the starting values for this script to execute
    private void Awake()
    {
        splinePath = GetComponent<SplineContainer>();
        movingObjects = new List<MovingElementScript>();

        // NOTE: Only the first Spline in the container matters.
        // You can have infinite Knots, but only the first Spline is used as the path
        splineLength = splinePath.Splines[0].GetLength();
        EnsureValidSpline();
    }

    private void Start()
    {
        // Calculates the length of the path
        splineLength = splinePath.CalculateLength();

        // Activates or deactivates the platform depending on activeByDefault
        if (activeByDefault) {
            Activate();
        } else {
            Deactivate();
        }
    }

    // Move the platform each FixedUpdate
    protected void FixedUpdate() => MoveObjects();

    // Runs after any change in the inspector during Edit Mode
    private void OnValidate()
    {
        splinePath = GetComponent<SplineContainer>();

        // Depending on the loop type, this sets if the spline should have a closed loop or not
        splinePath.Splines[0].Closed = ShouldCloseSplineLoop(loopType);

    }

    // Given a loop type, returns if that type should have a closed spline knot
    private static bool ShouldCloseSplineLoop(LoopType lType)
    {
        switch (lType) {
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
        if (splinePath.Splines[0].Closed != shouldBeClosed) {
            if (shouldBeClosed) {
                Debug.LogError("Loop Type \"" + loopType + "\" Only Works With A Closed Spline");
            } else {
                Debug.LogError("Loop Type \"" + loopType + "\" Doesn't Work With A Closed Spline");
            }
        }
    }

    // Main function that moves each element subscribed to the path
    private void MoveObjects()
    {
        if (movingObjects.Count == 0) {
            return;
        }

        // Moves each element
        foreach (MovingElementScript movingObject in movingObjects.ToArray()) {
            movingObject.Move(this);
        }
    }

    // When this path is activated, run the activate functions of each moving object attached
    public void Activate()
    {
        foreach (MovingElementScript movingObject in movingObjects.ToArray()) {
            movingObject.Activate(this);
        }
    }

    // When this path is deactivated, run the deactivate functions of each moving object attached
    public void Deactivate()
    {
        foreach (MovingElementScript movingObject in movingObjects.ToArray()) {
            movingObject.Deactivate();
        }
    }

    // Subscribes a moving element to the movingObjects list, assuming they aren't already
    public SplineContainer addMovingObject(MovingElementScript movingElement)
    {
        if (movingObjects.Contains(movingElement)) {
            return splinePath;
        }

        movingObjects.Add(movingElement);
        return splinePath;
    }

    // Unsubscribes a moving element to the movingObjects list
    public void RemoveMovingObject(MovingElementScript movingElement)
    {
        if (movingObjects.Contains(movingElement)) {
            movingObjects.Remove(movingElement);
        }
    }

    // Getter Functions

    public float GetMaxSpeed() => maxSpeed;

    public float GetAcceleration() => acceleration;

    public bool GetUseAcceleration() => useAcceleration;

    public LoopType GetLoopType() => loopType;

    public bool GetKeepSpeedAfterLoop() => keepSpeedAfterLoop;

    public float GetGravityScale() => gravityScale;

    public SplineContainer GetSplinePath() => splinePath;

    public bool ResetSpeedAtPathEnds() => resetSpeedAtPathEnds;

    public float GetSplineLength() => splineLength;
}
