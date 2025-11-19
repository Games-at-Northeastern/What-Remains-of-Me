using UnityEngine;
using UnityEditor;
using System.IO;

public class CustomTilesetPopulator : EditorWindow
{
    public static CustomTilesetPopulator Instance { get; private set; }
    private string tileFolderPath = "";
    private CustomTileset tileset = null;

    [MenuItem("Tools/Populate Tileset")]
    public static void ShowWindow()
    {
        if (Instance != null)
        {
            Instance.Close();
            Instance = null;
        }
        Instance = CreateInstance<CustomTilesetPopulator>();
        Instance.Show();
        Instance.Focus();
    }

    private void OnGUI()
    {
        tileFolderPath = EditorGUILayout.TextField("Tile folder path", tileFolderPath);
        tileset = (CustomTileset)EditorGUILayout.ObjectField("Custom tileset", tileset, typeof(CustomTileset), false);

        if (GUILayout.Button("Populate"))
        {
            PopulateTileset();
        }
    }

    private void PopulateTileset()
    {
        if (tileset == null || string.IsNullOrEmpty(tileFolderPath) || !Directory.Exists(tileFolderPath))
        {
            return;
        }

        var tilePaths = Directory.GetFiles(tileFolderPath);
        foreach (var path in tilePaths)
        {
            var tile = AssetDatabase.LoadAssetAtPath(path, typeof(CustomTile)) as CustomTile;
            if (tile == null)
            {
                continue;
            }
            tileset.AddTile(tile);
        }
    }
}
