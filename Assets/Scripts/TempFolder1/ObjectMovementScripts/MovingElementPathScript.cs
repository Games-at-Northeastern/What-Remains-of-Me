using System.Collections.Generic;
using Levels.Objects.Platform;
using UnityEngine;
using UnityEngine.Splines;

// Requires a Spline Container as it sets the path the platform will move on
[RequireComponent(typeof(SplineContainer))]
public class MovingObjectPathScript : MonoBehaviour
{
    [Space]

    // Platform object. While not stated, it does require a rigidbody2D
    [SerializeField]
    List<MovingObjectScript> movingObjects;

    [SerializeField]
    bool activeByDefault = true; // Sets if the platform is active on start

    [Header("Platform Speed")] [SerializeField]
    float maxSpeed = 1f; // Maximum platform speed

    [SerializeField]
    bool useAcceleration; // If the platform uses acceleration or starts at max speed

    // Handles Acceleration. Can toggle extra variables by clicking on useAcceleration
    [ShowIf(nameof(useAcceleration), true)] [SerializeField]
    float acceleration = 0.25f; // The platform's acceleration

    [Header("Loop Type")] [SerializeField]
    LoopType loopType; // The loop pathing type

    [ShowIf(nameof(loopType), LoopType.OneWay)] [SerializeField]
    bool keepSpeedAfterLoop;

    // LoopType OneWay Specific Variables
    [ShowIf(nameof(loopType), LoopType.OneWay)] [SerializeField]
    float gravity;

    float splineLength; // Length of the current spline

    SplineContainer splinePath;

    // Sets up all the starting values for this script to execute
    void Awake()
    {
        splinePath = GetComponent<SplineContainer>();
        splineLength = splinePath.Splines[0].GetLength();
        EnsureValidSpline();
    }

    void Start()
    {
        // Calculates the length of the spline aka the platform's path
        splineLength = splinePath.CalculateLength();

        // Activates or deactivates that platform depending on activeByDefault
        if (activeByDefault) {
            Activate();
        } else {
            Deactivate();
        }
    }

    // Move the platform each FixedUpdate
    protected void FixedUpdate()
    {
        MoveObjects();
    }

    // Runs after any change in the inspector and the game is not running
    void OnValidate()
    {
        if (!Application.isPlaying) {
            // Updates the platform position depending on the platformStartLocation's variable
            splinePath = GetComponent<SplineContainer>();

            // Depending on the loop type, this sets if the spline should have a closed loop type or not
            splinePath.Splines[0].Closed = ShouldCloseSplineLoop(loopType);

            foreach (MovingObjectScript movingObject in movingObjects) {
                movingObject.SetSplinePath(splinePath);
            }
        }
    }

    // Given a loop type, returns if that type should have a closed spline knot
    static bool ShouldCloseSplineLoop(LoopType lType)
    {
        switch (lType) {
            case LoopType.Wrap:
                return true;
            default:
                return false;
        }
    }

    // Given the current loop type, this tells the user that the spline knot is incorrectly closed/open
    void EnsureValidSpline()
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

    // Main function that moves the platform
    void MoveObjects()
    {
        if (movingObjects.Count == 0) {
            return;
        }

        foreach (MovingObjectScript movingObject in movingObjects.ToArray()) {
            movingObject.Move(this);
        }
    }

    public void Activate()
    {
        if (movingObjects.Count == 0) {
            return;
        }

        for (int i = movingObjects.Count - 1; i >= 0; --i) {
            movingObjects[i].Activate(this);
        }
    }

    public void Deactivate()
    {
        foreach (MovingObjectScript movingObject in movingObjects.ToArray()) {
            movingObject.Deactivate();
        }
    }

    public void RemoveMovingObject(MovingObjectScript movingObject)
    {
        if (movingObjects.Contains(movingObject)) {
            movingObjects.Remove(movingObject);
        }
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetAcceleration()
    {
        return acceleration;
    }

    public bool GetUseAcceleration()
    {
        return useAcceleration;
    }

    public LoopType GetLoopType()
    {
        return loopType;
    }

    public bool GetKeepSpeedAfterLoop()
    {
        return keepSpeedAfterLoop;
    }

    public float GetGravityScale()
    {
        return gravity;
    }

    public SplineContainer GetSplinePath()
    {
        return splinePath;
    }

    public float GetSplineLength()
    {
        return splineLength;
    }
}
