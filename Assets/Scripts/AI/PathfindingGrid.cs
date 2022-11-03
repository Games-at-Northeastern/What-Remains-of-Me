using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents the Path finding Grid
/// with positions, values, and cells
/// </summary>
public class PathfindingGrid
{
    private int _width;
    private int _height;
    private float _cellSize;
    private int[,] _gridArray;
    private Vector3 _offset;

    /// <summary>
    /// Construct a path finding grid
    /// with width, height, cellsize, and offset
    /// Also debug's currently
    /// </summary>
    /// <param name="width">Width of grid</param>
    /// <param name="height">height of grid</param>
    /// <param name="cellSize">size of cell as a float value</param>
    /// <param name="offset">vecor offset</param>
    public PathfindingGrid(int width, int height, float cellSize, Vector3 offset)
    {
        this._width = width;
        this._height = height;
        this._cellSize = cellSize;
        this._offset = offset;

        this._gridArray = new int[width, height];

        for (int i = 0; i < _gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < _gridArray.GetLength(1); j++)
            {
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.red, 100f);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.red, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.red, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.red, 100f);
    }

    /// <summary>
    /// Gets the value at the given position in the grid
    /// </summary>
    /// <param name="x">X position of value</param>
    /// <param name="y">Y position of the value</param>
    /// <returns>Desired value at position</returns>
    public int GetValue(int x, int y)
    {
        return _gridArray[x, y];
    }

    /// <summary>
    /// Set the value at a given position on the grid
    /// </summary>
    /// <param name="x">X value of the position</param>
    /// <param name="y">Y value of the position</param>
    /// <param name="value">desired value ot be set at position</param>
    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && x <= _width && y >= 0 && y <= _height)
        {
            _gridArray[x, y] = value;
        }
    }


    /// <summary>
    /// Get's the world poistion as a Vector3 using given x and y
    /// </summary>
    /// <param name="x">Given x </param>
    /// <param name="y">Given y position</param>
    /// <returns>World Position as a Vector3</returns>
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * _cellSize + _offset;
    }


    /// <summary>
    /// Return a vector2 with desired x and y from the given world position
    /// </summary>
    /// <param name="worldPosition">WorldPosition to retrieve a x and y from</param>
    /// <returns>Desired X and Y as a Vector2</returns>
    public Vector2Int GetXY(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - _offset.x) / _cellSize);
        int y = Mathf.FloorToInt((worldPosition.y - _offset.y) / _cellSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Checks if a cell exists at a given x and y.
    /// (Not fully implemented yet)
    /// </summary>
    /// <param name="x">Given x position</param>
    /// <param name="y">Givne y position</param>
    /// <returns>If a cell is at the given position</returns>
    public bool ContainsCell(int x, int y)
    {
        bool existsOnX = (x <= _width) && (x >= 0);
        bool existsOnY = (y <= _height) && (y >= 0);
        // UNCOMMENT WHEN ADDING FURTHER GRID FUNCTIONALITY
        /*
        if (existsOnX && existsOnY)
        {
            return gridArray[x, y] > 0;
        }
        return false;
        */
        return existsOnX && existsOnY;
    }
}
