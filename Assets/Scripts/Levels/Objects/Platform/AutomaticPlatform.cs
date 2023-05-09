namespace Levels.Objects.Platform
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Moves this platform between the given points sequentially and in
    /// straight lines. After the last point has been reached, goes back
    /// to the first point in the given array.
    /// </summary>
    public class AutomaticPlatform : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Transform[] _points;

        private int _currPointIndex;
        private bool _shouldMove = true;

        private void Start()
        {
            transform.position = _points[0].position;
        }

        private void Update()
        {

            // if (Vector2.Distance(transform.position, _points[_currPointIndex].position) < 0.02f)
            // {
            //     _currPointIndex++;

            //     if (_currPointIndex == _points.Length)
            //     {
            //         _currPointIndex = 0;
            //     }
            // }

            transform.position = Vector2.MoveTowards(transform.position,
                _points[_currPointIndex].position,
                _speed * Time.deltaTime);
        }

        public void Activate()
        {
            
        }

        public void Deactivate()
        {
            
        }

        // if the player touches the platform, the platform moves towards Point B
        private void OnCollisionEnter2D(Collision2D collision)
        {
            collision.transform.SetParent(transform);
            _currPointIndex = 1;
        }

        // if the player stops touching the platform, the platform moves towards Point A
        private void OnCollisionExit2D(Collision2D collision)
        {
            collision.transform.SetParent(null);
            _currPointIndex = 0;
        }
    }
}
