using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelTagWindow : EditorWindow
{
    private static string tagDataPath = "";
    private static LevelTagData data;

    private void PreGUI()
    {

    }

    private void PostGUI()
    {
    }

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        if (!data)
        {
            data = AssetDatabase.LoadAssetAtPath<LevelTagData>(tagDataPath);

            if (data)
            {
                return;
            }

            data = CreateInstance<LevelTagData>();

            AssetDatabase.CreateAsset(data, tagDataPath);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Tools/Level Tags")]
    public static void ShowEditor()
    {
        EditorWindow wnd = GetWindow<LevelTagWindow>();
        wnd.titleContent = new GUIContent("Level Tags");
    }
    public void OnGUI()
    {
        PreGUI();

        PostGUI();
    }
}
