using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ShadowCasterApplicator : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool selfShadows = true;
#endif
    private CompositeCollider2D tilemapCollider;

    static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
                                .Assembly
                                .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
                                .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

    private void Awake()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            Destroy(this);
        }
        else
        {
            hideFlags = HideFlags.DontSave;
        }
#endif
    }

#if UNITY_EDITOR
    public void Create()
    {
        DestroyOldShadowCasters();
        tilemapCollider = GetComponent<CompositeCollider2D>();

        for (int i = 0; i < tilemapCollider.pathCount; i++)
        {
            Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
            tilemapCollider.GetPath(i, pathVertices);
            GameObject shadowCaster = new GameObject("shadow_caster_" + i);
            shadowCaster.transform.parent = gameObject.transform;
            ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
            shadowCasterComponent.selfShadows = this.selfShadows;

            Vector3[] testPath = new Vector3[pathVertices.Length];
            for (int j = 0; j < pathVertices.Length; j++)
            {
                testPath[j] = pathVertices[j];
            }

            shapePathField.SetValue(shadowCasterComponent, testPath);
            shapePathHashField.SetValue(shadowCasterComponent, Random.Range(int.MinValue, int.MaxValue));
        }
    }

    public void DestroyOldShadowCasters()
    {
        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            DestroyImmediate(child.gameObject);
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(ShadowCasterApplicator))]
public class ShadowCaster2DTileMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create"))
        {
            var creator = (ShadowCasterApplicator)target;
            creator.Create();
        }
        if (GUILayout.Button("Remove shadows"))
        {
            var creator = (ShadowCasterApplicator)target;
            creator.DestroyOldShadowCasters();
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
