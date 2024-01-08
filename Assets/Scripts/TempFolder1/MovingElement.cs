using Levels.Objects.Platform;
using UnityEngine;

/// <summary>
/// A custom component that moves the gameobject it's on along a given path.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MovingElement : MonoBehaviour
{
    private Rigidbody2D rb;

    private int _destinationIndex;
    private bool _shouldMove;
    private bool _completed = false;
    private float _previousDistance;

    private Vector3 _moveDirection;
    private Transform[] _runtimePoints;
    private LoopType _runtimeLoopType;

    [SerializeField] private Transform[] _points;
    [SerializeField, Tooltip("If the movement of this body relies on the movement of others (by parenting, by relying on moving points, etc), reference those bodies here.")]
    protected Rigidbody2D[] _relativeBodies;

    [Space(15)]

    [SerializeField] private bool _activeByDefault;
    [SerializeField] private bool _rotateToDirection = false;
    [SerializeField, Range(0.0005f, 0.5f), Tooltip("How close do we have to get before moving to next point? Be careful with this value's magnitude. Around 0.05 is fine.")]
    private float _locationAccuracy = 0.05f;

    [Header("Initial Position")]
    [SerializeField, Tooltip("What point index do we start at (0-indexed)? Is Clamped upon initialization.")]
    private int _initialStartIndex = 0;
    [SerializeField, Range(0f, 1f), Tooltip("How far along are we to the next point in the path upon initialization?")]
    private float _normalizedInitialStartPosition = 0f;

    [Header("Speed")]
    [SerializeField] protected float _speed = 1f;
    [SerializeField] protected float _randomSpeedModifier = 0f;
    [SerializeField] protected float _maxSpeedModifier = 3;
    [SerializeField] protected float _speedModifier = 1f;

    [Header("Loop Info")]
    [SerializeField] private LoopType _loopType = LoopType.Wrap;
    [SerializeField] private bool _isMovingRight = true;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void Start()
    {
        _runtimePoints = _points;
        _runtimeLoopType = _loopType;

        if (_runtimePoints.Length == 0)
        {
            Debug.LogError("Runtime Points is empty!");
        }

        Init();

        if (_activeByDefault)
        {
            Activate();
        }
    }

    // TODO There are a lot of leftover debug logs bc i think there's still a bug here. It just kinda went away
    // last time so I don't know if it's fixed or just hiding. Essentially, the moving element would skip a point
    // sometimes when affected by a HecticMovementEffector. I let atlas swing from a super-fast, rapidly-changing moving
    // outlet for like 3 minutes and saw nothing, so im assuming the bug just vanished. Definitely wrong, but whatever.

    /// <summary>
    /// Sets up an element to start at the initial point going to the next.
    /// </summary>
    public void Init()
    {
        _completed = false;

        _initialStartIndex = Mathf.Clamp(_initialStartIndex, 0, _runtimePoints.Length - 1);

        // initial placement calculation. NOTE: THIS MIGHT LEAD TO SOME WARPING IF CALLED AGAIN. Abstract it?
        _destinationIndex = GetNextPointIndex(_initialStartIndex); // get next point

        var pos1 = _runtimePoints[_initialStartIndex].position; // get our initial position
        var pos2 = _runtimePoints[_destinationIndex].position; // get out next position
        transform.position = ((pos2 - pos1) * _normalizedInitialStartPosition) + pos1; // move towards next depending on _nISP
                                                                                       //

        _moveDirection = GetDirectionToPoint(_destinationIndex);
        _previousDistance = float.MaxValue; // if not maxed out, the first position might be skipped on lower-end hardware.
    }

    protected void FixedUpdate()
    {
        MovePlatform();
    }

    protected Vector2 MovePlatform()
    {
        if (_shouldMove) // if actionable...
        {
            float dist = GetDistanceToPoint(_destinationIndex);
            //Debug.Log((dist > _previousDistance ? "Greater! " : "") + $"Distance: {dist}, Previous Distance: {_previousDistance}");

            // both clauses are needed here. The first catches *just before* we hit the point, and the second
            // catches *just after* we pass the point, in case a fixed-update loop doesn't manage to catch the first in time.
            // The 2nd case is theoretically never needed as physics always occurs in fixed-update, but if the accuracy is small
            // enough, the distance might never be smaller than it.
            if (dist <= _locationAccuracy || dist > _previousDistance) // if we are close enough to point OR if we've passed the target point...
            {
                //Debug.Log("Fired for the following reason: " + (dist > _previousDistance ? $"distance ({dist}) is greater than previous ({_previousDistance})" : "within accuracy"));
                if (_runtimePoints.Length == 1) {
                    transform.position = _runtimePoints[0].position;
                }
                UpdateDestinationAndDirection();
            }

            if (!_completed) // if the track isn't already completed...
            {
                _previousDistance = GetDistanceToPoint(_destinationIndex);

                rb.velocity = _speed * (_speedModifier + _randomSpeedModifier) * _moveDirection;

                // add tracking speeds from other related bodies
                foreach (Rigidbody2D body in _relativeBodies)
                {
                    rb.velocity += body.velocity;
                }
            }
            else
            {
                Deactivate();
            }
        }

        // heavy-handed solution to elements flying off into nowhere.
        // not even sure it works, but eh. Good code is for chumperinos.
        else if (rb.velocity != Vector2.zero)
        {
            rb.velocity = Vector2.zero;
        }
        return rb.velocity;
    }

    /// <summary>
    /// Update the path of the moving element by changing the direction and destination index.
    /// </summary>
    private void UpdateDestinationAndDirection()
    {
        // this needs to be maximized bc if it isn't, you might skip a point when manually invoking this method.
        _previousDistance = float.MaxValue;

        // go to next
        // even with (what i think is) logically sound index-logic, draining virus/energy and re-adding it sometimes
        // has a chance to OoB. So, I'm clamping. Ah, well.
        int nextDest = Mathf.Clamp(GetNextPointIndex(_destinationIndex), 0, _runtimePoints.Length - 1);

        // get the direction to the next point
        _moveDirection = GetDirectionToPoint(nextDest);

        //Debug.Log("Moving to the " + (_isMovingRight ? "right" : "left") + $" from {_destinationIndex} to {nextDest}");

        _destinationIndex = nextDest; // nextDest can be removed, but this makes it more readable.
    }

    /// <summary>
    /// Returns the next point index depending on the looptype. Calculates using
    /// the passed index (usually _desinationIndex). Be sure to update it!
    /// </summary>
    /// <returns></returns>
    private int GetNextPointIndex(int index) => LoopTypeUtility.GetNextIndex(index, _runtimePoints.Length, ref _isMovingRight, ref _completed, _runtimeLoopType);

    /// <summary>
    /// Computes a normalized direction vector pointing towards the given point in the move sequence.
    /// </summary>
    /// <returns>A normalized Vector3.</returns>
    private Vector3 GetDirectionToPoint(int pointIndex)
    {
        var dir = (_runtimePoints[pointIndex].position - transform.position).normalized;

        if (_rotateToDirection)
        {
            transform.forward = dir;
        }

        return dir;
    }

    /// <summary>
    /// Returns the distance to the given point.
    /// </summary>
    /// <returns>The distance to the given point.</returns>
    private float GetDistanceToPoint(int pointIndex) => Vector2.Distance(transform.position, _runtimePoints[pointIndex].position);

    /// <summary>
    /// Enables the element's ability to move.
    /// </summary>
    public void Activate() => _shouldMove = true;

    /// <summary>
    /// Deactivates the element by stopping the movement immediately.
    /// </summary>
    public void Deactivate()
    {
        _shouldMove = false;
        rb.velocity = Vector2.zero;
    }

    // TODO: These setters (excluding setTrack and setDir) are remnants of old code. For future developers, find a way
    // to avoid having to make new setters every time you want to add new behavior to the platform via
    // a virus effector. Also, get rid of the speed modifiers and just make one for modifying a platform's
    // speed directly.
    //
    // Cut out the middleman, please. He makes me sad.

    /// <summary>
    /// Sets the platform's random speed modifier to the given modifier, capped out by the platform's max speed modifier.
    /// DOES NOT TAKE INTO ACCOUNT CURRENT SPEED MODIFIERS.
    /// </summary>
    /// <param name="newModifier"></param>
    public void SetRandomSpeedModifier(float newModifier) => _randomSpeedModifier = Mathf.Min(newModifier, _maxSpeedModifier);

    /// <summary>
    /// Sets the platform's speed modifier to the given modifier, capped out by the platform's max speed modifier.
    /// DOES NOT TAKE INTO ACCOUNT CURRENT RANDOM SPEED MODIFIERS.
    /// </summary>
    /// <param name="newModifier"></param>
    public void SetSpeedModifier(float newModifier) => _speedModifier = Mathf.Min(newModifier, _maxSpeedModifier);

    /// <summary>
    /// Sets the platform's track to the given track, starting at the point closest to the track's current position.
    /// If shouldRevert is true, ignores the given argument and reassigns the original path.
    /// If type is empty, skips the assignment.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="type"></param>
    /// <param name="shouldRevert"></param>
    public void SetTrack(Transform[] points, LoopType type = LoopType.None, bool shouldRevert = false)
    {
        bool isRedundant;

        if (shouldRevert)
        {
            isRedundant = _runtimePoints == _points;

            _runtimePoints = _points;
            _runtimeLoopType = _loopType;
        }
        else
        {
            isRedundant = _runtimePoints == points;

            _runtimePoints = points;
            _runtimeLoopType = type == LoopType.None ? _loopType : type;
        }

        if (isRedundant)
        {
            return;
        }

        // find closest point in new track
        float minDistance = float.MaxValue;
        for (int i = 0; i < _runtimePoints.Length; i += 1)
        {
            if (GetDistanceToPoint(i) < minDistance)
            {
                _destinationIndex = i; // set our new destination to that
            }
        }

        // get going
        _moveDirection = GetDirectionToPoint(_destinationIndex);
    }

    /// <summary>
    /// Method to permanetely assign a track.
    /// </summary>
    /// <param name="track"></param>
    public void SetPermanentTrack(Transform[] track) =>_points = track;

    /// <summary>
    /// Sets the direction of movement of the element.
    /// </summary>
    /// <param name="isMovingRight"></param>
    public void SetDir(bool isMovingRight)
    {
        if (_isMovingRight == isMovingRight)
        {
            return;
        }

        _isMovingRight = isMovingRight;

        UpdateDestinationAndDirection(); // we need to go in the opposite direction, so force an update
    }

    /// <summary>
    /// Gets the direction of movement of the element.
    /// </summary>
    /// <returns></returns>
    public bool GetDir() => _isMovingRight;

    /// <summary>
    /// Flips the direction of movement.
    /// </summary>
    public void ToggleDir()
    {
        //Debug.Log("Toggled direction");
        SetDir(!_isMovingRight);
    }

}
