using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenAreaTM : MonoBehaviour
{
    [SerializeField] private Tilemap hiddenTM;
    [SerializeField] private TilemapRenderer hiddenTMRender;
    [SerializeField] private Tilemap groundTM;

    [System.Serializable]
    public class DetailAdd
    {
        [SerializeField] public Tilemap detailMap;
        [SerializeField] public Tilemap addedDetails;

        [System.NonSerialized] public List<TileData> tileHold;
        [System.NonSerialized] public Tilemap fadeMap;
        [System.NonSerialized] public GameObject fadeObj;
        [System.NonSerialized] public TilemapRenderer fadeRend;
    }

    [SerializeField] private List<DetailAdd> detailMaps;

    [SerializeField] private List<Color> outlineColors;
    [SerializeField] private Tile defaultTile;

    [SerializeField] private GameObject fadeMapPrefab;

    private Tilemap fadeMap;
    private TilemapRenderer fadeRenderer;

    [SerializeField] private float fadeSpeed;

    public struct TileData
    {
        public Vector3Int pos;
        public TileBase tile;
        public TileData(Vector3Int pos, TileBase tile)
        {
            this.pos = pos;
            this.tile = tile;
        }
    }

    private List<TileData> tileHold;

    private HashSet<Vector3Int> ruleTileOverrides;

    private struct TileColorData
    {
        public Vector3Int pos;
        public Color color;
        public TileColorData(Vector3Int pos, Color color)
        {
            this.pos = pos;
            this.color = color;
        }
    }

    private List<TileColorData> tileColorStorage;

    private bool triggered = false;
    private bool faded = false;

    private static readonly Vector3Int[] NeighbourPositions =
    {
        Vector3Int.up,
        Vector3Int.right,
        Vector3Int.down,
        Vector3Int.left,
        //Vector3Int.up + Vector3Int.right,
        //Vector3Int.up + Vector3Int.left,
        //Vector3Int.down + Vector3Int.right,
        //Vector3Int.down + Vector3Int.left
    };
    private static readonly Vector3Int[] NeighbourPositionsDiagonals =
    {
        Vector3Int.up,
        Vector3Int.right,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.up + Vector3Int.right,
        Vector3Int.up + Vector3Int.left,
        Vector3Int.down + Vector3Int.right,
        Vector3Int.down + Vector3Int.left
    };

    private static readonly Vector3Int[] NeighbourDiagonals =
    {
        Vector3Int.up + Vector3Int.right,
        Vector3Int.up + Vector3Int.left,
        Vector3Int.down + Vector3Int.right,
        Vector3Int.down + Vector3Int.left
    };

    private void Update()
    {
        if (fadeMap.color.a < 0.001f && !faded)
        {
            faded = true;
            fadeMap.enabled = false;
            fadeRenderer.enabled = false;
            
            for (int i = 0; i < detailMaps.Count; i++)
            {
                detailMaps[i].fadeMap.enabled = false;
                detailMaps[i].fadeRend.enabled = false;
            }
        }
        if (triggered && !faded)
        {
            fadeMap.color = new Color(fadeMap.color.r, fadeMap.color.g, fadeMap.color.b, Mathf.Max(0, fadeMap.color.a - fadeSpeed * Time.deltaTime));

            for (int i = 0; i < detailMaps.Count; i++)
            {
                detailMaps[i].fadeMap.color = new Color(
                    detailMaps[i].fadeMap.color.r,
                    detailMaps[i].fadeMap.color.g,
                    detailMaps[i].fadeMap.color.b,
                    fadeMap.color.a);
            }
        }
    }

    private void Start()
    {
        GameObject fadeMapObj = Instantiate(fadeMapPrefab, transform);
        fadeMap = fadeMapObj.GetComponent<Tilemap>();
        fadeRenderer = fadeMapObj.GetComponent<TilemapRenderer>();

        hiddenTM.enabled = true;
        hiddenTMRender.enabled = true;

        for (int i = 0; i < detailMaps.Count; i++)
        {
            detailMaps[i].fadeObj = Instantiate(fadeMap.gameObject, transform);
            detailMaps[i].fadeMap = detailMaps[i].fadeObj.GetComponent<Tilemap>();
            detailMaps[i].fadeRend = detailMaps[i].fadeObj.GetComponent<TilemapRenderer>();
            detailMaps[i].fadeRend.sortingOrder += detailMaps.Count - i;

            detailMaps[i].fadeMap.enabled = false;
            detailMaps[i].fadeRend.enabled = false;

            detailMaps[i].tileHold = new List<TileData>();

            foreach (Vector3Int pos in detailMaps[i].addedDetails.cellBounds.allPositionsWithin)
            {
                TileBase tile = detailMaps[i].addedDetails.GetTile(pos);
                if (tile)
                {
                    detailMaps[i].tileHold.Add(new TileData(pos, detailMaps[i].detailMap.GetTile(pos)));
                    detailMaps[i].detailMap.SetTile(pos, tile);
                }
            }

            detailMaps[i].detailMap.RefreshAllTiles();
            detailMaps[i].addedDetails.enabled = false;
            detailMaps[i].addedDetails.gameObject.GetComponent<TilemapRenderer>().enabled = false;
        }

        for (int i = 0; i < detailMaps.Count; i++)
        {
            foreach (TileData td in detailMaps[i].tileHold)
            {
                Vector3Int pos = td.pos;
                detailMaps[i].fadeMap.SetTile(pos, detailMaps[i].detailMap.GetTile(pos));
                detailMaps[i].fadeMap.SetTransformMatrix(pos, detailMaps[i].detailMap.GetTransformMatrix(pos));
            }
            detailMaps[i].fadeMap.RefreshAllTiles();
        }

        fadeMap.enabled = false;
        fadeRenderer.enabled = false;
        tileHold = new List<TileData>();
        tileColorStorage = new List<TileColorData>();

        List<Vector3Int> seenTiles = new List<Vector3Int>();
        ruleTileOverrides = new HashSet<Vector3Int>();

        foreach (Vector3Int pos in hiddenTM.cellBounds.allPositionsWithin)
        {
            TileBase tile = hiddenTM.GetTile(pos);

            if (tile)
            {
                seenTiles.Add(pos);

                tileHold.Add(new TileData(pos, groundTM.GetTile(pos)));

                groundTM.SetTile(pos, tile);
            }
        }

        List<Vector3Int> replacePositions = new List<Vector3Int>();
        List<Tile> replaceTiles = new List<Tile>();

        foreach (Vector3Int pos in seenTiles)
        {
            if (CheckForBlankSpace(pos, hiddenTM, groundTM))
            {
                Tile outTile = CreateImageTile(pos, groundTM, true);
                //outTile.color = new Color(1, 1, 1, 1);
                outTile.name = "edgeTile" + pos.x + " " + pos.y;
                outTile.colliderType = Tile.ColliderType.None;

                replacePositions.Add(pos);
                replaceTiles.Add(outTile);

                foreach (Vector3Int rulePos in GetRuleTileSpaces(pos, groundTM))
                {
                    ruleTileOverrides.Add(rulePos);
                }
            };
        }

        foreach (Vector3Int pos in seenTiles)
        {
            hiddenTM.SetTile(pos, null);
        }

        foreach (Vector3Int pos in ruleTileOverrides)
        {
            if (!replacePositions.Contains(pos))
            {
                Tile outTile = CreateImageTile(pos, groundTM, false);
                //outTile.color = new Color(1, 1, 1, 1);
                outTile.name = "ruleFillTile" + pos.x + " " + pos.y;
                outTile.colliderType = Tile.ColliderType.None;

                hiddenTM.SetTile(pos, outTile);
                hiddenTM.SetTransformMatrix(pos, groundTM.GetTransformMatrix(pos));

                fadeMap.SetTile(pos, outTile);
                fadeMap.SetTransformMatrix(pos, groundTM.GetTransformMatrix(pos));
            }

            tileColorStorage.Add(new TileColorData(pos, groundTM.GetColor(pos)));
            groundTM.SetColor(pos, new Color(1, 1, 1, 0));
        }

        hiddenTM.RefreshAllTiles();

        List<Matrix4x4> transforms = new List<Matrix4x4>();

        for (int i = 0; i < replacePositions.Count; i++)
        {
            transforms.Add(groundTM.GetTransformMatrix(replacePositions[i]));
        }

        for (int i = 0; i < replacePositions.Count; i++)
        {
            groundTM.SetTile(replacePositions[i], replaceTiles[i]);
            groundTM.SetTransformMatrix(replacePositions[i], transforms[i]);
            groundTM.RefreshTile(replacePositions[i]);
        }

        foreach (Vector3Int pos in seenTiles)
        {
            if (!fadeMap.HasTile(pos))
            {
                fadeMap.SetTile(pos, CreateImageTile(pos, groundTM, false));
                fadeMap.SetTransformMatrix(pos, groundTM.GetTransformMatrix(pos));
            }
        }

        HashSet<Vector3Int> fadeOuterTiles = new HashSet<Vector3Int>();

        foreach (Vector3Int pos in ruleTileOverrides)
        {
            List<Vector3Int> potentialOuterTiles = GetRuleTileSpaces(pos, groundTM);
            foreach (Vector3Int potentialOuterPos in potentialOuterTiles)
            {
                if (!fadeMap.HasTile(potentialOuterPos))
                {
                    fadeOuterTiles.Add(potentialOuterPos);
                }
            }
        }
        foreach (Vector3Int pos in seenTiles)
        {
            List<Vector3Int> potentialOuterTiles = GetRuleTileSpaces(pos, groundTM);
            foreach (Vector3Int potentialOuterPos in potentialOuterTiles)
            {
                if (!fadeMap.HasTile(potentialOuterPos))
                {
                    fadeOuterTiles.Add(potentialOuterPos);
                }
            }
        }

        foreach(Vector3Int outerTilePos in fadeOuterTiles)
        {
            fadeMap.SetTile(outerTilePos, CreateImageTile(outerTilePos, groundTM, false));
            fadeMap.SetTransformMatrix(outerTilePos, groundTM.GetTransformMatrix(outerTilePos));
        }

        fadeMap.RefreshAllTiles();

        //hiddenTM.enabled = false;
        //hiddenTMRender.enabled = false;
    }

    public bool CheckForBlankSpace(Vector3Int pos, Tilemap tm1, Tilemap tm2)
    {
        foreach (Vector3Int offset in NeighbourPositions)
        {
            if (!tm1.HasTile(pos + offset) && !tm2.HasTile(pos + offset))
            {
                return true;
            }
        }

        foreach (Vector3Int offset in NeighbourDiagonals)
        {
            Vector3Int dPos = pos + offset;

            if (!tm1.HasTile(dPos) && !tm2.HasTile(dPos))
            {
                int hiddenAdjCount = 0;
                foreach(Vector3Int off2 in NeighbourPositions)
                {
                    if (hiddenTM.HasTile(dPos + off2))
                    {
                        hiddenAdjCount++;
                    }
                }
                if (hiddenAdjCount > 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Texture2D stripOutline(Texture2D sp)
    {
        Texture2D res = Instantiate(sp);
        for (int x = 0; x < sp.width; x++)
        {
            for (int y = 0; y < sp.height; y++)
            {
                if (!CheckForBlankSpaceSprite(x, y, sp))
                {
                    continue;
                }

                if (outlineColors.Contains(sp.GetPixel(x, y)))
                {
                    res.SetPixel(x, y, new Color(1, 1, 1, 0));
                }
            }
        }

        res.Apply();

        return res;
    }

    public bool CheckForBlankSpaceSprite(int x, int y, Texture2D tex)
    {
        foreach (Vector3Int offset in NeighbourPositions)
        {
            int xpos = x + offset.x;
            int ypos = y + offset.y;

            if (xpos < 0 || xpos >= tex.width)
            {
                return true;
            }

            if (ypos < 0 || ypos >= tex.height)
            {
                return true;
            }

            if (tex.GetPixel(x, y).a == 0)
            {
                return true;
            }
        }

        return false;
    }

    public List<Vector3Int> GetRuleTileSpaces(Vector3Int pos, Tilemap tm)
    {
        List<Vector3Int> res = new List<Vector3Int>();
        foreach (Vector3Int offset in NeighbourPositionsDiagonals)
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

    public Texture2D GetSlicedSpriteTexture(Sprite sprite)
    {
        Rect rect = sprite.rect;
        Texture2D slicedTex = new Texture2D((int)rect.width, (int)rect.height);
        slicedTex.filterMode = sprite.texture.filterMode;

        slicedTex.SetPixels(0, 0, (int)rect.width, (int)rect.height, sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
        slicedTex.Apply();

        return slicedTex;
    }

    public Tile CreateImageTile(Vector3Int pos, Tilemap tm, bool removeOutline)
    {
        Sprite curSp = tm.GetSprite(pos);
        Texture2D sliced = GetSlicedSpriteTexture(curSp);
        Texture2D resTex;
        /*if (removeOutline)
        {
            resTex = stripOutline(sliced);
        } else
        {
            resTex = sliced;
        }*/

        if (removeOutline)
        {
            sliced = stripOutline(sliced);
        }

        Sprite resSp = Sprite.Create(
            sliced,
            new Rect(0, 0, sliced.width, sliced.height),
            tm.tileAnchor,
            curSp.pixelsPerUnit);

        Tile res = Instantiate(defaultTile);

        res.sprite = resSp;

        return res;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !triggered)
        {
            triggered = true;

            foreach (TileData td in tileHold)
            {
                groundTM.SetTile(td.pos, td.tile);
            }

            foreach (TileColorData cd in tileColorStorage)
            {
                groundTM.SetColor(cd.pos, new Color(cd.color.r, cd.color.g, cd.color.b, cd.color.a));
            }

            for (int i = 0; i < detailMaps.Count; i++)
            {
                foreach (TileData td in detailMaps[i].tileHold)
                {
                    detailMaps[i].detailMap.SetTile(td.pos, td.tile);
                }

                detailMaps[i].fadeMap.enabled = true;
                detailMaps[i].fadeRend.enabled = true;
            }

            hiddenTM.enabled = false;
            hiddenTMRender.enabled = false;

            fadeMap.enabled = true;
            fadeRenderer.enabled = true;
        }
    }

}
