using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Levels.Objects
{
    public class PowerTrailRenderer : MonoBehaviour
    {
        [SerializeField, HideInInspector] private LineRenderer lineRenderer;
        [SerializeField] private float _zPos = -6f;

        [SerializeField] private Transform[] _requiredPoints;
        [SerializeField] private float _maxGeneratedSegmentLength = 5f;
        [SerializeField, Range(0f, 1f)] private float _randomVariance = 0.75f;
        [SerializeField, Range(0.25f, 2f)] private float _deltaMin = 0.75f;

        private int _size;
        private Vector3[] _points;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

#if UNITY_EDITOR
        private void InitFields()
        {
            if (!lineRenderer)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }

            _size = lineRenderer.positionCount;
            _points = new Vector3[_size];

            lineRenderer.GetPositions(_points);
        }

        /// <summary>
        /// Resets the number of points on this renderer to 0.
        /// </summary>
        public void ResetPoints()
        {
            InitFields();

            lineRenderer.positionCount = 0;
        }

        /// <summary>
        /// Given the required points, produces a linearized path through them.
        /// </summary>
        public void Generate()
        {
            var building = new List<Vector3>
            {
                _requiredPoints[0].position
            };

            for (int i = 1; i < _requiredPoints.Length; i += 1)
            {
                var targetPoint = _requiredPoints[i].position;
                var currentPoint = _requiredPoints[i - 1].position;

                do
                {
                    float dx = targetPoint.x - currentPoint.x;
                    float dy = targetPoint.y - currentPoint.y;

                    float adx = Mathf.Abs(dx);
                    float ady = Mathf.Abs(dy);


                    Vector3 offset;

                    if ((adx > ady) || (Random.Range(0f, 1f) > (1f - _randomVariance) && dy > _deltaMin && dx > _deltaMin))
                    {
                        offset = Mathf.Round(dx / adx) * Mathf.Min(adx, _maxGeneratedSegmentLength) * Vector3.right;
                    }
                    else
                    {
                        offset = Mathf.Round(dy / ady) * Mathf.Min(ady, _maxGeneratedSegmentLength) * Vector3.up;
                    }

                    currentPoint += offset;

                    building.Add(currentPoint);
                } while (Vector3.Distance(currentPoint, targetPoint) > 0.2f);
            }

            lineRenderer.positionCount = building.Count;
            lineRenderer.SetPositions(building.ToArray());
        }

        public void Reduce()
        {
            List<Vector3> building = new List<Vector3>();

            // TODO
        }

        public void LinearizePoints()
        {
            InitFields();

            var newPoints = new Vector3[_size];

            newPoints[0] = _points[0];
            newPoints[0].z = _zPos;

            for (int i = 1; i < _size; i += 1)
            {
                newPoints[i] = Snap2D(newPoints[i - 1], _points[i]);
            }

            lineRenderer.SetPositions(newPoints);
        }

        private Vector3 Snap2D(Vector3 posS, Vector3 posD)
        {
            float dx = Mathf.Abs(posD.x - posS.x);
            float dy = Mathf.Abs(posD.y - posS.y);

            var snap = Vector3.forward * _zPos;

            if (dx > dy)
            {
                snap.x = posD.x;
                snap.y = posS.y;
            }
            else
            {
                snap.x = posS.x;
                snap.y = posD.y;
            }

            return snap;
        }
    }
#endif
}
