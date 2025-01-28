using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakableTiles : MonoBehaviour
{
    public Tilemap destructableTilemap;

    private void Start()
    {
        destructableTilemap = GetComponent<Tilemap>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
            Vector3 hitPos = Vector3.zero;
            foreach (ContactPoint2D hit in collision.contacts)
            {
                hitPos.x = hit.point.x - 0.01f * hit.normal.x;
                hitPos.y = hit.point.y - 0.01f * hit.normal.y;
                //Tile destroyedTile =
                StartCoroutine(DestroyTile(hitPos));
            }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector3 hitPos = Vector3.zero;
        foreach (ContactPoint2D hit in collision.contacts)
        {
            hitPos.x = hit.point.x - 0.01f * hit.normal.x;
            hitPos.y = hit.point.y - 0.01f * hit.normal.y;
            //Tile destroyedTile =
            StartCoroutine(DestroyTile(hitPos));
        }
    }
        IEnumerator DestroyTile(Vector3 hit)
    {
        yield return new WaitForSeconds(0.2f);
        destructableTilemap.SetTile(destructableTilemap.WorldToCell(hit), null);
    }
}
