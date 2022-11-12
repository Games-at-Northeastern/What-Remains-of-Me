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

        private void Start()
        {
            transform.position = _points[0].position;
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
            }

            transform.position = Vector2.MoveTowards(transform.position,
                _points[_currPointIndex].position,
                _speed * Time.deltaTime);
        }

        public void Activate()
        {
            _shouldMove = true;
        }

        public void Deactivate()
        {
            _shouldMove = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            collision.transform.SetParent(transform);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            collision.transform.SetParent(null);
        }
    }
}
