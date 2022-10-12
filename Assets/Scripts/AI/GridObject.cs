using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents a Grid to be used for AI pathing
/// </summary>
public class GridObject : MonoBehaviour
{
    public int width;
    public int height;
    public float cellSize;

    private PathfindingGrid grid;


    /// <summary>
    /// Initialize a Pathing Grid.
    /// </summary>
    void Start()
    {
        grid = new PathfindingGrid(width, height, cellSize, this.transform.position);
    }

    /// <summary>
    /// Returns the Pathing Grid.
    /// </summary>
    /// <returns></returns>
    public PathfindingGrid GetPathfindingGrid()
    {
        return grid;
    }
}
