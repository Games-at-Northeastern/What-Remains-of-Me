using Levels.Objects;
using UnityEditor;
using UnityEngine;

/// <summary>
/// An editor script for line rendering utilities.
/// </summary>
[CustomEditor(typeof(LineRendererUtility))]
public class LineRendererUtilityEditor : Editor
{
    private string toEdit = "Name";

    private string text =
@"To use this component, you need to access the Line Renderer Utilty prefab in the prefabs folder. It comes with the script set up with some defaults, as well as the required LineRenderer component.

Use this link to learn more about the tool: https://docs.google.com/document/d/1ZRbZN_NT9Ptd6nZM-eFgRl2IhNvIyZpxumZl12F4Rzo/edit?usp=sharing
Or, go to the line renderer utility scene in the Developer Tutorials folder.
";


    public override void OnInspectorGUI()
    {
        GUILayout.Label(text, EditorStyles.wordWrappedLabel);

        var renderer = (LineRendererUtility)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate", GUILayout.ExpandHeight(true)))
        {
            renderer.Generate();
        }

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset"))
        {
            renderer.ResetPoints();
        }

        if (GUILayout.Button("Stock"))
        {
            renderer.StockPointsFromChildren();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Linearize"))
        {
            renderer.LinearizePoints();
        }

        if (GUILayout.Button("Reduce"))
        {
            renderer.Reduce();
        }

        GUILayout.EndHorizontal();


        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Points To Transforms", GUILayout.ExpandHeight(true)))
        {
            renderer.BakeToTransforms();
        }

        GUILayout.BeginVertical();

        toEdit = GUILayout.TextField(toEdit);

        if (GUILayout.Button("Bake to GameObject"))
        {
            renderer.BakeMeshIntoGameObject(toEdit);
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.Space(25);

        DrawDefaultInspector();
    }
}
