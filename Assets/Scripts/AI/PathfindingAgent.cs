using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PathfindingAgent : MonoBehaviour
{
    public GridObject grid;
    public Transform target;
    public Object ai;

    public float delay;
    public float stoppingDistance;
    public bool isFollowing;
    public bool debugMode;

    private Rigidbody2D rb;
    private PathfindingGrid pathfindingMap;
    private float currentTime;
    private List<Vector3Int> path;
    private int pathIndex = 0;

    // A* PATHFINDING PRIVATE VARIABLES
    private Transform start;
    private List<Vector3Int> openList;
    private List<Vector3Int> neighbors;
    private Dictionary<Vector3Int, Vector3Int> cameFrom;
    private Dictionary<Vector3Int, float> costSoFar;

    private Vector3Int DEFAULT_VECTOR3INT = new Vector3Int(-9999, -9999, -9999);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pathfindingMap = grid.GetPathfindingGrid();
        currentTime = delay;
        start = this.transform;
        path = new List<Vector3Int>();
    }

    private void Update()
    {
        if (pathfindingMap == null)
        {
            pathfindingMap = grid.GetPathfindingGrid();
        }

        if (currentTime <= 0 && isFollowing)
        {
            if (target != null) path = AstarAlgorithm();
            if (debugMode && path.Count > 0) PrintPath(path);
            currentTime = delay;
            return;
        }
        if (target != null && path.Count > 0 && pathIndex < path.Count && isFollowing) MoveOnPath();
        currentTime -= Time.fixedDeltaTime;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void MoveOnPath()
    {
        Vector3Int[] pathArray = path.ToArray();
        if (Vector3.Distance(transform.position, pathfindingMap.GetWorldPosition(path[pathIndex].x, path[pathIndex].y)) < stoppingDistance
            && pathIndex < path.Count - 1)
        {
            pathIndex++;
        }

        ((BehaviorTree)ai).Move(pathfindingMap.GetWorldPosition(pathArray[pathIndex].x, pathArray[pathIndex].y));
    }

    private void PrintPath(List<Vector3Int> path)
    {
        Vector3Int[] pathArray = path.ToArray();
        Vector3Int pos1 = pathArray[0];
        Vector3Int pos2;
        for (int ii = 1; ii < path.Count; ii++)
        {
            pos2 = pathArray[ii];
            Debug.DrawLine(pathfindingMap.GetWorldPosition(pos1.x, pos1.y), pathfindingMap.GetWorldPosition(pos2.x, pos2.y), Color.blue, delay);
            pos1 = pos2;
        }
    }

    private List<Vector3Int> FindNeighbors(Vector3Int pos)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for (int xx = pos.x - 1; xx <= pos.x + 1; xx++)
        {
            for (int yy = pos.y - 1; yy <= pos.y + 1; yy++)
            {
                Vector3Int newPos = new Vector3Int(xx, yy, pos.z);
                if (newPos != pos && pathfindingMap.ContainsCell(newPos.x, newPos.y))
                {
                    neighbors.Add(newPos);
                }
            }
        }

        return neighbors;
    }

    private float GetCost(Vector3Int current, Vector3Int next)
    {
        float cost = 1f;
        if (current.x != next.x && current.y != next.y) cost = 1.4f;
        return cost;
    }

    private float Heuristic(Vector3Int target, Vector3Int next)
    {
        float D = 1f;
        float D2 = 1.4f;
        float dx = Mathf.Abs(target.x - next.x);
        float dy = Mathf.Abs(target.y - next.y);
        return D * (dx + dy) + (D2 - 2 * D) * Mathf.Min(dx, dy);
    }

    private List<Vector3Int> AstarAlgorithm()
    {
        bool endFound = false;

        Vector2Int startPos = pathfindingMap.GetXY(start.position);
        Vector3Int startNode = new Vector3Int(startPos.x, startPos.y, 0);
        Vector2Int endPos = pathfindingMap.GetXY(target.position);
        Vector3Int endNode = new Vector3Int(endPos.x, endPos.y, 0);

        PathfindingMinheap frontier = new PathfindingMinheap();
        frontier.Add(startNode, 0);

        cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        costSoFar = new Dictionary<Vector3Int, float>();
        cameFrom.Add(startNode, DEFAULT_VECTOR3INT);
        costSoFar.Add(startNode, 0f);

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
                costSoFar.TryGetValue(next, out currentCost);
                float newCost = currentCost + GetCost(current, next);

                float oldCost;
                bool costExists = costSoFar.TryGetValue(next, out oldCost);
                if (!costExists || newCost < oldCost)
                {
                    if (costExists) costSoFar.Remove(next);
                    costSoFar.Add(next, newCost);

                    float priority = newCost + Heuristic(endNode, next);
                    frontier.Add(next, priority);
                    cameFrom.Add(next, current);
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
                cameFrom.TryGetValue(currentNode, out nextNode);
                currentNode = nextNode;
            }
            path.Reverse();
        }

        return path;
    }
}
