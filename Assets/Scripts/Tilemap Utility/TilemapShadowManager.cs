using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
[RequireComponent(typeof(CompositeShadowCaster2D))]
[DisallowMultipleComponent]
public class TilemapShadowManager : MonoBehaviour
{
    [SerializeField] private CompositeCollider2D[] sourceColliders = { };

    private static BindingFlags accessFlagsPrivate =
        BindingFlags.NonPublic | BindingFlags.Instance;

    private static FieldInfo shapePathField =
        typeof(ShadowCaster2D).GetField("m_ShapePath", accessFlagsPrivate);

    private static FieldInfo meshHashField =
        typeof(ShadowCaster2D).GetField("m_ShapePathHash", accessFlagsPrivate);

    public void RefreshWorldShadows()
    {
        ClearChildShadows();
        foreach (var source in sourceColliders)
        {
            CreateGroupsForCollider(source);
        }
    }

    private void ClearChildShadows()
    {
        var doomed = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.GetComponent<ShadowCaster2D>() == null || !child.name.StartsWith("_caster_"))
                continue;
            doomed.Add(transform.GetChild(i));
        }

        foreach (var child in doomed)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private void CreateGroupsForCollider(CompositeCollider2D source)
    {
        List<Vector3> totalPath = new List<Vector3>();
        for (int i = 0; i < source.pathCount; i++)
        {
            // get the path data
            Vector2[] pathVertices = new Vector2[source.GetPathPointCount(i)];
            source.GetPath(i, pathVertices);
            Vector3[] finalVerts = Array.ConvertAll<Vector2, Vector3>(pathVertices, input => input);

            totalPath.AddRange(finalVerts);
        }

        // make a new child
        var shadowCaster = new GameObject("_caster_" + 0 + "_" + source.transform.name);
        shadowCaster.transform.parent = transform;

        // create & prime the shadow caster
        var shadow = shadowCaster.AddComponent<ShadowCaster2D>();
        shadow.selfShadows = true;
        shapePathField.SetValue(shadow, totalPath.ToArray());
        // invalidate the hash so it re-generates the shadow mesh on the next Update()
        meshHashField.SetValue(shadow, -1);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(TilemapShadowManager))]
class TilemapShadowManagerEditor : Editor
{
    SerializedProperty tilemapColliderProp;

    private void Awake()
    {
        tilemapColliderProp = serializedObject.FindProperty("sourceColliders");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((TilemapShadowManager)target), typeof(TilemapShadowManager), false);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(tilemapColliderProp);

        TilemapShadowManager tsm = target as TilemapShadowManager;

        bool generate = GUILayout.Button("Generate Shadow Casters");

        if (generate)
        {
            tsm.RefreshWorldShadows();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
