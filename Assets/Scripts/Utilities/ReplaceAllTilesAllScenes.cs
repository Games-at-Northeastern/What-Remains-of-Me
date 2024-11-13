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
        List<RuleTile> tilesToSwap = GetRuleTiles();

        var swapMap = new Dictionary<RuleTile, ConnectedRuleTile>();

        foreach (RuleTile rt in tilesToSwap)
        {
            if (rt is ConnectedRuleTile)
            {
                Debug.Log(rt.name + "isrt");
                continue;
            }

            ConnectedRuleTile crt = CreateConnected(rt);


            string path = AssetDatabase.GetAssetPath(rt);
            AssetDatabase.CreateAsset(crt, path);
        }

        AssetDatabase.Refresh();
    }

    private ConnectedRuleTile CreateConnected(RuleTile rt)
    {
        ConnectedRuleTile crt = CreateInstance(typeof(ConnectedRuleTile)) as ConnectedRuleTile;

        crt.m_DefaultSprite = rt.m_DefaultSprite;
        crt.m_DefaultGameObject = rt.m_DefaultGameObject;
        crt.m_DefaultColliderType = rt.m_DefaultColliderType;
        crt.m_TilingRules = rt.m_TilingRules;

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
