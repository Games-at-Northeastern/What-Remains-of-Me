using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;

public class ShadowCasterDisabler : PlayerDistanceComponentDisabler<ShadowCaster2D>
{
    static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);

    protected override Bounds GetComponentBounds(ShadowCaster2D shadowCaster)
    {
        Vector3[] shapePath = shapePathField.GetValue(shadowCaster) as Vector3[];
        Bounds bounds = new Bounds(shadowCaster.transform.TransformPoint(shapePath[0]), Vector3.zero);
        for (int i = 1; i < shapePath.Length; i++)
        {
            bounds.Encapsulate(shadowCaster.transform.TransformPoint(shapePath[i]));
        }
        return bounds;
    }

    public void AddAllSceneComponents()
    {
        components.Clear();
        components.AddRange(FindObjectsOfType<ShadowCaster2D>(true));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ShadowCasterDisabler))]
public class ShadowCasterDisablerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Add all shadow casters in scene"))
        {
            var disabler = (ShadowCasterDisabler)target;
            disabler.AddAllSceneComponents();
        }
    }
}
#endif
