using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTileSelector : EditorWindow
{
    public static CustomTileSelector Instance { get; private set; }
    private List<CustomTile> all;
    private List<CustomTile> valid;
    private Tilemap tilemap;
    private Vector3Int target;
    private Vector3 targetCenter;
    private CustomTile ogTile = null;
    private bool ogTileSet = false;
    private CustomTile previewedTile;

    private bool showValidTiles = true;
    private bool shouldRepaint = true;
    private const int Padding = 8;
    private const int Columns = 8;
    private const int TileSize = 40;

    public static void ShowWindow(List<CustomTile> validTiles, List<CustomTile> allTiles, Tilemap tilemap, Vector3Int cell)
    {
        if (Instance != null)
        {
            Instance.Close();
            Instance = null;
        }
        Instance = CreateInstance<CustomTileSelector>();
        Instance.titleContent = new GUIContent("Select Tile");
        Instance.all = allTiles;
        Instance.valid = validTiles;
        Instance.tilemap = tilemap;
        Instance.target = cell;
        Instance.targetCenter = tilemap.GetCellCenterWorld(Instance.target);
        Instance.ShowUtility();
        Instance.Focus();
    }

    private void OnGUI()
    {
        if (all == null || valid == null || tilemap == null)
        {
            Debug.LogError("Failed to show CustomTileSelector window");
            Close();
            return;
        }

        Handles.DrawWireCube(targetCenter, tilemap.cellSize);

        if (!ogTileSet)
        {
            ogTile = tilemap.GetTile(target) as CustomTile;
            ogTileSet = true;
        }

        showValidTiles = EditorGUILayout.Toggle("Show Valid Tiles Only", showValidTiles);

        if (showValidTiles)
        {
            DisplayTiles(ref valid);
        }
        else
        {
            DisplayTiles(ref all);
        }
    }

    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;

    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    public void OnSceneGUI(SceneView sceneView)
    {
        if (!tilemap || target == null)
        {
            return;
        }
        Handles.color = Color.yellow;
        Handles.DrawWireCube(targetCenter, tilemap.cellSize);
    }

    private void DisplayTiles(ref List<CustomTile> tiles)
    {
        var rows = Mathf.CeilToInt((float)(tiles.Count + 1) / Columns);
        for (var y = 0; y < rows; y++)
        {
            GUILayout.BeginHorizontal();
            for (var x = 0; x < Columns; x++)
            {
                var rect = GUILayoutUtility.GetRect(TileSize + Padding, TileSize + Padding, GUILayout.ExpandWidth(false));
                Rect tileRect = new(rect.x + (Padding / 2), rect.y + (Padding / 2), rect.width - Padding, rect.height - Padding);
                var index = (y * Columns) + x - 1;
                if (index >= tiles.Count)
                {
                    break;
                }

                CustomTile tile = null;
                Sprite sprite = null;
                if (index != -1)
                {
                    tile = tiles[index];
                    sprite = tile.sprite;
                }

                if (tile == previewedTile)
                {
                    EditorGUI.DrawRect(rect, Color.black);
                }

                if (rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                    {
                        if (previewedTile == tile)
                        {
                            ClearPreview();
                        }
                        else
                        {
                            SetTile(tile, true);
                        }
                        shouldRepaint = true;
                    }

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        SetTile(tile);
                        Close();
                        GUIUtility.ExitGUI();
                        Instance = null;
                    }
                }

                if (sprite != null && sprite.texture != null)
                {
                    GUI.DrawTexture(tileRect, sprite.texture, ScaleMode.ScaleToFit, true);
                }
            }
            GUILayout.EndHorizontal();
        }

        if (shouldRepaint)
        {
            Repaint();
            shouldRepaint = false;
        }
    }

    private void SetTile(CustomTile tile, bool preview = false)
    {
        if (!tilemap || target == null)
        {
            return;
        }
        previewedTile = preview ? tile : null;
        tilemap.SetTile(target, tile);
        EditorUtility.SetDirty(tilemap);
    }

    private void ClearPreview()
    {
        if (!tilemap || target == null)
        {
            return;
        }

        tilemap.SetTile(target, ogTile);
        EditorUtility.SetDirty(tilemap);
        previewedTile = null;
    }
}
