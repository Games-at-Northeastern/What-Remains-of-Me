using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    private Vector3 offset;

    public PathfindingGrid(int width, int height, float cellSize, Vector3 offset)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.offset = offset;

        this.gridArray = new int[width, height];

        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.red, 100f);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.red, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.red, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.red, 100f);
    }

    public int GetValue(int x, int y)
    {
        return gridArray[x, y];
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && x <= width && y >= 0 && y <= height)
        {
            gridArray[x, y] = value;
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + offset;
    }

    public Vector2Int GetXY(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - offset.x) / cellSize);
        int y = Mathf.FloorToInt((worldPosition.y - offset.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public bool ContainsCell(int x, int y)
    {
        bool existsOnX = (x <= width) && (x >= 0);
        bool existsOnY = (y <= height) && (y >= 0);
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
