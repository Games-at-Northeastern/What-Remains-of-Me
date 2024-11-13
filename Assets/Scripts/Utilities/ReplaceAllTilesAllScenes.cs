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

    [SerializeField] private bool useAllScenes;
    [SerializeField] private List<SceneAsset> scenes;

    [SerializeField] private bool useAllPrefabs;
    [SerializeField] private List<GameObject> prefabs;

    [SerializeField] private ConnectedRuleTile.SerializableTileBaseHashSet find;
    [SerializeField] private TileBase replace;

    [SerializeField] private List<RuleTile> ruleTiles;

    #region Rule to Connected

    public void ApplyRuleToConnectedProject()
    {
        List<RuleTile> tilesToSwap = GetRuleTiles();

        var swapMap = new Dictionary<TileBase, TileBase>();

        foreach (RuleTile rt in tilesToSwap)
        {
            if (rt is ConnectedRuleTile)
            {
                continue;
            }

            ConnectedRuleTile crt = CreateConnected(rt);


            string path = AssetDatabase.GetAssetPath(rt);
            string directory = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path) + "_deprecated" + Path.GetExtension(path);

            AssetDatabase.RenameAsset(path, Path.Join(directory, name));

            AssetDatabase.CreateAsset(crt, path);

            swapMap.Add(rt, crt);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // edit tilemaps in scenes

        var scenesToSwap = GetAllAssetsOfType<SceneAsset>("Scene");

        var initialScene = EditorSceneManager.GetActiveScene().path;

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

                List<Tilemap> tilemaps = new List<Tilemap>();
                ReplaceAllTiles.AppendTilemaps(gridObj.transform, tilemaps);

                ReplaceTiles(swapMap, tilemaps);
            }

            EditorSceneManager.SaveScene(scene);
        }

        EditorSceneManager.OpenScene(initialScene);

        // edit tilemaps in prefabs

        var prefabsToSwap = GetAllAssetsOfType<GameObject>("GameObject");

        foreach (GameObject gameObject in prefabsToSwap)
        {
            Debug.Log(gameObject.name);
            List<Tilemap> tilemaps = new List<Tilemap>();

            ReplaceAllTiles.AppendTilemaps(gameObject.transform, tilemaps);

            ReplaceTiles(swapMap, tilemaps);
        }

        foreach (RuleTile oldTile in swapMap.Keys)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldTile));
            DestroyImmediate(oldTile, true);
        }

        AssetDatabase.SaveAssets();
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

    #region Find/Replace

    private void FindReplace()
    {
        if (find.Count <= 0 || replace == null)
        {
            return;
        }

        var scenesToSwap = GetScenes();

        var initialScene = EditorSceneManager.GetActiveScene().path;

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        foreach (SceneAsset sceneAsset in scenesToSwap)
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

            foreach (Grid grid in grids)
            {
                GameObject gridObj = grid.gameObject;

                List<Tilemap> tilemaps = new List<Tilemap>();
                ReplaceAllTiles.AppendTilemaps(gridObj.transform, tilemaps);

                ReplaceTiles(find, replace, tilemaps);
            }

            EditorSceneManager.SaveScene(scene);
        }

        EditorSceneManager.OpenScene(initialScene);

        // edit tilemaps in prefabs

        var prefabsToSwap = GetPrefabs();

        foreach (GameObject gameObject in prefabsToSwap)
        {
            List<Tilemap> tilemaps = new List<Tilemap>();

            ReplaceAllTiles.AppendTilemaps(gameObject.transform, tilemaps);

            ReplaceTiles(find, replace, tilemaps);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    #endregion

    private List<GameObject> GetPrefabs()
    {
        if (useAllPrefabs)
        {
            return ExcludeItems(GetAllAssetsOfType<GameObject>("GameObject"), prefabs);
        }

        return prefabs;
    }

    private List<SceneAsset> GetScenes()
    {
        if (useAllScenes)
        {
            return ExcludeItems(GetAllAssetsOfType<SceneAsset>("Scene"), scenes);
        }

        return scenes;
    }

    private List<T> ExcludeItems<T>(List<T> items, List<T> excluded)
    {
        List<T> result = new List<T>();

        foreach(T item in items)
        {
            if (!excluded.Contains(item))
            {
                result.Add(item);
            }
        }

        return result;
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

    public void ReplaceTiles(Dictionary<TileBase, TileBase> dict, List<Tilemap> tilemaps)
    {

        foreach (Tilemap tilemap in tilemaps)
        {
            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(position))
                {
                    continue;
                }

                TileBase tile = tilemap.GetTile(position);

                if (dict.ContainsKey(tile))
                {
                    tilemap.SetTile(position, dict[tile]);
                }
            }
        }
    }

    public void ReplaceTiles(ConnectedRuleTile.SerializableTileBaseHashSet findTiles, TileBase replaceTile, List<Tilemap> tilemaps)
    {
        Dictionary<TileBase, TileBase> swapDict = new Dictionary<TileBase, TileBase>();

        foreach (TileBase key in findTiles)
        {
            if (key != null)
            {
                swapDict.Add(key, replaceTile);
            }
        }

        ReplaceTiles(swapDict, tilemaps);
    }

    private SerializedObject serializedObject;

    private SerializedProperty scenesProp;
    private SerializedProperty useAllScenesProp;
    private SerializedProperty prefabsProp;
    private SerializedProperty useAllPrefabsProp;

    private SerializedProperty findProp;
    private SerializedProperty replaceProp;

    private SerializedProperty ruleTilesProp;

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);

        scenesProp = serializedObject.FindProperty("scenes");
        useAllScenesProp = serializedObject.FindProperty("useAllScenes");

        prefabsProp = serializedObject.FindProperty("prefabs");
        useAllPrefabsProp = serializedObject.FindProperty("useAllPrefabs");

        findProp = serializedObject.FindProperty("find");
        replaceProp = serializedObject.FindProperty("replace");

        ruleTilesProp = serializedObject.FindProperty("ruleTiles");
    }

    private bool areYouSureFR = false;
    private bool areYouSureConvert = false;

    public void OnGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField(new GUIContent("Find/Replace", "Specifies where in the project tile replace operations should execute. If use all scenes is selected, scenes in the scenes list are excluded from find/replace. If use all scenes is" +
            " turned off, scenes in the scenes list are find/replaced"), EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.PropertyField(useAllScenesProp);
        EditorGUILayout.PropertyField(scenesProp);

        EditorGUILayout.PropertyField(useAllPrefabsProp);
        EditorGUILayout.PropertyField(prefabsProp);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(findProp);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(replaceProp);
        EditorGUILayout.Space();

        bool findReplace = GUILayout.Button(areYouSureFR ? "Are you sure?" : "Run Find/Replace");
        if (findReplace)
        {
            if (areYouSureFR)
            {
                FindReplace();
            }
            areYouSureFR = !areYouSureFR;
        }

        EditorGUILayout.Space();

        // draw convert vertical

        EditorGUILayout.LabelField("Convert Rule Tiles to Connected Tiles", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.PropertyField(ruleTilesProp, new GUIContent("Rule Tiles", "Rule Tiles to convert, if empty, converts all non converted rule tiles in Assets"));

        bool convert = GUILayout.Button(areYouSureConvert ? "Are you sure?" : "Run Rule Tile Convert");
        if (convert)
        {
            if (areYouSureConvert)
            {
                ApplyRuleToConnectedProject();
            }
            areYouSureConvert = !areYouSureConvert;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
