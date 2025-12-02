using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;
using System.Collections.Generic;

[CustomGridBrush(true, false, false, "Custom Tile Brush")]
public class CustomTileBrush : GridBrush
{
    private readonly Vector3Int[] offsets = new Vector3Int[]
    {
        new(-1, 1, 0), new(0, 1, 0), new(1, 1, 0),
        new(-1, 0, 0),               new(1, 0, 0),
        new(-1, -1, 0), new(0, -1, 0), new(1, -1, 0)
    };

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget == null || brushTarget.GetComponent<Tilemap>() == null)
        {
            return;
        }

        var tilemap = brushTarget.GetComponent<Tilemap>();
        var customTilemap = brushTarget.GetComponent<CustomTilemap>();

        if (tilemap == null || customTilemap == null)
        {
            return;
        }

        List<CustomTile> placeableTiles = new();
        foreach (var tile in customTilemap.Tileset.Tiles)
        {
            if (IsPlaceable(tile, tilemap, position))
            {
                placeableTiles.Add(tile);
            }
        }

        CustomTileSelector.ShowWindow(placeableTiles, customTilemap.Tileset.Tiles, tilemap, position);
    }

    private bool IsPlaceable(CustomTile tile, Tilemap tilemap, Vector3Int target) =>
        tile.CanGoBelowRightOfOther(tilemap.GetTile(target + offsets[0]) as CustomTile)
        && tile.CanGoBelowOther(tilemap.GetTile(target + offsets[1]) as CustomTile)
        && tile.CanGoBelowLeftOfOther(tilemap.GetTile(target + offsets[2]) as CustomTile)
        && tile.CanGoRightOfOther(tilemap.GetTile(target + offsets[3]) as CustomTile)
        && tile.CanGoLeftOfOther(tilemap.GetTile(target + offsets[4]) as CustomTile)
        && tile.CanGoAboveRightOfOther(tilemap.GetTile(target + offsets[5]) as CustomTile)
        && tile.CanGoAboveOther(tilemap.GetTile(target + offsets[6]) as CustomTile)
        && tile.CanGoAboveLeftOfOther(tilemap.GetTile(target + offsets[7]) as CustomTile);
}
