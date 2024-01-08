using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Utility class to help create meshes for line things quickly.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LineRendererUtility : MonoBehaviour
{
    [SerializeField, HideInInspector] private LineRenderer lineRenderer;

    [SerializeField] private GameObject _bakedObjectPrefab;
    [SerializeField] private Transform[] _requiredPoints;

    [Header("Generation")]
    [SerializeField] private bool _cardinalOnly = true;
    [SerializeField, Range(0.1f, 10f)] private float _maxGeneratedSegmentLength = 2f;
    [SerializeField] private float _zPos = -6f;
    [SerializeField, Range(0.05f, 1f)] private float _thickness = 0.1f;

    [Space(10)]

    [Header("Randomness")]
    [SerializeField] private bool _useFunky = false;
    [SerializeField, Range(0f, 1f)] private float _lengthVariance = 0f;
    [SerializeField, Range(0f, 1f)] private float _randomVariance = 0.75f;
    [SerializeField, Range(0.25f, 2f)] private float _deltaMin = 0.75f;

    private int _size;
    private bool _bakedToTransforms = false;
    private Vector3[] _points;

    private Vector3[] _vectors = new Vector3[] { Vector3.left, Vector3.up, Vector3.right, Vector3.down };

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

        for (int i = transform.childCount - 1; i >= 0; i -= 1)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }


        int pointIndex = 1;

        foreach (var point in _points)
        {
            var childTransform = new GameObject().transform;

            childTransform.name = $"Point {pointIndex++}";

            childTransform.transform.position = point;
            childTransform.transform.SetParent(gameObject.transform);
        }

        _bakedToTransforms = true;

        Debug.Log($"Baked {pointIndex - 1} points into Transforms.");
    }

    /// <summary>
    /// Bakes the mesh into a power trail gameobject.
    /// </summary>
    /// <param name="name"></param>
    public void BakeMeshIntoGameObject(string name)
    {
        if (!_bakedToTransforms)
        {
            Debug.LogError("You cannot bake a path to a IRenderableTrail without baking it to Transforms first!");

            // NOTE:
            // Ok, technically you *can*, but I wanted to use MovingElements for the moving VFX graph on the PowerTrail
            // component. Since I don't want to have to magic up transforms out of nowhere, I'm forcing you to bake. Sucks.

            return;
        }

        InitFields();

        var fab = Instantiate(_bakedObjectPrefab);
        var meshFilter = fab.GetComponent<MeshFilter>();

        fab.name = name;

        Mesh destMesh = new Mesh();
        destMesh.name = $"Custom LineRender #{destMesh.GetHashCode()}";

        lineRenderer.BakeMesh(destMesh);

        meshFilter.sharedMesh = destMesh;

        if (fab.TryGetComponent(out IRenderableTrail trail))
        {
            var children = new Transform[transform.childCount];

            int i = 0;
            foreach (Transform t in transform)
            {
                children[i++] = Instantiate(t.gameObject, fab.transform).transform;
            }

            trail.EditorOnlySetPoints(children);
        }

        _bakedToTransforms = false;

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

        var vec = _requiredPoints[0].position;
        vec.z = _zPos;

        var building = new List<Vector3>
            {
                vec
            };

        for (int i = 1; i < _requiredPoints.Length; i += 1)
        {
            var targetPoint = _requiredPoints[i].position;
            var currentPoint = _requiredPoints[i - 1].position;

            targetPoint.z = _zPos;
            currentPoint.z = _zPos;

            if (!_cardinalOnly)
            {
                // lazy
                building.Add(targetPoint);
            }
            else
            {
                // compare every position with the next to see if it's worth making a segment in that cardinal direction.
                do
                {
                    float dx = targetPoint.x - currentPoint.x;
                    float dy = targetPoint.y - currentPoint.y;

                    float adx = Mathf.Abs(dx);
                    float ady = Mathf.Abs(dy);

                    // max distance the segment can travel
                    float maxDist = _maxGeneratedSegmentLength * (1f + Random.Range(-_lengthVariance, _lengthVariance));


                    Vector3 offset = _useFunky ?
                        FunkyAlgo(dx, dy, adx, ady, maxDist, building.Count) :
                        SimpleAlgo(dx, dy, adx, ady, maxDist);

                    // offset us
                    currentPoint += offset;

                    // then add us
                    building.Add(currentPoint);

                    // all the while making sure we haven't reach the destination yet.
                } while (Vector3.Distance(currentPoint, targetPoint) > 0.2f);
            } 


        }

        lineRenderer.positionCount = building.Count;
        _size = building.Count;
        lineRenderer.SetPositions(building.ToArray());

        _bakedToTransforms = false;

        Debug.Log($"Successfully generated a path using {building.Count} positions.");
    }

    // Don't look at the arguments please.
    // I just pulled out the behaviors and didn't think twice, lol.
    private Vector3 SimpleAlgo(float dx, float dy, float adx, float ady, float maxDist)
    {
        // determine the directional offset
        Vector3 offset;
        // less powerful randomness
        if ((adx > ady) || (Random.Range(0f, 1f) > (1f - _randomVariance) && dy > _deltaMin && dx > _deltaMin))
        {
            offset = Mathf.Round(dx / adx) * Mathf.Min(adx, maxDist) * Vector3.right;
        }
        else
        {
            offset = Mathf.Round(dy / ady) * Mathf.Min(ady, maxDist) * Vector3.up;
        }

        return offset;
    }

    private Vector3 FunkyAlgo(float dx, float dy, float adx, float ady, float maxDist, int count)
    {
        // determine the directional offset
        Vector3 offset;

        bool doRandom = Random.Range(0f, 1f) > (1f - Mathf.Pow(_randomVariance, count * 1.5f));

        // heavily utilizes randomness
        if (doRandom && dy > _deltaMin && dx > _deltaMin)
        {
            offset = Random.Range(_maxGeneratedSegmentLength / 2f, _maxGeneratedSegmentLength) * _vectors[(int)Random.Range(0f, _vectors.Length)];
        }
        else if (adx > ady)
        {
            offset = Mathf.Round(dx / adx) * Mathf.Min(adx, maxDist) * Vector3.right;
        }
        else
        {
            offset = Mathf.Round(dy / ady) * Mathf.Min(ady, maxDist) * Vector3.up;
        }

        return offset;
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
    /// This algorithm is not the best (it doesn't *totally* reduce), but it works well enough for the task.
    ///
    /// Feel free to improve on it!
    /// </summary>
    public void Reduce()
    {
        InitFields();

        if (!_cardinalOnly)
        {
            Debug.LogError("Don't Reduce non-cardinal generations! They are already optimal! You can Linearize them if you want, though.");
            return;
        }

        if (_size < 3)
        {
            Debug.LogError("You cannot reduce with less than 3 points!");
            return;
        }

        var building = new List<Vector3>();


        bool skipLeft = false;

        // the logic here is to find left and right positions that, between them, contain meaningless segments.
        for (int i = 0; i < _size - 1; i += 1)
        {
            var left = _points[i]; // find our left bound

            // if we didn't already add the segment in the j for loop with the add(right) line, add the current left.
            if (!skipLeft)
            {
                building.Add(left);
            }

            int j;

            // now try to find a right bound, excluding the last point.
            for (j = i + 1; j < _size; j += 1)
            {
                var right = _points[j];

                // if the right is different from the left, add it and the point previous to it
                if (!Approx(left.x, right.x) && !Approx(left.y, right.y))
                {
                    // we need this segment bc it is the one that connects the left and right w/ meaningful data
                    // "meaningful" = a delta on the opposite 2D axis (x or y) than the previous delta.
                    // e.g. (0, 1) -> (0, 2) -> (0, 3) => (0, 2) is meaningless since (0, 1) -> (0, 3) is the same line.
                    building.Add(_points[j - 1]);
                    building.Add(right);

                    skipLeft = true; // we don't want to dupe add the "right"

                    break;
                }
            }

            i = j - 1; // repeat the process, but the right is now the left.
        }

        // add the Destination point if we didn't already add it.
        // This is because I'm too lazy to keep bug-fixing the reduction algorithm.
        // but hey I can afford to code stupid in an Editor script :>
        if (building[^1] != _points[_size - 1]) // i have never seen the [^i] indexing before, but it is AWESOME!
        {
            building.Add(_points[_size - 1]);
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

        _bakedToTransforms = false;

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

        _bakedToTransforms = false;

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
