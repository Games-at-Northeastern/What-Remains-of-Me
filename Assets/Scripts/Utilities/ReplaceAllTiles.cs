using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR

[RequireComponent(typeof(Grid))]
public class ReplaceAllTiles : MonoBehaviour
{
    [SerializeField] private TileBase find;
    [SerializeField] private TileBase replace;

    [SerializeField] private List<Tilemap> maps;

    public TileBase Find { get => find; set => find = value; }
    public TileBase Replace { get => replace; set => replace = value; }
    public List<Tilemap> Maps { get => maps; set => maps = value; }

    public void ReplaceTiles()
    {
        List<Tilemap> replaceOn;

        if (maps == null)
        {
            maps = new List<Tilemap>();
        }

        if (maps.Count > 0)
        {
            replaceOn = maps;
        }
        else
        {
            replaceOn = new List<Tilemap>();
            AppendTilemaps(transform, replaceOn);
        }

        foreach (Tilemap tilemap in replaceOn)
        {
            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(position))
                {
                    continue;
                }

                if (tilemap.GetTile(position).Equals(find))
                {
                    tilemap.SetTile(position, replace);
                }
            }
        }
    }

    public static void AppendTilemaps(Transform transform, List<Tilemap> tilemaps)
    {
        var tilemap = transform.gameObject.GetComponent<Tilemap>();

        if (!(tilemap == null))
        {
            tilemaps.Add(tilemap);
        }

        foreach (Transform child in transform)
        {
            AppendTilemaps(child, tilemaps);
        }
    }
}


[CustomEditor(typeof(ReplaceAllTiles))]
public class ReplaceAllTilesEditor : Editor
{
    private bool areYouSure = false;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        var replace = GUILayout.Button(areYouSure ? "Are you sure?" : "Replace Tiles");
        if (replace)
        {
            if (areYouSure)
            {
                var replaceAllTiles = target as ReplaceAllTiles;
                Undo.RegisterFullObjectHierarchyUndo(replaceAllTiles.gameObject, "Replace All Tiles");

                replaceAllTiles.ReplaceTiles();
            }
            areYouSure = !areYouSure;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
