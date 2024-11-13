using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;
using System.IO;

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
            string directory = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path) + "_Connected" + Path.GetExtension(path);

            AssetDatabase.CreateAsset(crt, Path.Join(directory, name));

            swapMap.Add(rt, crt);
        }

        AssetDatabase.Refresh();

        var scenesToSwap = GetScenes();

        var initialScene = EditorSceneManager.GetActiveScene();

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        foreach(SceneAsset sceneAsset in scenesToSwap)
        {
            UnityEngine.SceneManagement.Scene scene;
            try
            {
                scene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset));
            }
            catch (Exception e)
            {
                scene = EditorSceneManager.GetActiveScene();
                Debug.Log(e.Message);
            }

            Grid[] grids = Array.ConvertAll(FindObjectsOfType(typeof(Grid)), obj => (Grid)obj);

            foreach(Grid grid in grids)
            {
                GameObject gridObj = grid.gameObject;

                ReplaceAllTiles rat = gridObj.AddComponent<ReplaceAllTiles>();

                foreach ((var rt, var crt) in swapMap)
                {
                    rat.Find = rt;
                    rat.Replace = crt;

                    rat.ReplaceTiles();
                }

                DestroyImmediate(rat);
            }

            EditorSceneManager.SaveScene(scene);
        }

        EditorSceneManager.OpenScene(initialScene.path);
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

    private List<T> GetAllAssetsOfType<T>(string searchType) where T : UnityEngine.Object
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
