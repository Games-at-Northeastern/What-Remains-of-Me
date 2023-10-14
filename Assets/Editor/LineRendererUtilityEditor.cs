using Levels.Objects;
using UnityEditor;
using UnityEngine;

/// <summary>
/// An editor script for line rendering utilities.
/// </summary>
[CustomEditor(typeof(LineRendererUtility))]
public class LineRendererUtilityEditor : Editor
{
    private string toEdit = "GameObject Name";

    public override void OnInspectorGUI()
    {
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
