using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.Rendering.Universal;

// the #if ensures the script is not compiled when making a build, because it is only meant to work in the Editor.
// Once the casters are applied, no need to compile this script with the rest.
#if UNITY_EDITOR

public class ShadowCasterApplicator : MonoBehaviour
{
    [SerializeField] private bool selfShadows = true;
    private CompositeCollider2D tilemapCollider;

	static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
	static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
									.Assembly
									.GetType("UnityEngine.Rendering.Universal.ShadowUtility")
									.GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

    public void Create()
    {
        DestroyOldShadowCasters();
        tilemapCollider = GetComponent<CompositeCollider2D>();

        // for all paths in the collider, which I think of as different seperated chunks, it creates a new GameObject, makes it a child of this tilemap, and adds a ShadowCaster2D component to it
        for (int i = 0; i < tilemapCollider.pathCount; i++)
        {
            Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
            tilemapCollider.GetPath(i, pathVertices);
            GameObject shadowCaster = new GameObject("shadow_caster_" + i);
            shadowCaster.transform.parent = gameObject.transform;
            ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
            shadowCasterComponent.selfShadows = this.selfShadows;

            // shapes the new shadow caster to fit this piece of tilemap geometry
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
}

// This is the GUI that appears in the Inspector, allowing for the 'Create' and 'Remove shadows' buttons.
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
