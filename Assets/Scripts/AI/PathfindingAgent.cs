using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class the represents Pathfinding for the AI
/// </summary>
public class PathfindingAgent : MonoBehaviour
{
    public GridObject grid;
    public Transform target;
    public Object ai;

    public float delay;
    public float stoppingDistance;
    public bool isFollowing;
    public bool debugMode;

    [SerializeField] private Rigidbody2D _rb;
    private PathfindingGrid _pathfindingMap;
    private float _currentTime;
    private List<Vector3Int> _path;
    private int _pathIndex = 0;

    // A* PATHFINDING PRIVATE VARIABLES
    private Transform _start;
    private List<Vector3Int> _openList;
    private List<Vector3Int> _neighbors;
    private Dictionary<Vector3Int, Vector3Int> _cameFrom;
    private Dictionary<Vector3Int, float> _costSoFar;

    private Vector3Int DEFAULT_VECTOR3INT = new Vector3Int(-9999, -9999, -9999);

    /// <summary>
    /// Initialize the Pathfinding Agent with the body, grid, time, start,
    /// and path
    /// </summary>
    private void Start()
    {
        //_rb = GetComponent<Rigidbody2D>();
        _pathfindingMap = grid.GetPathfindingGrid();
        _currentTime = delay;
        _start = this.transform;
        _path = new List<Vector3Int>();
    }

    /// <summary>
    /// Every Tick, initilize a path finding map if it doesnt exist,
    /// at the start of following create A* Algorithim,
    /// Move on Path.
    /// </summary>
    private void Update()
    {
        if (_pathfindingMap == null)
        {
            _pathfindingMap = grid.GetPathfindingGrid();
        }

        if (_currentTime <= 0 && isFollowing)
        {
            if (target != null) _path = AstarAlgorithm();
            if (debugMode && _path.Count > 0) PrintPath(_path);
            _currentTime = delay;
            return;
        }
        if (target != null && _path.Count > 0 && _pathIndex < _path.Count && isFollowing) MoveOnPath();
        _currentTime -= Time.fixedDeltaTime;
    }

    /// <summary>
    /// Set the AI's target to the given object
    /// </summary>
    /// <param name="target">Target to be set for the AI's path finding</param>
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    /// <summary>
    /// Increment the AI's path and move it.
    /// </summary>
    private void MoveOnPath()
    {
        Vector3Int[] pathArray = _path.ToArray();
        if (Vector3.Distance(transform.position, _pathfindingMap.GetWorldPosition(_path[_pathIndex].x, _path[_pathIndex].y)) < stoppingDistance
            && _pathIndex < _path.Count - 1)
        {
            _pathIndex++;
        }

        ((IBehaviorTree)ai).Move(_pathfindingMap.GetWorldPosition(pathArray[_pathIndex].x, pathArray[_pathIndex].y));
    }

    /// <summary>
    /// Debugging method to print out the given path
    /// </summary>
    /// <param name="path">path list to be printed</param>
    private void PrintPath(List<Vector3Int> path)
    {
        Vector3Int[] pathArray = path.ToArray();
        Vector3Int pos1 = pathArray[0];
        Vector3Int pos2;
        for (int ii = 1; ii < path.Count; ii++)
        {
            pos2 = pathArray[ii];
            Debug.DrawLine(_pathfindingMap.GetWorldPosition(pos1.x, pos1.y), _pathfindingMap.GetWorldPosition(pos2.x, pos2.y), Color.blue, delay);
            pos1 = pos2;
        }
    }


    /// <summary>
    /// Finds the neighbors along the Path
    /// </summary>
    /// <param name="pos">Given Vector Position</param>
    /// <returns>The neighbors found</returns>
    private List<Vector3Int> FindNeighbors(Vector3Int pos)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for (int xx = pos.x - 1; xx <= pos.x + 1; xx++)
        {
            for (int yy = pos.y - 1; yy <= pos.y + 1; yy++)
            {
                Vector3Int newPos = new Vector3Int(xx, yy, pos.z);
                if (newPos != pos && _pathfindingMap.ContainsCell(newPos.x, newPos.y))
                {
                    neighbors.Add(newPos);
                }
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Gets the cost between the given current and given next vectors
    /// </summary>
    /// <param name="current">Current vector to start from</param>
    /// <param name="next">End vector to compare to</param>
    /// <returns>Cost from vectors</returns>
    private float GetCost(Vector3Int current, Vector3Int next)
    {
        float cost = 1f;
        if (current.x != next.x && current.y != next.y) cost = 1.4f;
        return cost;
    }


    /// <summary>
    /// Finds absolute difference from the x and y between the two inputs
    /// and returns a Heuristic Value
    /// </summary>
    /// <param name="target">Target Vector</param>
    /// <param name="next">Next Vector</param>
    /// <returns>Heuristic result</returns>
    private float Heuristic(Vector3Int target, Vector3Int next)
    {
        float D = 1f;
        float D2 = 1.4f;
        float dx = Mathf.Abs(target.x - next.x);
        float dy = Mathf.Abs(target.y - next.y);
        return D * (dx + dy) + (D2 - 2 * D) * Mathf.Min(dx, dy);
    }


    /// <summary>
    /// A* pathing Algoritihm for AI pathing
    /// </summary>
    /// <returns>the path found</returns>
    private List<Vector3Int> AstarAlgorithm()
    {
        bool endFound = false;

        Vector2Int startPos = _pathfindingMap.GetXY(_start.position);
        Vector3Int startNode = new Vector3Int(startPos.x, startPos.y, 0);
        Vector2Int endPos = _pathfindingMap.GetXY(target.position);
        Vector3Int endNode = new Vector3Int(endPos.x, endPos.y, 0);

        PathfindingMinheap frontier = new PathfindingMinheap();
        frontier.Add(startNode, 0);

        _cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        _costSoFar = new Dictionary<Vector3Int, float>();
        _cameFrom.Add(startNode, DEFAULT_VECTOR3INT);
        _costSoFar.Add(startNode, 0f);

        Vector3Int current;
        while (!frontier.IsEmpty())
        {
            current = frontier.Pop();

            if (current == endNode)
            {
                endFound = true;
                break;
            }

            foreach (Vector3Int next in FindNeighbors(current))
            {
                float currentCost;
                _costSoFar.TryGetValue(next, out currentCost);
                float newCost = currentCost + GetCost(current, next);

                float oldCost;
                bool costExists = _costSoFar.TryGetValue(next, out oldCost);
                if (!costExists || newCost < oldCost)
                {
                    if (costExists) _costSoFar.Remove(next);
                    _costSoFar.Add(next, newCost);

                    float priority = newCost + Heuristic(endNode, next);
                    frontier.Add(next, priority);
                    _cameFrom.Add(next, current);
                }
            }
        }

        List<Vector3Int> path = new List<Vector3Int>();

        if (endFound)
        {
            Vector3Int currentNode = endNode;
            Vector3Int nextNode;
            while (currentNode != DEFAULT_VECTOR3INT)
            {
                path.Add(currentNode);
                _cameFrom.TryGetValue(currentNode, out nextNode);
                currentNode = nextNode;
            }
            path.Reverse();
        }

        return path;
    }
}
