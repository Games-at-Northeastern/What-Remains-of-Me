using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UtilityData;

public class HiddenAreaTM : MonoBehaviour
{
    public class TileData
    {
        public Vector3Int pos;
        public TileBase tile;
        public TileData(Vector3Int pos, TileBase tile)
        {
            this.pos = pos;
            this.tile = tile;
        }
    }

    [System.Serializable]
    private class DetailAdd
    {
        [SerializeField] public TilemapData detailMap;
        [SerializeField] public TilemapData addedDetails;
    }

    [SerializeField] private List<Color> outlineLayers;
    [SerializeField] private List<DetailAdd> detailMaps;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float enterMapDistance;
    [SerializeField] private bool triggerOnContact;
    [SerializeField] private TilemapData groundMap;

    // references

    [SerializeField] private TilemapData hiddenMap;
    [SerializeField] private Tile defaultTile;
    private CompositeCollider2D cc2d;

    private bool triggered = false;
    private bool faded = false;

    private class SpriteTree : BaseSpriteTree
    {
        private readonly Texture2D tex;

        private readonly Rect rect;

        private float ppu;

        public Sprite BakedSprite { get; set; }
        public Tile BakedTile { get; set; }

        public SpriteTree(Texture2D tex, float ppu, Rect rect) : base()
        {
            this.tex = tex;
            this.ppu = ppu;
            this.rect = rect;

            BakedSprite = null;
            BakedTile = null;
        }

        public void BakeSprite(Vector3 anchor)
        {
            BakedSprite = Sprite.Create(
                tex,
                rect,
                anchor,
                ppu);

            BakedSprite.name = GetHashCode().ToString();
        }

        public void BakeTile(Vector3 anchor, Tile tile)
        {
            BakeSprite(anchor);

            BakedTile = Instantiate(tile);
            BakedTile.colliderType = Tile.ColliderType.Sprite;
            BakedTile.sprite = BakedSprite;
            BakedTile.name = "SpriteTile" + GetHashCode().ToString();
        }

        public override SpriteTree Add((Sprite sprite, Matrix4x4 matrix) spriteData, Color tint, Texture2D newTex = null)
        {
            if (Overlays.ContainsKey((spriteData.sprite, spriteData.matrix)))
            {
                return Overlays[spriteData];
            }

            if (newTex is null)
            {
                newTex = BakeTexture(spriteData.sprite, spriteData.matrix, tint);
            }

            Texture2D oldTex = CloneTexture(tex);
            OverlayTexture(oldTex, newTex);

            SpriteTree newTree = new SpriteTree(oldTex, ppu, rect);

            Overlays.Add(spriteData, newTree);
            return Overlays[spriteData];
        }

        private Texture2D CloneTexture(Texture2D tex)
        {
            Texture2D result = new Texture2D(tex.width, tex.height)
            {
                filterMode = tex.filterMode
            };
            result.SetPixels32(tex.GetPixels32());
            result.Apply();
            return result;
        }

        private void OverlayTexture(Texture2D baseTex, Texture2D addedTex)
        {
            Color[] basePixels = baseTex.GetPixels();
            Color[] addedPixels = addedTex.GetPixels();

            Color[] resultPixels = new Color[basePixels.Length];

            int baseCenterX = Mathf.FloorToInt(baseTex.width / 2);
            int baseCenterY = Mathf.FloorToInt(baseTex.height / 2);

            int addedCenterX = Mathf.FloorToInt(addedTex.width / 2);
            int addedCenterY = Mathf.FloorToInt(addedTex.height / 2);

            for (int i = 0; i < basePixels.Length; i++)
            {
                int x = i % baseTex.width;
                int y = i / baseTex.width;

                int xOffFromCenter = x - baseCenterX;
                int yOffFromCenter = y - baseCenterY;

                int xOnAdded = addedCenterX + xOffFromCenter;
                int yOnAdded = addedCenterY + yOffFromCenter;

                if (xOnAdded < 0 || xOnAdded >= addedTex.width ||
                    yOnAdded < 0 || yOnAdded >= addedTex.height)
                {
                    resultPixels[i] = new Color(basePixels[i].r, basePixels[i].g, basePixels[i].b, basePixels[i].a);
                    continue;
                }

                int aI = xOnAdded + (yOnAdded * addedTex.width);

                float frontA = addedPixels[aI].a;
                float backA = basePixels[i].a;

                float resultA = frontA + (backA * (1 - frontA));

                float resultR = ((addedPixels[aI].r * frontA) + (basePixels[i].r * (backA * (1 - frontA)))) / resultA;
                float resultG = ((addedPixels[aI].g * frontA) + (basePixels[i].g * (backA * (1 - frontA)))) / resultA;
                float resultB = ((addedPixels[aI].b * frontA) + (basePixels[i].b * (backA * (1 - frontA)))) / resultA;

                resultPixels[i] = new Color(resultR, resultG, resultB, resultA);
            }

            baseTex.SetPixels(0, 0, baseTex.width, baseTex.height, resultPixels);
            baseTex.Apply();
        }
    }
    private class BaseSpriteTree
    {
        public class SpriteTreeComparer : IEqualityComparer<(Sprite sprite, Matrix4x4 matrix)>
        {
            public bool Equals((Sprite sprite, Matrix4x4 matrix) x, (Sprite sprite, Matrix4x4 matrix) y) => GetHashCode(x) == GetHashCode(y);

            public int GetHashCode((Sprite sprite, Matrix4x4 matrix) obj) => AppendInts(obj.sprite.GetHashCode(), GetMtxHash(obj.matrix));

            private int GetMtxHash(Matrix4x4 matrix) => int.Parse(UseMtx(matrix, new Vector3(1, 2, 0)) + UseMtx(matrix, new Vector3(-3, 2, 0)) + UseMtx(matrix, new Vector3(3, -4, 0)));

            private string UseMtx(Matrix4x4 matrix, Vector3 pos)
            {
                Vector3 transformPos = matrix.MultiplyPoint(pos);
                Vector3Int floored = Vector3Int.FloorToInt(transformPos);
                return MapToPositive(floored.x).ToString() + "0" + MapToPositive(floored.y).ToString();
            }

            private int MapToPositive(int num)
            {
                if (num < 0)
                {
                    return num * -2;
                }
                return 1 + ((num - 1) * 2);
            }

            private int AppendInts(int a, int b)
            {
                int digitsB = Mathf.FloorToInt(Mathf.Log10(b)) + 1;
                return (a * 10 * digitsB) + b;
            }
        }

        public Dictionary<(Sprite, Matrix4x4), SpriteTree> Overlays { get; set; }

        public BaseSpriteTree() => Overlays = new Dictionary<(Sprite, Matrix4x4), SpriteTree>(new SpriteTreeComparer());

        public SpriteTree Add(Tilemap fromTM, Vector3Int position)
        {
            (Sprite sprite, Matrix4x4 matrix) spriteData = (fromTM.GetSprite(position), fromTM.GetTransformMatrix(position));
            return Add(spriteData, fromTM.color * fromTM.GetColor(position));
        }

        public virtual SpriteTree Add((Sprite sprite, Matrix4x4 matrix) spriteData, Color tint, Texture2D newTex = null)
        {
            if (Overlays.ContainsKey((spriteData.sprite, spriteData.matrix)))
            {
                return Overlays[spriteData];
            }

            if (newTex is null)
            {
                newTex = BakeTexture(spriteData.sprite, spriteData.matrix, tint);
            }

            SpriteTree newTree = new SpriteTree(newTex, spriteData.sprite.pixelsPerUnit, new Rect(0, 0, spriteData.sprite.rect.width, spriteData.sprite.rect.height));

            Overlays.Add(spriteData, newTree);
            return Overlays[spriteData];
        }

        public SpriteTree Add(Tilemap fromTM, Vector3Int position, List<Vector3Int> blankOffsets, List<Color> outlineColors, Color tint)
        {
            Texture2D tex = CopyTileTexture(fromTM, position, tint);
            StripTextureOutline(tex, blankOffsets, outlineColors);
            Sprite oldSprite = fromTM.GetSprite(position);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, oldSprite.rect.width, oldSprite.rect.height), oldSprite.pivot, oldSprite.pixelsPerUnit);

            return Add((sprite, fromTM.GetTransformMatrix(position)), tint, tex);
        }

        protected Texture2D BakeTexture(Sprite sprite, Matrix4x4 matrix, Color tint)
        {
            Texture2D spriteTex = GetSlicedSpriteTexture(sprite);

            TintTexture(spriteTex, tint);

            if (matrix == Matrix4x4.identity)
            {
                return spriteTex;
            }
            ApplyMatrixToTexture(spriteTex, matrix);
            return spriteTex;
        }

        protected void TintTexture(Texture2D tex, Color tint)
        {
            Color[] pixels = tex.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = pixels[i] * tint;
            }

            tex.SetPixels(pixels);
            tex.Apply();
        }

        protected void ApplyMatrixToTexture(Texture2D tex, Matrix4x4 matrix)
        {
            Color[] pixels = tex.GetPixels();
            Color[] newPixels = new Color[pixels.Length];

            Vector3 centerOffset = new Vector3(
                (tex.width - 1) / 2f,
                (tex.height - 1) / 2f,
                0);

            for (int i = 0; i < pixels.Length; i++)
            {
                int x = i % tex.width;
                int y = i / tex.width;

                Vector3 transposedPointFloat = matrix.MultiplyPoint3x4(
                        new Vector3(x - centerOffset.x, y - centerOffset.y, 0));
                Vector3Int transposedPoint = Vector3Int.RoundToInt(transposedPointFloat + centerOffset);


                newPixels[transposedPoint.x + (transposedPoint.y * tex.width)] =
                    pixels[i];
            }

            tex.SetPixels(newPixels);
            tex.Apply();
        }

        protected Texture2D GetSlicedSpriteTexture(Sprite sprite)
        {
            Rect rect = sprite.rect;
            Texture2D slicedTex = new Texture2D((int)rect.width, (int)rect.height)
            {
                filterMode = sprite.texture.filterMode
            };

            slicedTex.SetPixels(0, 0, (int)rect.width, (int)rect.height, sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
            slicedTex.Apply();

            return slicedTex;
        }

        protected Texture2D CopyTileTexture(Tilemap tm, Vector3Int position, Color tint) => BakeTexture(tm.GetSprite(position), tm.GetTransformMatrix(position), tint);

        protected void StripTextureOutline(Texture2D sprite, List<Vector3Int> blankOffsets, List<Color> outlineColors)
        {
            Color[] pixels = sprite.GetPixels();
            foreach (Color color in outlineColors)
            {
                for (int x = 0; x < sprite.width; x++)
                {
                    for (int y = 0; y < sprite.height; y++)
                    {
                        if (!CheckForBlankSpaceSprite(x, y, sprite, blankOffsets))
                        {
                            continue;
                        }

                        if (ColorEqualApprox(pixels[x + (y * sprite.width)], color))
                        {
                            pixels[x + (y * sprite.width)] = new Color(1, 1, 1, 0);
                        }
                    }
                }
                sprite.SetPixels(pixels);
            }
            sprite.Apply();
        }

        protected bool ColorEqualApprox(Color a, Color b)
        {
            var res = true;

            res &= Mathf.Abs(a.r - b.r) < (2f / 255f);
            res &= Mathf.Abs(a.g - b.g) < (2f / 255f);
            res &= Mathf.Abs(a.b - b.b) < (2f / 255f);
            res &= Mathf.Abs(a.a - b.a) < (2f / 255f);

            return res;
        }

        protected bool CheckForBlankSpaceSprite(int x, int y, Texture2D tex, List<Vector3Int> blankOffsets)
        {
            foreach (Vector3Int blankOffset in blankOffsets)
            {
                int checkX = x + blankOffset.x;
                int checkY = y + blankOffset.y;

                if (checkX < 0 || checkX >= tex.width)
                {
                    return true;
                }

                if (checkY < 0 || checkY >= tex.height)
                {
                    return true;
                }

                if (tex.GetPixel(checkX, checkY).a == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
    private class TileSpriteStorage
    {
        private Dictionary<Vector3Int, SpriteTree> positionToSpriteTree;
        private BaseSpriteTree spriteTree;

        private Vector3 anchor;

        public TileSpriteStorage(Tilemap tm)
        {
            anchor = tm.tileAnchor;
            positionToSpriteTree = new Dictionary<Vector3Int, SpriteTree>();
            spriteTree = new BaseSpriteTree();
        }

        public void AddSprite(Tilemap fromTM, Vector3Int position)
        {
            if (!positionToSpriteTree.ContainsKey(position))
            {
                positionToSpriteTree[position] = spriteTree.Add(fromTM, position);
                return;
            }

            positionToSpriteTree[position] = positionToSpriteTree[position].Add(fromTM, position);
        }

        public void AddSpriteWithOutline(Tilemap fromTM, Vector3Int position, List<Vector3Int> blankOffsets, List<Color> outlineColors)
        {
            Color tint = fromTM.GetColor(position) * fromTM.color;

            if (!positionToSpriteTree.ContainsKey(position))
            {
                positionToSpriteTree[position] = spriteTree.Add(fromTM, position, blankOffsets, outlineColors, tint);
                return;
            }

            positionToSpriteTree[position] = positionToSpriteTree[position].Add(fromTM, position, blankOffsets, outlineColors, tint);
        }

         public Dictionary<Vector3Int, Tile> BakeTiles(Tile defaultTile)
        {
            var positionToSprite = new Dictionary<Vector3Int, Tile>();

            foreach (Vector3Int position in positionToSpriteTree.Keys)
            {
                if (positionToSpriteTree[position].BakedTile is null)
                {
                    positionToSpriteTree[position].BakeTile(anchor, defaultTile);
                }
                positionToSprite[position] = positionToSpriteTree[position].BakedTile;
            }

            return positionToSprite;
        }
    }

    private void FixedUpdate()
    {
        if (triggered && !faded)
        {
            hiddenMap.TMap.color = new Color(
                hiddenMap.TMap.color.r,
                hiddenMap.TMap.color.g, hiddenMap.TMap.color.b,
                Mathf.Max(0, hiddenMap.TMap.color.a - (fadeSpeed * Time.deltaTime)));

            if (hiddenMap.TMap.color.a < 0.001f)
            {
                faded = true;
                hiddenMap.Destroy();
            }
        }
    }

    private void Start()
    {
        if (!triggerOnContact)
        {
            GetComponent<CompositeCollider2D>().enabled = false;
            GetComponent<TilemapCollider2D>().enabled = false;
        }

        cc2d = GetComponent<CompositeCollider2D>();

        hiddenMap.Enable();

        foreach (DetailAdd detailAdd in detailMaps)
        {
            detailAdd.addedDetails.Enable();
        }

        // grab edge positions
        // send outline tiles from ground to hidden

        Vector3IntSet hideMapPositions = SendBorderUp(groundMap.TMap, hiddenMap.TMap, out var edgePositions);

        // send second outline layer to hidden
        SendBorderUp(groundMap.TMap, hiddenMap.TMap);

        hiddenMap.TMap.RefreshAllTiles();

        // Handle sprite tile creation

        var spriteData = new TileSpriteStorage(hiddenMap.TMap);

        foreach (Vector3Int position in hideMapPositions)
        {
            if (edgePositions.TryGetValue(position, out var blankOffsets))
            {
                spriteData.AddSpriteWithOutline(hiddenMap.TMap, position, blankOffsets, outlineLayers);
                continue;
            }

            spriteData.AddSprite(hiddenMap.TMap, position);
        }

        for (int i = detailMaps.Count - 1; i > -1; i--)
        {
            Vector3IntSet detailPositions = SendBorderUp(detailMaps[i].detailMap.TMap, detailMaps[i].addedDetails.TMap);

            SendBorderUp(detailMaps[i].detailMap.TMap, detailMaps[i].addedDetails.TMap);

            foreach (Vector3Int position in detailPositions)
            {
                spriteData.AddSprite(detailMaps[i].addedDetails.TMap, position);
            }

            detailMaps[i].addedDetails.Destroy();
        }

        Dictionary<Vector3Int, Tile> positionToTile = spriteData.BakeTiles(defaultTile);

        // replace tiles with sprite tiles

        hiddenMap.TMap.ClearAllTiles();

        foreach (Vector3Int position in positionToTile.Keys)
        {
            hiddenMap.TMap.SetTile(position, positionToTile[position]);
        }

        // hide added detail maps

        foreach (DetailAdd detailAdd in detailMaps)
        {
            detailAdd.addedDetails.Disable();
        }
    }

    public Vector3IntSet SendBorderUp(Tilemap fromMap, Tilemap toMap) => DoSendBorderUp(fromMap, toMap, TMPositions.ClassifyTiles(toMap, true));
    public Vector3IntSet SendBorderUp(Tilemap fromMap, Tilemap toMap, out Dictionary<Vector3Int, List<Vector3Int>> edges)
    {
        var toMapData = TMPositions.ClassifyTiles(toMap, true);

        edges = new Dictionary<Vector3Int, List<Vector3Int>>();

        foreach (Vector3Int position in toMapData.TilePositions)
        {
            if (IsEdgeOfArea(position, toMap, fromMap, out var blankOffsets))
            {
                edges.Add(position, blankOffsets);
            }
        }

        return DoSendBorderUp(fromMap, toMap, toMapData);
    }

    public Vector3IntSet DoSendBorderUp(Tilemap fromMap, Tilemap toMap, TMPositions.ClassifyResult toMapData)
    {
        var usedPositions = new Vector3IntSet(toMapData.TilePositions);

        foreach (Vector3Int position in toMapData.BorderPositions)
        {
            if (!fromMap.HasTile(position))
            {
                continue;
            }

            toMap.SetTile(position, fromMap.GetTile(position));
            usedPositions.Add(position);
        }

        return usedPositions;
    }

    // If hiddenTM tile is a connecting space to outside the hidden area, returns
    // the offset to the blank tile position next to the given pos
    // Otherwise, returns (0, 0, 0)
    public bool IsEdgeOfArea(Vector3Int pos, Tilemap hiddenTM, Tilemap groundTM, out List<Vector3Int> edgeOffsets)
    {
        bool isEdge = false;
        edgeOffsets = new List<Vector3Int>();
        foreach (Vector3Int offset in UData.NeighbourAdjacents())
        {
            if (!hiddenTM.HasTile(pos + offset) && !groundTM.HasTile(pos + offset))
            {
                edgeOffsets.Add(offset);
                isEdge = true;
            }
        }

        foreach (Vector3Int offset in UData.NeighbourDiagonals())
        {
            Vector3Int dPos = pos + offset;

            if (!hiddenTM.HasTile(dPos) && !groundTM.HasTile(dPos))
            {
                int hiddenAdjCount = 0;
                foreach(Vector3Int off2 in UData.NeighbourAdjacents())
                {
                    if (hiddenTM.HasTile(dPos + off2))
                    {
                        hiddenAdjCount++;
                    }
                }
                if (hiddenAdjCount > 1)
                {
                    edgeOffsets.Add(dPos);
                    isEdge = true;
                }
            }
        }

        return isEdge;
    }

    public List<Vector3Int> GetRuleTileSpaces(Vector3Int pos, Tilemap tm)
    {
        List<Vector3Int> res = new List<Vector3Int>();
        foreach (Vector3Int offset in UData.NeighbourAdjacentDiagonals())
        {
            Vector3Int check = pos + offset;

            TileBase tile = tm.GetTile(check);


            if (tile is RuleTile)
            {
                res.Add(check);
            }
        }

        return res;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !triggered)
        {
            if (Physics2D.Distance(cc2d, collision).distance < -1 * enterMapDistance)
            {
                TriggerArea();
                cc2d.enabled = false;
                GetComponent<TilemapCollider2D>().enabled = false;
            }
        }
    }

    public void TriggerArea() => triggered = true;
}

#if UNITY_EDITOR
[CustomEditor(typeof(HiddenAreaTM))]
public class HiddenAreaTMEditor : Editor
{
    private SerializedProperty outlineLayersProp;
    private SerializedProperty detailMapsProp;
    private SerializedProperty fadeSpeedProp;
    private SerializedProperty enterMapDistanceProp;
    private SerializedProperty triggerOnContactProp;
    private SerializedProperty groundMapProp;
    private SerializedProperty hiddenMapProp;
    private SerializedProperty defaultTileProp;

    private bool showSettings = true;
    private bool showReferences = false;

    public void Awake()
    {
        outlineLayersProp = serializedObject.FindProperty("outlineLayers");
        detailMapsProp = serializedObject.FindProperty("detailMaps");
        fadeSpeedProp = serializedObject.FindProperty("fadeSpeed");
        enterMapDistanceProp = serializedObject.FindProperty("enterMapDistance");
        triggerOnContactProp = serializedObject.FindProperty("triggerOnContact");
        groundMapProp = serializedObject.FindProperty("groundMap");

        hiddenMapProp = serializedObject.FindProperty("hiddenMap");
        defaultTileProp = serializedObject.FindProperty("defaultTile");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

        showSettings = EditorGUILayout.Foldout(showSettings, "Settings");

        if (showSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(fadeSpeedProp);
            EditorGUILayout.PropertyField(triggerOnContactProp);
            if (triggerOnContactProp.boolValue)
            {
                EditorGUILayout.PropertyField(enterMapDistanceProp);
            }

            EditorGUILayout.PropertyField(outlineLayersProp);
            EditorGUILayout.PropertyField(detailMapsProp);

            EditorGUILayout.PropertyField(groundMapProp);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        showReferences = EditorGUILayout.Foldout(showReferences, "References");

        if (showReferences)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(hiddenMapProp);
            EditorGUILayout.PropertyField(defaultTileProp);

            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

[System.Serializable]
public class TilemapData
{
    private Tilemap tilemap;
    public Tilemap TMap
    {
        get
        {
            if (!tilemap)
            {
                tilemap = tilemapObject.GetComponent<Tilemap>();
            }
            return tilemap;
        }
        set => tilemap = value;
    }
    private TilemapRenderer renderer;
    public TilemapRenderer Renderer
    {
        get
        {
            if (!renderer)
            {
                renderer = tilemapObject.GetComponent<TilemapRenderer>();
            }
            return renderer;
        }
        set => renderer = value;
    }

    [SerializeField] private GameObject tilemapObject;

    public static TilemapData Construct(GameObject mapObjectPrefab)
    {
        TilemapData newData = new TilemapData();

        GameObject tilemapObject = Object.Instantiate(mapObjectPrefab);

        newData.tilemapObject = tilemapObject;
        newData.tilemap = newData.tilemapObject.GetComponent<Tilemap>();
        newData.renderer = newData.tilemapObject.GetComponent<TilemapRenderer>();

        return newData;
    }

    public TilemapData()
    {
        tilemap = null;
        renderer = null;
        tilemapObject = null;
    }

    public void Enable() => SetEnabled(true);
    public void Disable() => SetEnabled(false);

    public void SetEnabled(bool enabled)
    {
        TMap.enabled = enabled;
        Renderer.enabled = enabled;
    }

    public void Destroy() => Object.Destroy(tilemapObject);
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TilemapData))]
public class TilemapDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label.text = property.displayName;
        EditorGUI.ObjectField(position, property.FindPropertyRelative("tilemapObject"), typeof(GameObject), label);
    }
}
#endif
