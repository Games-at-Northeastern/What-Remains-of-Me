namespace Levels.Objects.Platform
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Moves the gameobject along a series of points, in an ordering defined by the platform's loop type.
    /// </summary>
    public class Platform : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Transform[] _points;

        [Space(10)]

        [Header("Loop Info")]
        [SerializeField] private PlatformLoopType _loopType = PlatformLoopType.Wrap;
        [SerializeField] private bool _isMovingRight = true;

        [Space(10)]

        [Header("Speed")]
        [SerializeField] private float _randomSpeedModifier = 0f;
        [SerializeField] private float _maxSpeedModifier = 3;
        [SerializeField] private float _speedModifier = 1f;

        [Space(20)]

        [SerializeField] private string _eventCollisionTag; // tag for object we collide with to trigger its event

        private int _destinationIndex;
        private bool _shouldMove;
        private bool _completed = false; // has the platform finished? Only used by OneWay and None.
        private float _previousDistance;

        private Vector3 _moveDirection;
        private Transform[] _runtimePoints;
        private PlatformLoopType _runtimeLoopType;

        private Rigidbody2D rb;
        private MovementExecuter movementExecuter;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            movementExecuter = GameObject.FindGameObjectsWithTag("Player")[0].GetComponentInChildren<MovementExecuter>();
        }

        private void Start()
        {
            _runtimePoints = _points;
            _runtimeLoopType = _loopType;
            InitPlatform();
        }

        /// <summary>
        /// Sets up a platform to start at the initial point going to the next.
        /// </summary>
        public void InitPlatform()
        {
            _completed = false;

            _destinationIndex = 0;
            _destinationIndex = GetNextPointIndex();

            transform.position = _runtimePoints[0].position;
            _moveDirection = GetDirectionToPoint(_destinationIndex);
            _previousDistance = int.MaxValue; // if not maxed out, the first position will be skipped.
        }

        private void FixedUpdate()
        {
            if (_shouldMove) // if actionable...
            {
                if (_previousDistance - GetDistanceToPoint(_destinationIndex) <= 0) // if we've passed the target point...
                {
                    // go to next
                    // even with (what i think is) logically sound index-logic, draining virus/energy and re-adding it sometimes
                    // has a chance to OoB. So, I'm clamping. Ah, well.
                    int nextDest = Mathf.Clamp(GetNextPointIndex(), 0, _runtimePoints.Length - 1);

                    // get the direction to the next point
                    _moveDirection = GetDirectionToPoint(nextDest);

                    _destinationIndex = nextDest; // nextDest can be removed, but this makes it more readable.
                }

                if (!_completed) // if the track isn't already completed...
                {
                    _previousDistance = GetDistanceToPoint(_destinationIndex);

                    rb.velocity = _speed * (_speedModifier + _randomSpeedModifier) * _moveDirection;
                }
                else
                {
                    Deactivate();
                }
            }

            // heavy-handed solution to platforms flying off into nowhere.
            // not even sure it works, but eh. Good code is for chumperinos.
            else if (rb.velocity != Vector2.zero)
            {
                rb.velocity = Vector2.zero;
            }
        }

        /// <summary>
        /// Returns the next point index depending on the looptype. Calculates using
        /// the current destination index. Be sure to update it!
        /// </summary>
        /// <returns></returns>
        private int GetNextPointIndex()
        {
            int dir = _isMovingRight ? 1 : -1;

            switch (_runtimeLoopType) // could replace this with an enhanced switch, but nah.
            {
                case PlatformLoopType.Wrap: // wraps around if out of bounds
                    return (_destinationIndex + dir) % _runtimePoints.Length;

                case PlatformLoopType.Pingpong: // bounces back if out of bounds
                    int next = _destinationIndex + dir;

                    if (next < 0) // hit left side
                    {
                        _isMovingRight = true;
                        return _destinationIndex + 1;
                    }
                    else if (next >= _runtimePoints.Length) // hit right side
                    {
                        _isMovingRight = false;
                        return _destinationIndex - 1;
                    }

                    return next;

                case PlatformLoopType.OneWay: // does not move after hitting opposite side
                    int nextIndex = _destinationIndex + dir;

                    if (nextIndex < 0 || nextIndex >= _runtimePoints.Length)
                    {
                        _completed = true;
                        return _destinationIndex;
                    }

                    return nextIndex;

                default: // None
                    _completed = true;
                    return _destinationIndex;
            }
        }

        /// <summary>
        /// Computes a normalized direction vector pointing towards the given point in the move sequence.
        /// </summary>
        /// <returns>A normalized Vector3.</returns>
        private Vector3 GetDirectionToPoint(int pointIndex) => (_runtimePoints[pointIndex].position - transform.position).normalized;

        /// <summary>
        /// Returns the distance to the given point.
        /// </summary>
        /// <returns>The distance to the given point.</returns>
        private float GetDistanceToPoint(int pointIndex) => Vector2.Distance(transform.position, _runtimePoints[pointIndex].position);

        /// <summary>
        /// Enables the platform's ability to move.
        /// </summary>
        public void Activate() => _shouldMove = true;

        /// <summary>
        /// Deactivates the platform by stopping the movement immediately.
        /// </summary>
        public void Deactivate()
        {
            _shouldMove = false;
            rb.velocity = Vector2.zero;
        }

        /// <summary>
        /// Sets the platform's random speed modifier to the given modifier, capped out by the platform's max speed modifier.
        /// DOES NOT TAKE INTO ACCOUNT CURRENT SPEED MODIFIERS.
        /// </summary>
        /// <param name="newModifier"></param>
        public void SetRandomSpeedModifier(float newModifier) => _randomSpeedModifier = Math.Min(newModifier, _maxSpeedModifier);

        /// <summary>
        /// Sets the platform's speed modifier to the given modifier, capped out by the platform's max speed modifier.
        /// DOES NOT TAKE INTO ACCOUNT CURRENT RANDOM SPEED MODIFIERS.
        /// </summary>
        /// <param name="newModifier"></param>
        public void SetSpeedModifier(float newModifier) => _speedModifier = Math.Min(newModifier, _maxSpeedModifier);

        /// <summary>
        /// Sets the platform's track to the given track, starting at the point closest to the track's current position.
        /// If shouldRevert is true, ignores the given argument and reassigns the original path.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="shouldRevert"></param>
        public void SetTrack(Transform[] points, PlatformLoopType type, bool shouldRevert = false)
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
                _runtimeLoopType = type;
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                /*
                 * // TODO : this should NOT have an explicit reference to the player's movement executor...
                 * 
                 * Meh. Probably.
                */

                movementExecuter.isOnAPlatform = true;
                movementExecuter.platformRb = rb;
            }
            else if (collision.gameObject.CompareTag(_eventCollisionTag))
            {
                var col = collision.gameObject.GetComponent<IOnCollision>();

                if (col != null)
                {
                    col.Collide((collision.transform.position - transform.position).normalized);
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                movementExecuter.isOnAPlatform = false;
            }
        }
    }
}
