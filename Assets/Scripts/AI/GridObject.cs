using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public int width;
    public int height;
    public float cellSize;

    private PathfindingGrid grid;

    void Start()
    {
        grid = new PathfindingGrid(width, height, cellSize, this.transform.position);
    }

    public PathfindingGrid GetPathfindingGrid()
    {
        return grid;
    }
}
