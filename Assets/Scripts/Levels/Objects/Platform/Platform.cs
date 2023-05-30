namespace Levels.Objects.Platform
{
    using System;
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

        private int _currPointIndex;
        private bool _shouldMove;


        Rigidbody2D rb;
        Vector3 moveDirection;

        private MovementExecuter movementExecuter;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            movementExecuter = GameObject.FindGameObjectsWithTag("Player")[0].GetComponentInChildren<MovementExecuter>();
        }

        private void Start()
        {
            transform.position = _points[0].position;
            moveDirection = new Vector3().normalized;
        }

        private void Update()
        {
            if (!_shouldMove)
            {
                return;
            }

            if (Vector2.Distance(transform.position, _points[_currPointIndex].position) < 0.02f)
            {
                _currPointIndex++;

                if (_currPointIndex == _points.Length)
                {
                    _currPointIndex = 0;
                }
                DirectionCalculate();
            }

            //transform.position = Vector2.MoveTowards(transform.position,
            //    _points[_currPointIndex].position,
            //    _speed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (_shouldMove)
            {
                rb.velocity = moveDirection * _speed;
            }

        }

        private void DirectionCalculate()
        {
            moveDirection = (_points[_currPointIndex].position - transform.position).normalized;
        }

        public void Activate()
        {
            _shouldMove = true;
        }

        /// <summary>
        /// Deactivates the platform by stopping the movement immediately.
        /// </summary>
        public void Deactivate()
        {
            _shouldMove = false;
            rb.velocity = Vector2.zero;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                movementExecuter.isOnAPlatform = true; // TODO : this should NOT have an explicit reference to the player's movement executor...
                movementExecuter.platformRb = rb;
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
