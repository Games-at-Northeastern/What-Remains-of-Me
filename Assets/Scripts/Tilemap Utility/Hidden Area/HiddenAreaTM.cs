using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UtilityData;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapRenderer))]
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
    public class DetailAdd
    {
        [SerializeField] public TilemapData detailMap;
        [SerializeField] public TilemapData addedDetails;

        public Dictionary<Vector3Int, TileBase> storedOriginalTiles;

        public Dictionary<Vector3Int, TileBase> GetOriginalTiles(bool initialized)
        {
            if (!initialized)
            {
                Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>(new Vector3IntSet.V3IComparer());
                foreach (Vector3Int position in addedDetails.TMap.cellBounds.allPositionsWithin)
                {
                    if (addedDetails.TMap.HasTile(position))
                    {
                        tiles.Add(position, addedDetails.TMap.GetTile(position));
                    }
                }
                return tiles;
            }

            return storedOriginalTiles;
        }
    }

    [SerializeField] private bool removeOutline;
    public bool RemoveOutline { get => removeOutline; set { if (!initialized) { removeOutline = value; } else { throw new System.Exception("too late to change this property"); } } }

    [SerializeField] private List<Color> outlineLayers;
    public List<Color> OutlineLayers { get => outlineLayers; set { if (!initialized) { outlineLayers = value; } else { throw new System.Exception("too late to change this property"); } } }

    [SerializeField] private List<DetailAdd> detailMaps;
    public List<DetailAdd> DetailMaps { get => detailMaps; set { if (!initialized) { detailMaps = value; } else { throw new System.Exception("too late to change this property"); } } }

    [SerializeField] private float fadeSpeed;
    public float FadeSpeed { get => fadeSpeed; set => fadeSpeed = value; }

    [SerializeField] private float enterMapDistance;
    public float EnterMapDistance { get => EnterMapDistance; set => EnterMapDistance = value; }

    [SerializeField] private bool triggerOnContact;
    public bool TriggerOnContact { get => triggerOnContact; set => triggerOnContact = value; }

    [SerializeField] private TilemapData groundMap;
    public TilemapData GetGroundMap() => groundMap;

    [SerializeField] private bool rehideArea;
    public bool RehideArea { get => rehideArea; set { if (!initialized) { rehideArea = value; } else { throw new System.Exception("too late to change this property"); } } }

    // references

    [SerializeField] private TilemapData hiddenMap;
    public TilemapData GetHiddenMap() => hiddenMap;

    private CompositeCollider2D cc2d;

    // advanced

    [SerializeField] private bool initOnStart = false;
    [SerializeField] private List<HiddenAreaTM> adjacentHiddenMaps;
    public List<HiddenAreaTM> AdjacentHiddenMaps { get => adjacentHiddenMaps; set { if (!initialized) { adjacentHiddenMaps = value; } else { throw new System.Exception("too late to change this property"); } } }

    [SerializeField] private List<Vector3Int> ignoreTiles;
    public List<Vector3Int> IgnoreTiles { get => ignoreTiles; set { if (!initialized) { ignoreTiles = value; } else { throw new System.Exception("too late to change this property"); } } }

    private Dictionary<Vector3Int, TileBase> storedOriginalTiles;
    public Dictionary<Vector3Int, TileBase> GetOriginalTiles()
    {
        if (!initialized)
        {
            Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>(new Vector3IntSet.V3IComparer());
            foreach (Vector3Int position in hiddenMap.TMap.cellBounds.allPositionsWithin)
            {
                if (hiddenMap.TMap.HasTile(position))
                {
                    tiles.Add(position, hiddenMap.TMap.GetTile(position));
                }
            }
            return tiles;
        }

        return storedOriginalTiles;
    }

    private bool initialized = false;
    private bool triggered = false;

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

        public void BakeTile(Vector3 anchor)
        {
            BakeSprite(anchor);

            BakedTile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
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

         public Dictionary<Vector3Int, Tile> BakeTiles()
        {
            var positionToSprite = new Dictionary<Vector3Int, Tile>();

            foreach (Vector3Int position in positionToSpriteTree.Keys)
            {
                if (positionToSpriteTree[position].BakedTile is null)
                {
                    positionToSpriteTree[position].BakeTile(anchor);
                }
                positionToSprite[position] = positionToSpriteTree[position].BakedTile;
            }

            return positionToSprite;
        }
    }

    private float defaultTMAlpha = 0;
    private float fadeRatio = 1;
    private void FixedUpdate()
    {
        if (triggered)
        {
            fadeRatio = Mathf.Max(0, fadeRatio - (fadeSpeed * Time.fixedDeltaTime));

            if (fadeRatio <= 0 && !rehideArea)
            {
                hiddenMap.Destroy();
            }
        }
        else
        {
            fadeRatio = Mathf.Min(1, fadeRatio + (FadeSpeed * Time.fixedDeltaTime));
        }

        hiddenMap.TMap.color = new Color(
                hiddenMap.TMap.color.r,
                hiddenMap.TMap.color.g, hiddenMap.TMap.color.b,
                defaultTMAlpha * fadeRatio);
    }

    private void Start()
    {
        if (initOnStart)
        {
            Init();
        }
    }

    public void Init()
    {
        if (initialized)
        {
            throw new System.Exception("Attempting to initialize hidden map which is already initialized");
        }

        cc2d = GetComponent<CompositeCollider2D>();

        hiddenMap.Enable();

        foreach (DetailAdd detailAdd in detailMaps)
        {
            detailAdd.addedDetails.Enable();
        }

        if (adjacentHiddenMaps.Count > 0)
        {
            storedOriginalTiles = new Dictionary<Vector3Int, TileBase>(new Vector3IntSet.V3IComparer());

            foreach (Vector3Int position in hiddenMap.TMap.cellBounds.allPositionsWithin)
            {
                if (!hiddenMap.TMap.HasTile(position))
                {
                    continue;
                }

                storedOriginalTiles[position] = hiddenMap.TMap.GetTile(position);
            }

            foreach (DetailAdd detailAdd in detailMaps)
            {
                detailAdd.storedOriginalTiles = new Dictionary<Vector3Int, TileBase>(new Vector3IntSet.V3IComparer());

                foreach (Vector3Int position in detailAdd.addedDetails.TMap.cellBounds.allPositionsWithin)
                {
                    if (!detailAdd.addedDetails.TMap.HasTile(position))
                    {
                        continue;
                    }

                    detailAdd.storedOriginalTiles[position] = detailAdd.addedDetails.TMap.GetTile(position);
                }
            }
        }

        // grab edge positions
        // send outline tiles from ground to hidden

        Vector3IntSet hideMapPositions = SendGroundBorderUp(groundMap.TMap, hiddenMap.TMap, out var edgePositions, out var adjacentPositions);

        // send second outline layer to hidden
        SendGroundBorderUp(groundMap.TMap, hiddenMap.TMap, out _, out _);

        hiddenMap.TMap.RefreshAllTiles();

        // Handle sprite tile creation

        var spriteData = new TileSpriteStorage(hiddenMap.TMap);

        foreach (Vector3Int position in hideMapPositions)
        {
            if (adjacentPositions.Contains(position))
            {
                continue;
            }

            if (edgePositions.TryGetValue(position, out var blankOffsets) && removeOutline)
            {
                spriteData.AddSpriteWithOutline(hiddenMap.TMap, position, blankOffsets, outlineLayers);
                continue;
            }

            spriteData.AddSprite(hiddenMap.TMap, position);
        }

        for (int i = detailMaps.Count - 1; i > -1; i--)
        {
            Vector3IntSet detailPositions = SendDetailBorderUp(detailMaps[i].detailMap.TMap, detailMaps[i].addedDetails.TMap, out var adjacentDetails);

            SendDetailBorderUp(detailMaps[i].detailMap.TMap, detailMaps[i].addedDetails.TMap, out _);

            foreach (Vector3Int position in detailPositions)
            {
                if (adjacentDetails.Contains(position))
                {
                    continue;
                }

                spriteData.AddSprite(detailMaps[i].addedDetails.TMap, position);
            }

            detailMaps[i].addedDetails.Destroy();
        }

        Dictionary<Vector3Int, Tile> positionToTile = spriteData.BakeTiles();

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

        defaultTMAlpha = hiddenMap.TMap.color.a;
        initialized = true;
    }

    public Vector3IntSet SendDetailBorderUp(Tilemap fromMap, Tilemap toMap, out Vector3IntSet adjacentPositions)
    {
        var toMapData = TMPositions.ClassifyTiles(toMap, true);

        adjacentPositions = new Vector3IntSet();

        var usedPositions = new Vector3IntSet(toMapData.TilePositions);

        foreach (Vector3Int position in toMapData.BorderPositions)
        {
            if (ignoreTiles.Contains(position))
            {
                continue;
            }

            (TileBase tile, bool fromGround) = GetTileFromSurroundDetails(fromMap, position);
            if (tile is null)
            {
                continue;
            }

            toMap.SetTile(position, tile);
            usedPositions.Add(position);

            if (!fromGround)
            {
                adjacentPositions.Add(position);
            }
        }

        return usedPositions;
    }
    public Vector3IntSet SendGroundBorderUp(Tilemap fromMap, Tilemap toMap, out Dictionary<Vector3Int, List<Vector3Int>> edges, out Vector3IntSet adjacentPositions)
    {
        adjacentPositions = new Vector3IntSet();
        var toMapData = TMPositions.ClassifyTiles(toMap, true);

        edges = new Dictionary<Vector3Int, List<Vector3Int>>();

        foreach (Vector3Int position in toMapData.TilePositions)
        {
            if (ignoreTiles.Contains(position))
            {
                continue;
            }

            if (IsEdgeOfArea(position, out var blankOffsets))
            {
                edges.Add(position, blankOffsets);
            }
        }

        var usedPositions = new Vector3IntSet(toMapData.TilePositions);

        foreach (Vector3Int position in toMapData.BorderPositions)
        {
            if (ignoreTiles.Contains(position))
            {
                continue;
            }

            (TileBase tile, bool fromGround) = GetTileFromSurrounds(position);
            if (tile is null)
            {
                continue;
            }

            if (!fromGround)
            {
                adjacentPositions.Add(position);
            }

            toMap.SetTile(position, tile);
            usedPositions.Add(position);
        }

        return usedPositions;
    }

    public (TileBase tile, bool fromGround) GetTileFromSurrounds(Vector3Int pos)
    {
        if (groundMap.TMap.HasTile(pos))
        {
            return (groundMap.TMap.GetTile(pos), true);
        }

        foreach (HiddenAreaTM hatm in adjacentHiddenMaps)
        {
            var otherStored = hatm.GetOriginalTiles();

            if (otherStored.ContainsKey(pos))
            {
                return (otherStored[pos], false);
            }
        }

        return (null, false);
    }

    public (TileBase tile, bool fromGround) GetTileFromSurroundDetails(Tilemap detailmap, Vector3Int pos)
    {
        if (detailmap.HasTile(pos))
        {
            return (detailmap.GetTile(pos), true);
        }

        foreach (HiddenAreaTM hatm in adjacentHiddenMaps)
        {
            Tilemap otherTMap = hatm.hiddenMap.TMap;

            foreach (DetailAdd detailAdd in hatm.detailMaps)
            {
                if (!detailAdd.detailMap.Equals(detailmap))
                {
                    continue;
                }

                TileBase output = detailAdd.storedOriginalTiles[pos];
                if (output is not null)
                {
                    return (output, false);
                }
            }
        }

        return (null, false);
    }

    // If hiddenTM tile is a connecting space to outside the hidden area, returns
    // the offset to the blank tile position next to the given pos
    // Otherwise, returns (0, 0, 0)
    public bool IsEdgeOfArea(Vector3Int pos, out List<Vector3Int> edgeOffsets)
    {
        Tilemap hiddenTM = hiddenMap.TMap;

        bool isEdge = false;
        edgeOffsets = new List<Vector3Int>();
        foreach (Vector3Int offset in UData.NeighbourAdjacents())
        {
            if (!hiddenTM.HasTile(pos + offset) && GetTileFromSurrounds(pos + offset).tile is null)
            {
                edgeOffsets.Add(offset);
                isEdge = true;
            }
        }

        foreach (Vector3Int offset in UData.NeighbourDiagonals())
        {
            Vector3Int dPos = pos + offset;

            if (!hiddenTM.HasTile(dPos) && GetTileFromSurrounds(pos + offset).tile is null)
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
        if (!triggerOnContact)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            float dist = Physics2D.Distance(cc2d, collision).distance;

            if (dist < -1 * enterMapDistance)
            {
                TriggerArea();
                return;
            }

            if (!rehideArea)
            {
                return;
            }

            if (dist > Mathf.Min(-0.05f, (-1 * enterMapDistance) + .1f))
            {
                HideArea();
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!triggerOnContact)
        {
            return;
        }

        if (!rehideArea)
        {
            return;
        }

        HideArea();
    }

    public void TriggerArea() => triggered = true;
    public void HideArea() => triggered = false;

    public static HiddenAreaTM CreateDormantHiddenArea(Tilemap _groundMap)
    {
        GameObject hiddenAreaTM = new GameObject();

        hiddenAreaTM.transform.parent = _groundMap.gameObject.transform.parent;
        Tilemap tilemap = hiddenAreaTM.AddComponent<Tilemap>();

        tilemap.color = _groundMap.color;
        tilemap.tileAnchor = _groundMap.tileAnchor;
        tilemap.animationFrameRate = _groundMap.animationFrameRate;
        tilemap.orientation = _groundMap.orientation;
        tilemap.orientationMatrix = _groundMap.orientationMatrix;

        TilemapRenderer fromRenderer = _groundMap.GetComponent<TilemapRenderer>();
        TilemapRenderer tmRenderer = hiddenAreaTM.AddComponent<TilemapRenderer>();

        tmRenderer.sortOrder = fromRenderer.sortOrder;
        tmRenderer.mode = fromRenderer.mode;
        tmRenderer.detectChunkCullingBounds = fromRenderer.detectChunkCullingBounds;
        tmRenderer.chunkCullingBounds = fromRenderer.chunkCullingBounds;
        tmRenderer.maskInteraction = fromRenderer.maskInteraction;
        tmRenderer.material = fromRenderer.material;
        tmRenderer.sortingLayerName = fromRenderer.sortingLayerName;
        tmRenderer.sortingLayerID = fromRenderer.sortingLayerID;
        tmRenderer.sortingOrder = fromRenderer.sortingOrder;
        tmRenderer.renderingLayerMask = fromRenderer.renderingLayerMask;

        HiddenAreaTM hatm = hiddenAreaTM.AddComponent<HiddenAreaTM>();

        hatm.groundMap = new TilemapData();
        hatm.hiddenMap = new TilemapData();
        hatm.adjacentHiddenMaps = new List<HiddenAreaTM>();
        hatm.outlineLayers = new List<Color>();
        hatm.detailMaps = new List<DetailAdd>();

        hatm.groundMap.SetObj(_groundMap.gameObject);
        hatm.hiddenMap.SetObj(hiddenAreaTM);

        return hatm;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HiddenAreaTM))]
public class HiddenAreaTMEditor : Editor
{
    private SerializedProperty removeOutlineProp;
    private SerializedProperty outlineLayersProp;
    private SerializedProperty detailMapsProp;
    private SerializedProperty fadeSpeedProp;
    private SerializedProperty enterMapDistanceProp;
    private SerializedProperty triggerOnContactProp;
    private SerializedProperty groundMapProp;
    private SerializedProperty rehideAreaProp;
    private SerializedProperty adjacentHiddenMapsProp;
    private SerializedProperty ignoreTilesProp;

    private SerializedProperty hiddenMapProp;

    private SerializedProperty initOnStartProp;

    private bool showSettings = true;
    private bool showReferences = false;
    private bool showAdvanced = false;

    public void Awake()
    {
        removeOutlineProp = serializedObject.FindProperty("removeOutline");
        outlineLayersProp = serializedObject.FindProperty("outlineLayers");
        detailMapsProp = serializedObject.FindProperty("detailMaps");
        fadeSpeedProp = serializedObject.FindProperty("fadeSpeed");
        enterMapDistanceProp = serializedObject.FindProperty("enterMapDistance");
        triggerOnContactProp = serializedObject.FindProperty("triggerOnContact");
        groundMapProp = serializedObject.FindProperty("groundMap");
        rehideAreaProp = serializedObject.FindProperty("rehideArea");

        adjacentHiddenMapsProp = serializedObject.FindProperty("adjacentHiddenMaps");
        ignoreTilesProp = serializedObject.FindProperty("ignoreTiles");

        hiddenMapProp = serializedObject.FindProperty("hiddenMap");

        initOnStartProp = serializedObject.FindProperty("initOnStart");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

        showSettings = EditorGUILayout.Foldout(showSettings, "Settings");
        //
        if (showSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(groundMapProp);

            EditorGUILayout.PropertyField(fadeSpeedProp);

            EditorGUILayout.PropertyField(rehideAreaProp);

            EditorGUILayout.PropertyField(removeOutlineProp);

            if (removeOutlineProp.boolValue)
            {
                EditorGUILayout.PropertyField(outlineLayersProp);
            }

            EditorGUILayout.PropertyField(detailMapsProp);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }

        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advaced");

        if (showAdvanced)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(triggerOnContactProp);
            if (triggerOnContactProp.boolValue)
            {
                EditorGUILayout.PropertyField(enterMapDistanceProp);
            }

            EditorGUILayout.PropertyField(initOnStartProp);

            EditorGUILayout.PropertyField(adjacentHiddenMapsProp);

            EditorGUILayout.PropertyField(ignoreTilesProp);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }

        showReferences = EditorGUILayout.Foldout(showReferences, "References");

        if (showReferences)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(hiddenMapProp);

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

    public void SetObj(GameObject tmObj) => tilemapObject = tmObj;
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
