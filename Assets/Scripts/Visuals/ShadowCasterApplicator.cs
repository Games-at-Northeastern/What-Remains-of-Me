#if UNITY_EDITOR
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

/// <summary>
/// Adds shadow caster 2Ds with the same shape as this game object's tilemap 2D component.
/// Shadow casters are broken down into sections of user-specified width.
/// Creates or adds a ShadowCasterDisabler component to this game object upon generating
///   shadowcasters. 
/// </summary>
[RequireComponent(typeof(Tilemap))]
public class ShadowCasterApplicator : MonoBehaviour
{
    [SerializeField] private int shadowCasterWidth = 20;

    private int shadowCasterCount = 0;
    private Grid grid;
    private Tilemap tilemap;
    private Tilemap tempTilemap;
    private CompositeCollider2D tempCompositeCollider;
    private TilemapCollider2D tempTilemapCollider;
    private ShadowCasterDisabler disabler;

    static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);

    public void Create()
    {
        if (!TryGetComponent(out tilemap))
        {
            return;
        }
        if (!transform.parent.TryGetComponent(out grid))
        {
            return;
        }
        if (!TryGetComponent(out disabler))
        {
            disabler = gameObject.AddComponent<ShadowCasterDisabler>();
        }

        DestroyShadowcasters();
        shadowCasterCount = 0;

        tilemap.CompressBounds();
        int startX = tilemap.cellBounds.xMin;

        while (startX < tilemap.cellBounds.xMax)
        {
            InstantiateTempTilemap();

            for (int x = startX; x < Mathf.Min(startX + shadowCasterWidth, tilemap.cellBounds.xMax); x++)
            {
                for (int y = tilemap.cellBounds.yMin; y <= tilemap.cellBounds.yMax; y++)
                {
                    Vector3Int target = new(x, y);
                    TileBase tile = tilemap.GetTile(target);
                    if (!tile)
                        continue;
                    tempTilemap.SetTile(target, tile);
                }
            }

            tempTilemap.CompressBounds();
            tempTilemapCollider.ProcessTilemapChanges();
            GenerateShadowCasters(tempCompositeCollider);

            DestroyTempTilemap();

            startX += shadowCasterWidth;
        }
    }

    private void InstantiateTempTilemap()
    {
        GameObject tempGO = new GameObject("Temp");
        tempTilemap = tempGO.AddComponent<Tilemap>();

        tempTilemapCollider = tempGO.AddComponent<TilemapCollider2D>();
        tempTilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
        tempGO.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        tempCompositeCollider = tempGO.AddComponent<CompositeCollider2D>();

        tempGO.transform.parent = grid.transform;
        tempGO.transform.SetLocalPositionAndRotation(tilemap.transform.localPosition, tilemap.transform.localRotation);
        tempGO.transform.localScale = tilemap.transform.localScale;
    }

    private void DestroyTempTilemap()
    {
        DestroyImmediate(tempTilemap.gameObject);
        tempTilemap = null;
        tempTilemapCollider = null;
        tempCompositeCollider = null;
    }

    private void GenerateShadowCasters(CompositeCollider2D collider)
    {
        for (int i = 0; i < collider.pathCount; i++)
        {
            GameObject shadowCaster = new GameObject("shadow_caster_" + shadowCasterCount);
            shadowCaster.transform.parent = gameObject.transform;
            shadowCasterCount++;

            ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
            shadowCasterComponent.selfShadows = false;

            Vector2[] pathVertices = new Vector2[collider.GetPathPointCount(i)];
            collider.GetPath(i, pathVertices);

            Vector3[] testPath = new Vector3[pathVertices.Length];
            for (int j = 0; j < pathVertices.Length; j++)
            {
                testPath[j] = pathVertices[j];
            }

            shapePathField.SetValue(shadowCasterComponent, testPath);
            shapePathHashField.SetValue(shadowCasterComponent, Random.Range(int.MinValue, int.MaxValue));

            if (disabler)
            {
                disabler.AddDisableableComponent(shadowCasterComponent);
            }
        }
    }

    public void DestroyShadowcasters()
    {
        var children = transform.Cast<Transform>().ToList();
        foreach (var child in children)
        {
            if (child.GetComponent<ShadowCaster2D>())
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}


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
        if (GUILayout.Button("Remove Shadows"))
        {
            var creator = (ShadowCasterApplicator)target;
            creator.DestroyShadowcasters();
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif