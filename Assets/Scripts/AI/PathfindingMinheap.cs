using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingMinheap
{
    private List<Tuple<Vector3Int, float>> heap;

    public PathfindingMinheap()
    {
        heap = new List<Tuple<Vector3Int, float>>();
    }

    public void Add(Vector3Int pos, float priority)
    {
        heap.Add(new Tuple<Vector3Int, float>(pos, priority));
    }

    private Tuple<Vector3Int, float> GetTopTuple()
    {
        Tuple<Vector3Int, float> bottom = new Tuple<Vector3Int, float>(new Vector3Int(-9999, -9999, -9999), 9999);
        foreach (Tuple<Vector3Int, float> pair in heap)
        {
            if (bottom.Item2 > pair.Item2)
            {
                bottom = pair;
            }
        }
        return bottom;
    }

    public Vector3Int Top()
    {
        return GetTopTuple().Item1;
    }

    public Vector3Int Pop()
    {
        Tuple<Vector3Int, float> bottom = GetTopTuple();
        Vector3Int bottomPos = bottom.Item1;
        heap.Remove(bottom);
        return bottomPos;
    }

    public bool IsEmpty()
    {
        return heap.Count == 0;
    }
}
