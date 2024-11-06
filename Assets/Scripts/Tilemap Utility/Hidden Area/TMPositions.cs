using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UtilityData;

public class TMPositions
{
    #region Singleton

    public struct ClassifyResult
    {
        public Vector3IntSet TilePositions { get; set; }
        public Vector3IntSet BorderPositions { get; set; }

        public ClassifyResult(Vector3IntSet tilePositions, Vector3IntSet borderPositions)
        {
            TilePositions = tilePositions;
            BorderPositions = borderPositions;
        }
    }

    private static readonly TMPositions UtilityTMP = new(false);

    public static ClassifyResult ClassifyTiles(Tilemap tilemap, bool includeDiagonals)
    {
        UtilityTMP.SetIncludeDiagonals(includeDiagonals, false);
        UtilityTMP.LoadTilemap(tilemap);

        ClassifyResult result = new ClassifyResult(
            UtilityTMP.CloneTilePositions(),
            UtilityTMP.CloneBorderPositions());

        UtilityTMP.Clear();

        return result;
    }

    public static Vector3IntSet GetTilePositions(Tilemap tilemap, bool includeDiagonals)
    {
        UtilityTMP.SetIncludeDiagonals(includeDiagonals, false);
        UtilityTMP.LoadTilemap(tilemap);
        Vector3IntSet result = UtilityTMP.CloneTilePositions();

        UtilityTMP.Clear();

        return result;
    }


    public static Vector3IntSet GetBorderPositions(Tilemap tilemap, bool includeDiagonals)
    {
        UtilityTMP.SetIncludeDiagonals(includeDiagonals, false);
        UtilityTMP.LoadTilemap(tilemap);
        Vector3IntSet result = UtilityTMP.CloneBorderPositions();

        UtilityTMP.Clear();

        return result;
    }

    #endregion

    public Vector3IntSet TilePositions { get; private set; }
    public Vector3IntSet BorderPositions { get; private set; }

    private bool includeDiagonals;

    public TMPositions(bool includeDiagonals) {
        this.includeDiagonals = includeDiagonals;
        TilePositions = new Vector3IntSet();
        BorderPositions = new Vector3IntSet();
    }

    public void LoadTilemap(Tilemap tilemap, bool overrwrite = true)
    {
        if (overrwrite)
        {
            Clear();
        }
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Add(pos);
            }
        }
    }

    public void Add(Vector3Int position)
    {
        if (TilePositions.Contains(position))
        {
            return;
        }

        TilePositions.Add(position);
        BorderPositions.Remove(position);

        foreach (Vector3Int neighbour in GetCandidateNeighbours(position))
        {
            if (!TilePositions.Contains(neighbour))
            {
                BorderPositions.Add(neighbour);
            }
        }
    }

    public void Clear()
    {
        TilePositions.Clear();
        BorderPositions.Clear();
    }

    public bool ContainsPosition(Vector3Int position) => TilePositions.Contains(position) || BorderPositions.Contains(position);

    public void SetIncludeDiagonals(bool includeDiagonals, bool recalculate = true)
    {
        this.includeDiagonals = includeDiagonals;

        if (!recalculate)
        {
            return;
        }

        if (this.includeDiagonals)
        {
            foreach (Vector3Int currentBorderPos in BorderPositions)
            {
                foreach (Vector3Int borderNeighbour in GetCandidateNeighbours(currentBorderPos, false))
                {
                    if (ContainsPosition(borderNeighbour))
                    {
                        continue;
                    }

                    if (HasNeighbourIn(TilePositions, borderNeighbour))
                    {
                        BorderPositions.Add(borderNeighbour);
                    }
                }
            }
        }
        else
        {
            foreach (Vector3Int currentBorderPos in BorderPositions)
            {
                if (!HasNeighbourIn(TilePositions, currentBorderPos))
                {
                    BorderPositions.Remove(currentBorderPos);
                }
            }
        }
    }

    public Vector3IntSet CloneTilePositions() => ClonePositions(TilePositions);

    public Vector3IntSet CloneBorderPositions() => ClonePositions(BorderPositions);

    private static Vector3IntSet ClonePositions(Vector3IntSet positions)
    {
        Vector3IntSet cloned = new Vector3IntSet();
        foreach (Vector3Int position in positions)
        {
            cloned.Add(new Vector3Int(
                position.x,
                position.y,
                position.z));
        }
        return cloned;
    }

    // private utility
    private Vector3Int[] GetCandidateNeighbours(Vector3Int position) => GetCandidateNeighbours(position, includeDiagonals);

    private Vector3Int[] GetCandidateNeighbours(Vector3Int position, bool includeDiagonals)
    {
        Vector3Int[] candidates = includeDiagonals ? UData.NeighbourAdjacentDiagonals() : UData.NeighbourAdjacents();
        for (int i = 0; i < candidates.Length; i++)
        {
            candidates[i] += position;
        }
        return candidates;
    }

    private bool HasNeighbourIn(Vector3IntSet positionSet, Vector3Int position)
    {
        foreach (Vector3Int neighbour in GetCandidateNeighbours(position))
        {
            if (positionSet.Contains(neighbour))
            {
                return true;
            }
        }
        return false;
    }
}
