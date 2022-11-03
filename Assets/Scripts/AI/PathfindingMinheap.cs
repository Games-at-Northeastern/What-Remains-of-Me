using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents finding the minimum heap path
/// </summary>
public class PathfindingMinheap
{
    private List<Tuple<Vector3Int, float>> _heap;


    /// <summary>
    /// Initialize heap to be a new list tuple of Vector3Int type float
    /// </summary>
    public PathfindingMinheap()
    {
        _heap = new List<Tuple<Vector3Int, float>>();
    }

    /// <summary>
    /// Adds a given Vector3Int to the heap with a given priority
    /// </summary>
    /// <param name="pos">Vector3Int to add</param>
    /// <param name="priority">Its priority</param>
    public void Add(Vector3Int pos, float priority)
    {
        _heap.Add(new Tuple<Vector3Int, float>(pos, priority));
    }

    /// <summary>
    /// Gets the Top tuple in the heap
    /// </summary>
    /// <returns>The Top tuple</returns>
    private Tuple<Vector3Int, float> GetTopTuple()
    {
        Tuple<Vector3Int, float> bottom = new Tuple<Vector3Int, float>(new Vector3Int(-9999, -9999, -9999), 9999);
        foreach (Tuple<Vector3Int, float> pair in _heap)
        {
            if (bottom.Item2 > pair.Item2)
            {
                bottom = pair;
            }
        }
        return bottom;
    }


    /// <summary>
    /// Gets the first component of the TopTuple
    /// </summary>
    /// <returns>the first component from the TopTuple</returns>
    public Vector3Int Top()
    {
        return GetTopTuple().Item1;
    }

    /// <summary>
    /// Pop's off the bottom element from TopTuple and returns it
    /// </summary>
    /// <returns>Bottom element</returns>
    public Vector3Int Pop()
    {
        Tuple<Vector3Int, float> bottom = GetTopTuple();
        Vector3Int bottomPos = bottom.Item1;
        _heap.Remove(bottom);
        return bottomPos;
    }


    /// <summary>
    /// Checks if the heap is currently empty
    /// </summary>
    /// <returns>If the heap is empty</returns>
    public bool IsEmpty()
    {
        return _heap.Count == 0;
    }
}
