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

There are 7 buttons and a text field:
1. Generate: Using the Required Points you have specified, creates a path through all of them according to current parameters.
2. Reset: Resets all the points on the line renderer. Does not reset the Required Points list.
3. Stock: Overrides the list of Require Points with all the child transforms of this game object. Useful for collecting/reusing transform points.
4. Linearize: If you manually draw points with the line renderer's Scene Tool, this button will straighten out the lines to cardinal directions.
5. Reduce: Attempts to reduce the number of points without ruining the line. Reduce whenever possible.
6. Points to Transforms: Converts the line renderer's points into child Transform game objects. Be sure to Reduce before this step. This step is required before Baking. This overrides the transforms currently in the Required Points list.
7. Bake to GameObject: Using the currently stocked Baked Object Prefab (which must have a component that extends the IRenderableTrail interface), creates a new instance of it with the baked trail. Once you bake, you must perform Points to Transforms again.
The textfield just controls the name of the prefab instance.

Controls:
Cardinaly Only - Do we generate paths only using cardinal segments? If false, directly connects points.
Max Generated Segment Length - Controls how long a segment can be until it is forced to break into another segment. Note that a segment won't always be as long as it can be.
Z Pos - The Z position of the points generated. This is used for rendering order (i.e. which sprite goes over which).
Thickness - How thick the line should be. Thicker lines have visible artifacting with more complicated paths.
Use Funky - Swaps the algorithm to one that is HEAVILY influenced by randomness. Produces wacky (but sometimes cooler) paths.
Length Variance - How much the Max Generated Segment Length randomly varies. Often best left at 0, but feel free to fool around with it.
Random Variance - How often the algorithm will make a stupid segment choice in order to get a more interesting path.
Delta Min - The minimum dx/dy of a segment where the Random Variance can affect it. Exists to prevent divisions by 0.
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
