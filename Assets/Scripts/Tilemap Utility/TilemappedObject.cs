using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
public class TilemappedObject : MonoBehaviour
{
    public delegate void AcceptTrigger(Collider2D otherCol);
    public delegate void AcceptCollider(Collision2D otherCol);

    [HideInInspector] public event AcceptTrigger callOnTrigger2D;
    [HideInInspector] public event AcceptCollider callOnCollision2D;

    private BoxCollider2D c2D;
    private bool isTrigger;
    private Tilemap map;
    private Vector3Int position;
    public static GameObject Generate(Transform parent, Tilemap map, Vector3Int position, bool isTrigger)
    {
        if (!map.HasTile(position))
        {
            return null;
        }

        GameObject result = new GameObject();
        result.transform.parent = parent;
        TilemappedObject tmo = result.AddComponent<TilemappedObject>();
        tmo.c2D = result.AddComponent<BoxCollider2D>();
        tmo.c2D.isTrigger = isTrigger;
        tmo.isTrigger = isTrigger;
        tmo.map = map;
        tmo.position = position;

        result.transform.position = map.CellToWorld(position);

        tmo.c2D.size = new Vector2(1.05f, 1.05f);
        tmo.c2D.offset = new Vector2(.5f, .5f);

        return result;
    }

    public void FixedUpdate()
    {
        if (map is null)
        {
            Destroy(this);
        }

        if (!map.HasTile(position))
        {
            Destroy(this);
        }
    }

    public void DestroyTile()
    {
        map.SetTile(position, null);
        Destroy(this);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTrigger)
        {
            return;
        }

        callOnTrigger2D?.Invoke(collision);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTrigger)
        {
            return;
        }

        callOnCollision2D?.Invoke(collision);
    }
}
