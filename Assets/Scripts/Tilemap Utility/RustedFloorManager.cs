using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RustedFloorManager : MonoBehaviour
{
    private class RustedGroup
    {
        public List<RustedTile> tiles;

        public List<(Vector3 left, Vector3 right)> getOpenings()
        {
            List<(Vector3 left, Vector3 right)> res = new List<(Vector3 left, Vector3 right)>();

            bool open = false;

            int leftHead = 0;
            int rightHead = 0;

            while (rightHead < tiles.Count)
            {
                if (!open)
                {
                    if (tiles[leftHead].destroyed)
                    {
                        open = true;
                        rightHead++;
                        continue;
                    }
                    else
                    {
                        leftHead++;
                        rightHead = leftHead;
                        continue;
                    }
                }

                if (!tiles[rightHead].destroyed)
                {
                    res.Add((tiles[leftHead].leftEnd, tiles[rightHead - 1].rightEnd));
                    rightHead++;
                    leftHead = rightHead;
                    open = false;
                    continue;
                }

                rightHead++;
            }

            if (open)
            {
                res.Add((tiles[leftHead].leftEnd, tiles[rightHead - 1].rightEnd));
            }

            return res;
        }

        public RustedGroup()
        {
            tiles = new List<RustedTile>();
        }
    }

    public struct RustedTile
    {
        public TilemappedObject tmo;
        public Vector3 leftEnd;
        public Vector3 rightEnd;
        Vector3 worldPos;
        Vector3Int mapPos;

        public bool destroyed;

        public RustedTile(Tilemap map, Vector3Int position, Transform parent)
        {
            tmo = TilemappedObject.Generate(parent, map, position, true).GetComponent<TilemappedObject>();
            worldPos = map.CellToWorld(position);
            mapPos = position;
            leftEnd = worldPos + new Vector3(-map.cellBounds.size.x / 2, map.cellBounds.size.y, 0);
            rightEnd = worldPos = new Vector3(map.cellBounds.size.x / 2, map.cellBounds.size.y, 0);

            destroyed = false;
        }

        public void Destroy() => tmo.DestroyTile();

        /*private void UpdateEnds(Tilemap map, Vector3Int position)
        {
            Sprite tileSprite = map.GetSprite(position);

            int rectX = (int)tileSprite.rect.x;
            int rectY = (int)tileSprite.rect.y;
            int rectWidth = (int)tileSprite.rect.width;
            int rectHeight = (int)tileSprite.rect.height;

            Color[] pixels = tileSprite.texture.GetPixels(rectX, rectY, rectWidth, rectHeight);

            int leftEndPix = 0;

            for (int x = 0; x < tileSprite.rect.width; x++)
            {
                int xOff = x + (int)tileSprite.rect.x;
                Color col = pixels[x + ((rectHeight - 1) * rectWidth)];
                if (col.a <= 0.001)
                {
                    leftEndPix = x;
                    break;
                }
            }

            tmo.
        }*/

        private Texture2D GetSlicedSpriteTexture(Sprite sprite)
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
    }

    [SerializeField] private TileBase rustedTileType;
    private List<RustedGroup> rustedGroups;
    [SerializeField] private Tilemap groundMap;

    public void Start()
    {
        rustedGroups = new List<RustedGroup>();
        HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();

        int rGroups = 0;

        foreach (Vector3Int position in groundMap.cellBounds.allPositionsWithin)
        {
            if (usedPositions.Contains(position))
            {
                continue;
            }

            TileBase tile = groundMap.GetTile(position);

            if (tile is null)
            {
                continue;
            }

            if (tile != rustedTileType)
            {
                continue;
            }

            RustedGroup rgroup = new RustedGroup();

            List<Vector3Int> leftPositions = new List<Vector3Int>();

            Vector3Int head = position + Vector3Int.left;
            while (groundMap.GetTile(head) == rustedTileType)
            {
                leftPositions.Add(head);
                head += Vector3Int.left;
            }

            int rustedIndex = 0;

            for (int i = leftPositions.Count - 1; i > -1; i--)
            {
                rgroup.tiles.Add(initializeRustedTile(leftPositions[i], rustedIndex, rGroups));
                rustedIndex++;
            }

            RustedTile rt = initializeRustedTile(position, rustedIndex, rGroups);
            rgroup.tiles.Add(rt);

            rustedIndex++;

            head = position + Vector3Int.right;
            while (groundMap.GetTile(head) == rustedTileType)
            {
                rgroup.tiles.Add(initializeRustedTile(position, rustedIndex, rGroups));
                head += Vector3Int.right;
                rustedIndex++;
            }

            rGroups++;

            rustedGroups.Add(rgroup);
        }
    }

    private RustedTile initializeRustedTile(Vector3Int position, int rustedIndex, int rustedGroupIndex)
    {
        RustedTile rt = new RustedTile(groundMap, position, transform);
        rt.tmo.callOnTrigger2D += (Collider2D otherCol) => handleCollidedWith(rustedIndex, rustedGroupIndex);
        return rt;
    }

    private void handleCollidedWith(int rustedIndex, int rustedGroupIndex)
    {
        rustedGroups[rustedGroupIndex].tiles[rustedIndex].Destroy();
    }
}
