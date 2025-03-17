using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR

public class ShadowCasterApplicator : MonoBehaviour
{
    [SerializeField] private bool selfShadows = true;
    private CompositeCollider2D tilemapCollider;

    //static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
									.Assembly
									.GetType("UnityEngine.Rendering.Universal.ShadowUtility")
									.GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

    public void Create()
    {
        //Debug.Log(meshField);
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
            //meshField.SetValue(shadowCasterComponent, new Mesh());
            //generateShadowMeshMethod.Invoke(shadowCasterComponent, new object[] { new Mesh(), shapePathField.GetValue(shadowCasterComponent)});
            //meshField.GetValue(shadowCasterComponent)
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
}

[CustomEditor (typeof(ShadowCasterApplicator))]
public class ShadowCaster2DTileMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create"))
        {
            var creator = (ShadowCasterApplicator) target;
            creator.Create();
        }
        if (GUILayout.Button("Remove shadows"))
        {
            var creator = (ShadowCasterApplicator) target;
            creator.DestroyOldShadowCasters();
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
