using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenAreaTM : MonoBehaviour
{
    [SerializeField] private Tilemap hiddenTM;
    [SerializeField] private TilemapRenderer hiddenTMRender;
    [SerializeField] private Tilemap targetTM;

    private struct TileData
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

    private bool triggered = false;

    private void Start()
    {
        tileHold = new List<TileData>();

        int count = 0;

        /*for (int x = hiddenTM.origin.x; x < hiddenTM.size.x; x++)
        {
            for (int y = hiddenTM.origin.y; x < hiddenTM.size.y; y++)
            {
                for (int z = hiddenTM.origin.z; x < hiddenTM.size.z; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);

                    TileBase tile = hiddenTM.GetTile(pos);

                    if (++count > 50)
                    {
                        return;
                    } else
                    {
                        Debug.Log(x + " " + y + " " + z);
                    }

                    if (tile)
                    {
                        hiddenTM.SetTile(pos, null);
                        targetTM.SetTile(pos, tile);
                        tileHold.Add(new TileData(pos, tile));
                    }
                }
            }
        }*/


        foreach (Vector3Int pos in hiddenTM.cellBounds.allPositionsWithin)
        {
            TileBase tile = hiddenTM.GetTile(pos);

            /*if (++count > 50)
            {
                return;
            }
            else*/
            {
                Debug.Log(pos.x + " " + pos.y + " " + pos.z);
            }

            if (tile)
            {
                hiddenTM.SetTile(pos, null);
                tileHold.Add(new TileData(pos, targetTM.GetTile(pos)));

                if (CheckForBlankSpace(pos, hiddenTM, targetTM))
                {
                    // spritestuff
                }

                targetTM.SetTile(pos, tile);
            }
        }

        hiddenTM.enabled = false;
        hiddenTMRender.enabled = false;
    }

    public bool CheckForBlankSpace(Vector3Int pos, Tilemap tm1, Tilemap tm2)
    {
        Vector3Int[] neighbourPositions =
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

        foreach (Vector3Int offset in neighbourPositions)
        {
            if (!tm1.GetTile(pos + offset) && !tm2.GetTile(pos + offset))
            {
                return true;
            }
        }

        return false;
    }

   /* public Texture2D stripOutline(Texture2D sp, int numOutlineColors)
    {
        List<Color> seenColors = new List<Color>();
        
    }*/

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("gardem");
        if (collision.gameObject.CompareTag("Player") && !triggered)
        {
            triggered = true;

            foreach (TileData td in tileHold)
            {
                targetTM.SetTile(td.pos, td.tile);
            }
        }
    }
}
