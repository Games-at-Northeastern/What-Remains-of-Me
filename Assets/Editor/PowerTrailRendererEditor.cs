using System.Collections;
using System.Collections.Generic;
using Levels.Objects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PowerTrailRenderer))]
public class PowerTrailRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var renderer = (PowerTrailRenderer)target;

        if (GUILayout.Button("Generate"))
        {
            renderer.Generate();
        }

        if (GUILayout.Button("Reset Positions"))
        {
            renderer.ResetPoints();
        }

        if (GUILayout.Button("Linearize"))
        {
            renderer.LinearizePoints();
        }

        DrawDefaultInspector();
    }
}
