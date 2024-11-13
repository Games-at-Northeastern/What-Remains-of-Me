using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpikeTeleport))]
public class RustedFloorManager : MonoBehaviour
{
    private class RustedGroup
    {
        public List<RustedTile> tiles;

        public List<(Vector3 left, Vector3 right)> GetOpenings()
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

        public void InitHiddenMaps()
        {
            if (tiles.Count < 1)
            {
                return;
            }

            List<Vector3Int> ignoreTiles = new List<Vector3Int>();

            foreach(RustedTile tile in tiles)
            {
                ignoreTiles.Add(tile.mapPos);
            }

            foreach (RustedTile tile in tiles)
            {
                tile.HiddenArea.IgnoreTiles = ignoreTiles;
            }

            RustedTile leftLeftTile = null;
            RustedTile leftTile = null;
            RustedTile currentTile = null;
            RustedTile rightTile = tiles[0];
            RustedTile rightRightTile = tiles.Count > 1 ? tiles[1] : null;

            int i = 2;

            while (rightTile is not null)
            {
                leftLeftTile = leftTile;
                leftTile = currentTile;
                currentTile = rightTile;
                rightTile = rightRightTile;
                rightRightTile = i < tiles.Count ? tiles[i] : null; 
                i++;

                currentTile.InitHM(i.ToString(), leftLeftTile, leftTile, rightTile, rightRightTile);
            }
        }
    }

    public class RustedTile
    {
        public TilemappedObject tmo;
        public Vector3 leftEnd;
        public Vector3 rightEnd;
        Vector3 worldPos;
        public Vector3Int mapPos;

        public bool destroyed;

        public HiddenAreaTM HiddenArea { get; set; }

        public RustedTile(Tilemap map, Vector3Int position, Transform parent, int depth)
        {
            tmo = TilemappedObject.Generate(parent, map, position, true, false).GetComponent<TilemappedObject>();
            worldPos = map.CellToWorld(position);
            mapPos = position;
            leftEnd = worldPos + new Vector3(-map.cellBounds.size.x / 2, map.cellBounds.size.y, 0);
            rightEnd = worldPos = new Vector3(map.cellBounds.size.x / 2, map.cellBounds.size.y, 0);

            destroyed = false;

            GenHM(depth);
        }

        public void Destroy()
        {
            tmo.DestroyTile();
            destroyed = true;
        }

        public void GenHM(int depth)
        {
            HiddenArea = HiddenAreaTM.CreateDormantHiddenArea(tmo.Map);
            TilemapData tilemapData = HiddenArea.GetHiddenMap();
            Tilemap tilemap = tilemapData.TMap;

            Vector3Int pos = mapPos;
            for (int i = 0; i < depth; i++)
            {
                pos += Vector3Int.down;

                TileBase tile = tmo.Map.GetTile(pos);
                tilemap.SetTile(pos, tile);
                tilemap.SetColor(pos, tmo.Map.GetColor(pos));
                if (tile is not RuleTile)
                {
                    tilemap.SetTransformMatrix(pos, tmo.Map.GetTransformMatrix(pos));
                }

                tmo.Map.SetTile(pos, null);
                tmo.Map.RefreshTile(pos);

                tilemap.RefreshTile(pos);
            }

            HiddenArea.RemoveOutline = false;
            HiddenArea.TriggerOnContact = false;
            HiddenArea.FadeSpeed = 1.5f;
        }

        public void InitHM(string name, params RustedTile[] adjacents)
        {
            HiddenArea.gameObject.name = name;
            foreach (RustedTile rt in adjacents)
            {
                if (rt is null)
                {
                    continue;
                }
                HiddenArea.AdjacentHiddenMaps.Add(rt.HiddenArea);
            }

            HiddenArea.Init();
        }
    }

    [SerializeField] private TileBase rustedTileType;
    private List<RustedGroup> rustedGroups;
    [SerializeField] private Tilemap groundMap;
    [SerializeField] private int depth;
    [SerializeField] Cinemachine.CinemachineVirtualCamera virtualCamera;

    private SpikeTeleport spikeTeleport;

    public void Start()
    {
        spikeTeleport = GetComponent<SpikeTeleport>();

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
                rgroup.tiles.Add(InitializeRustedTile(leftPositions[i], rustedIndex, rGroups));
                usedPositions.Add(leftPositions[i]);
                rustedIndex++;
            }

            RustedTile rt = InitializeRustedTile(position, rustedIndex, rGroups);
            usedPositions.Add(position);
            rgroup.tiles.Add(rt);

            rustedIndex++;

            head = position + Vector3Int.right;
            while (groundMap.GetTile(head) == rustedTileType)
            {
                rgroup.tiles.Add(InitializeRustedTile(head, rustedIndex, rGroups));
                usedPositions.Add(head);
                head += Vector3Int.right;
                rustedIndex++;
            }

            rGroups++;

            rgroup.InitHiddenMaps();

            rustedGroups.Add(rgroup);
        }
    }

    private RustedTile InitializeRustedTile(Vector3Int position, int rustedIndex, int rustedGroupIndex)
    {
        RustedTile rt = new RustedTile(groundMap, position, transform, depth);
        rt.tmo.callOnTrigger2D += (Collider2D otherCol) => HandleCollidedWith(rustedIndex, rustedGroupIndex, otherCol.gameObject);
        return rt;
    }

    bool dropped = false;
    bool killed = false;

    private void HandleCollidedWith(int rustedIndex, int rustedGroupIndex, GameObject player)
    {
        if (!player.CompareTag("Player") || (dropped && killed))
        {
            return;
        }

        if (!dropped)
        {
            RustedGroup rg = rustedGroups[rustedGroupIndex];

            if (!rg.tiles[rustedIndex].destroyed)
            {
                rg.tiles[rustedIndex].HiddenArea.TriggerArea();
                rg.tiles[rustedIndex].Destroy();
                dropped = true;
            }
        }

        if (!killed)
        {
            StartCoroutine(KillAfterDelay(0.5f, player));
            killed = true;
        }
    }

    private IEnumerator DeathCam(Transform target)
    {
        virtualCamera.Follow = null;
        float lerpSpeed = 0.15f;
        while (true)
        {
            lerpSpeed *= 0.99f;
            yield return new WaitForFixedUpdate();
            Vector3 newPos = Vector3.Lerp(virtualCamera.transform.position, target.position, lerpSpeed * Time.fixedDeltaTime);
            virtualCamera.ForceCameraPosition(newPos, virtualCamera.transform.rotation);
        }
    }

    private IEnumerator KillAfterDelay(float delay, GameObject target)
    {
        yield return new WaitForSeconds(0.1f);

        Coroutine deathCam = StartCoroutine(DeathCam(target.transform));

        yield return new WaitForSeconds(delay - 0.1f);

        yield return spikeTeleport.PerformDeath(target, new Vector3(0, 2));

        StopCoroutine(deathCam);
        virtualCamera.Follow = target.transform;

        killed = false;
        dropped = false;

        //virtualCamera.Follow = target.transform;
    }
}
