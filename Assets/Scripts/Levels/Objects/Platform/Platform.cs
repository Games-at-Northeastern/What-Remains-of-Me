namespace Levels.Objects.Platform
{
    using System;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Moves this platform between the given points sequentially and in
    /// straight lines. After the last point has been reached, goes back
    /// to the first point in the given array.
    /// </summary>
    public class Platform : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Transform[] _points;

        [SerializeField] private float randomSpeedModifier = 0;
        [SerializeField] private float maxSpeedModifier = 3;


        [SerializeField] private float speedModifier = 1f;

        private int _currPointIndex;
        private bool _shouldMove;


        Rigidbody2D rb;
        Collider2D collider;
        Vector3 moveDirection;

        private MovementExecuter movementExecuter;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            collider = GetComponent<Collider2D>();
            movementExecuter = GameObject.FindGameObjectsWithTag("Player")[0].GetComponentInChildren<MovementExecuter>();
        }

        private void Start()
        {
            transform.position = _points[0].position;
            DirectionCalculate();
        }

        private void FixedUpdate()
        {
            if (_shouldMove)
            {
                rb.velocity = moveDirection * speedModifier * UnityEngine.Random.Range(_speed - randomSpeedModifier, _speed + randomSpeedModifier);
            }

        }

        private void DirectionCalculate() => moveDirection = (_points[_currPointIndex].position - transform.position).normalized;

        public void Activate()
        {
            _shouldMove = true;
            // Upon activation, check that the direction it should be moving in is correct. Adjust if not.
            if (isPlatformBeyondTarget())
            {
                SetNextTargetPoint();
            }
            DirectionCalculate();
        }

        /// <summary>
        /// Deactivates the platform by stopping the movement immediately.
        /// </summary>
        public void Deactivate()
        {
            _shouldMove = false;
            rb.velocity = Vector2.zero;
        }

        public void SetRandomSpeedModifier(float newModifier) => randomSpeedModifier = Math.Min(newModifier, maxSpeedModifier);

        public void SetSpeedModifier(float newModifier)
        {
            speedModifier = newModifier;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("collision  " + collision);

            if (collision.gameObject.CompareTag("Player"))
            {
                movementExecuter.isOnAPlatform = true; // TODO : this should NOT have an explicit reference to the player's movement executor...
                movementExecuter.platformRb = rb;
            } else if (collision.gameObject.CompareTag("platformPoint"))
            {
                SetNextTargetPoint();
                DirectionCalculate();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("platformPoint"))
            {
                SetNextTargetPoint();
                DirectionCalculate();
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                movementExecuter.isOnAPlatform = false;
            }
        }

        private void SetNextTargetPoint()
        {
            _currPointIndex++;

            if (_currPointIndex == _points.Length)
            {
                _currPointIndex = 0;
            }
        }

        private bool isPlatformBeyondTarget() => Vector2.Dot(moveDirection, _points[_currPointIndex].position - transform.position) <= 0;
    }
}
