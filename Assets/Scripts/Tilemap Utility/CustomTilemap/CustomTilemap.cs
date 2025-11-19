using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class CustomTilemap : MonoBehaviour
{
    [SerializeField] public CustomTileset Tileset;
}

public static class CustomTilemapMenu
{
    [MenuItem("GameObject/2D Object/Tilemap/Custom", false, 10)]
    public static void CreateCustomTilemap(MenuCommand menuCommand)
    {
        var grid = new GameObject("Grid", typeof(Grid));
        GameObjectUtility.SetParentAndAlign(grid, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(grid, "Create Grid");

        var tilemap = new GameObject("Custom Tilemap", typeof(Tilemap), typeof(TilemapRenderer), typeof(CustomTilemap));
        tilemap.transform.SetParent(grid.transform, false);
        Undo.RegisterCreatedObjectUndo(tilemap, "Create Custom Tilemap");

        Selection.activeObject = tilemap;
    }
}
