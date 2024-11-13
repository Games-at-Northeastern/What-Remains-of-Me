using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.IO;
using System.Reflection;

public class ReplaceAllTilesAllScenes : EditorWindow
{
    [MenuItem("Window/UI Toolkit/ReplaceAllTilesAllScenes")]
    public static void ShowExample()
    {
        ReplaceAllTilesAllScenes wnd = GetWindow<ReplaceAllTilesAllScenes>();
        wnd.titleContent = new GUIContent("ReplaceAllTilesAllScenes");
    }

    [SerializeField] private List<SceneAsset> scenes;
    [SerializeField] private List<TileBase> tiles;
    [SerializeField] private List<RuleTile> ruleTiles;

    [SerializeField] private Tile replace;

    #region Rule to Connected

    public void ApplyRuleToConnectedProject()
    {
        //List<SceneAsset> replaceScenes = GetScenes();
        List<RuleTile> tilesToSwap = GetRuleTiles();

        var swapMap = new Dictionary<RuleTile, ConnectedRuleTile>();

        foreach (RuleTile rt in tilesToSwap)
        {
            ConnectedRuleTile crt = CreateConnected(rt);
            EditorUtility.CopySerialized(crt, rt);
        }

        //foreach(SceneAsset scene in replaceScenes)
        //{
        //    
        //}
    }

    private ConnectedRuleTile CreateConnected(RuleTile rt)
    {
        Debug.Log(rt.name);
        string path = AssetDatabase.GetAssetPath(rt);
        Debug.Log(path);
        string directory = Path.GetDirectoryName(path);

        string connectedName = Path.GetFileNameWithoutExtension(path) + "_Connected" + Path.GetExtension(path);
        string connectedPath = Path.Join(directory, connectedName);

        string guid = AssetDatabase.AssetPathToGUID(connectedPath, AssetPathToGUIDOptions.OnlyExistingAssets);

        if (!string.IsNullOrEmpty(guid))
        {
            throw new System.Exception("Connected copy of " + path + "already exists");
        }

        ConnectedRuleTile crt = new ConnectedRuleTile();

        foreach (var prop in rt.GetType().GetProperties())
        {
            PropertyInfo pInfo = crt.GetType().GetProperty(prop.Name);
            if (pInfo.CanWrite)
            {
                pInfo.SetValue(crt, prop.GetValue(rt, null), null);
            }
        }

        AssetDatabase.CreateAsset(crt, connectedPath);

        return crt;
    }

    private List<RuleTile> GetRuleTiles()
    {
        if (ruleTiles.Count > 0)
        {
            return ruleTiles;
        }

        return GetAllAssetsOfType<RuleTile>("RuleTile");
    }


    #endregion

    private List<SceneAsset> GetScenes()
    {
        if (scenes.Count > 0)
        {
            return scenes;
        }

        return GetAllAssetsOfType<SceneAsset>("Scene");
    }

    private List<T> GetAllAssetsOfType<T>(string searchType) where T : Object
    {
        string[] guids = AssetDatabase.FindAssets("t:" + searchType, new string[] { "Assets" });

        List<string> paths = new List<string>();

        foreach (string guid in guids)
        {
            paths.Add(AssetDatabase.GUIDToAssetPath(guid));
        }

        List<T> assets = new List<T>();

        foreach (string path in paths)
        {
            Debug.Log(path);
        }

        foreach (string path in paths)
        {
            assets.Add(AssetDatabase.LoadAssetAtPath<T>(path));
        }

        return assets;
    }

    public void OnGUI()
    {
        bool replaceRules = GUILayout.Button("Convert From Rule to Connected");

        if (replaceRules)
        {
            ApplyRuleToConnectedProject();
        }
    }
}
