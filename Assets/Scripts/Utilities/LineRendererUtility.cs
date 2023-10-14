using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Utility class to help create meshes for line things quickly.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LineRendererUtility : MonoBehaviour
{
    [SerializeField, HideInInspector] private LineRenderer lineRenderer;

    [SerializeField] private Transform[] _requiredPoints;

    [Header("Generation")]
    [SerializeField, Range(0f, 10f)] private float _maxGeneratedSegmentLength = 2f;
    [SerializeField] private float _zPos = -6f;
    [SerializeField, Range(0.05f, 1f)] private float _thickness = 0.1f;

    [Space(10)]

    [Header("Randomness")]
    [SerializeField, Range(0f, 1f)] private float _lengthVariance = 0f;
    [SerializeField, Range(0f, 1f)] private float _randomVariance = 0.75f;
    [SerializeField, Range(0.25f, 2f)] private float _deltaMin = 0.75f;

    private int _size;
    private Vector3[] _points;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void InitFields()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        _size = lineRenderer.positionCount;
        _points = new Vector3[_size];

        lineRenderer.GetPositions(_points);
        lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0f, _thickness));
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
    /// Assigns the required points list to all the transforms of the children of this gameobject.
    /// </summary>
    public void StockPointsFromChildren()
    {
        InitFields();

        _requiredPoints = new Transform[transform.childCount];

        int i = 0;
        foreach (Transform child in transform)
        {
            _requiredPoints[i++] = child;
        }

        Debug.Log($"Stocked {i} required points from child transforms.");
    }

    /// <summary>
    /// Bakes all the positions in the linerenderer to child transforms.
    /// Destroys/Replaces pre-existing transforms, so don't forget to restock.
    /// </summary>
    public void BakeToTransforms()
    {
        InitFields();

        foreach (Transform t in transform)
        {
            DestroyImmediate(t.gameObject);
        }

        int pointIndex = 0;

        foreach (var point in _points)
        {
            Transform childTransform = new GameObject().transform;

            childTransform.name = $"Point {pointIndex++}";

            childTransform.transform.position = point;
            childTransform.transform.SetParent(gameObject.transform);
        }

        Debug.Log($"Baked {pointIndex} points into Transforms.");
    }

    /// <summary>
    /// Bakes the mesh into a power trail gameobject.
    /// </summary>
    /// <param name="name"></param>
    public void BakeMeshIntoGameObject(string name)
    {
        InitFields();

        PowerTrail.Setup(name, lineRenderer).transform
            .SetParent(gameObject.transform);

        Debug.Log($"Created {name} in root hierarchy.");
    }

    /// <summary>
    /// Given the required points, produces a linearized path through them.
    /// </summary>
    public void Generate()
    {
        InitFields();

        if (_requiredPoints.Length == 0)
        {
            Debug.LogError("You must have at least two points in the required point list!");
            return;
        }

        var building = new List<Vector3>
            {
                _requiredPoints[0].position
            };

        for (int i = 1; i < _requiredPoints.Length; i += 1)
        {
            var targetPoint = _requiredPoints[i].position;
            var currentPoint = _requiredPoints[i - 1].position;

            // compare every position with the next to see if it's worth making a segment in that cardinal direction.
            do
            {
                float dx = targetPoint.x - currentPoint.x;
                float dy = targetPoint.y - currentPoint.y;

                float adx = Mathf.Abs(dx);
                float ady = Mathf.Abs(dy);

                // max distance the segment can travel
                float maxDist = _maxGeneratedSegmentLength * (1f + Random.Range(-_lengthVariance, _lengthVariance));


                // determine the directional offset
                Vector3 offset;

                // if the absolute delta x is greater than the delta y, and the random variance doesn't act up, make a right-segment.
                if ((adx > ady) || (Random.Range(0f, 1f) > (1f - _randomVariance) && dy > _deltaMin && dx > _deltaMin))
                {
                    offset = Mathf.Round(dx / adx) * Mathf.Min(adx, maxDist) * Vector3.right;
                }
                else
                {
                    offset = Mathf.Round(dy / ady) * Mathf.Min(ady, maxDist) * Vector3.up;
                }

                // offset us
                currentPoint += offset;
                // then add us
                building.Add(currentPoint);

                // all the while making sure we haven't reach the destination yet.
            } while (Vector3.Distance(currentPoint, targetPoint) > 0.2f);
        }

        lineRenderer.positionCount = building.Count;
        _size = building.Count;
        lineRenderer.SetPositions(building.ToArray());

        Debug.Log($"Successfully generated a path using {building.Count} positions.");
    }

    /// <summary>
    /// Compares two floats by Mathf.Approximately if there is no length variance, but with more leniency
    /// if there is a length variance.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private bool Approx(float a, float b)
    {
        if (_lengthVariance == 0)
        {
            return Mathf.Approximately(a, b);
        }

        return Mathf.Abs(a - b) < _lengthVariance;
    }

    /// <summary>
    /// Attempts to combine similar segments.
    /// </summary>
    public void Reduce()
    {
        InitFields();

        if (_size < 3)
        {
            Debug.LogError("You cannot reduce with less than 3 points!");
            return;
        }

        var building = new List<Vector3>();

        // the logic here is to find left and right positions that, between them, contain meaningless segments.
        for (int i = 0; i < _size - 2; i += 1)
        {
            var left = _points[i]; // find our left bound

            building.Add(left); // add it

            int j;

            // now try to find a right bound
            for (j = i + 2; j < _size; j += 1)
            {
                var right = _points[j];

                // if the right is different from the left, add it and the point previous to it
                if (!Approx(left.x, right.x) && !Approx(left.y, right.y))
                {
                    building.Add(_points[j - 1]);
                    building.Add(right);

                    break;
                }
            }

            i = j;
        }

        if (building.Count == _size)
        {
            Debug.LogWarning("Failed to reduce number of points.");
            return;
        }

        int oldSize = _size;

        lineRenderer.positionCount = building.Count;
        _size = building.Count;
        lineRenderer.SetPositions(building.ToArray());

        Debug.Log($"Reduced {oldSize} points to {_size} points.");
    }

    /// <summary>
    /// Snaps segments to be within purely cardinal directions of each other.
    /// </summary>
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

        Debug.Log("Linearized points. Don't forget to Reduce to remove redundancies.");
    }

    /// <summary>
    /// Given two vectors, snaps them to a similar cardinal direction.
    /// </summary>
    /// <param name="posS"></param>
    /// <param name="posD"></param>
    /// <returns></returns>
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
