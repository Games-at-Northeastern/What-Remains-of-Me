
/*
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;


[CustomEditor(typeof(TestMovingObjectScript))]
[CanEditMultipleObjects]
public class CustomEditorTestMovingObject : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Run the GUI for the base first
        TestMovingObjectScript movingObjectScript = (TestMovingObjectScript)target;
        float platformNode = movingObjectScript.platformStartLocation;

        SplineContainer splineContainer = movingObjectScript.GetComponent<SplineContainer>();
        int splineKnots = splineContainer.Spline.Knots.Count();

        EditorGUILayout.BeginHorizontal("box", GUILayout.Height(20), GUILayout.ExpandWidth(true));

        EditorGUILayout.LabelField("Platform Starting Node:", EditorStyles.boldLabel, GUILayout.Width(150));
        float slider = EditorGUILayout.Slider(platformNode, 0, splineKnots);
        movingObjectScript.platformStartLocation = slider;


        EditorGUILayout.EndHorizontal();

    }
}
*/

