using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Serialization;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

// if any questions come up @Thomas Belk in discord
// additionally, this script comes from this tutorial with some light modifications to better suit our game
// https://www.youtube.com/watch?v=zit45k6CUMk&t=317s 
public class FourTileParallax : MonoBehaviour
{

    //private float lengthX, lengthY, startpos, startpos2;
    Vector2 startpos;
    private GameObject cam;
    private PixelPerfectCamera ppc;
    [SerializeField] private float parallaxEffectX;
    [SerializeField] private bool enableVerticleParallax = false;
    [SerializeField] private float parallaxEffectY = .05f;
    [SerializeField] private bool tileY = false;

    [SerializeField] private Tile[] tiles;

    Vector2 resolution; //texture resolution (size) of any single textures
    Vector2 camSize;
    Vector2 scrollRectSize; //the total area of the 4 tile

    Vector2Int unitScrollRectSize;
    Vector2 scrollRectPos = new Vector2();

    Vector2 oldPos;

    [Serializable]
    public struct Tile
    {
        public Transform transform;
        [HideInInspector] public Vector2 imageBounds;
        public Vector2Int unitLocalOffset;

    }

    

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position;
        oldPos = transform.position;
        Vector2 rectMin = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 rectMax = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        
        for (int i = 0; i < tiles.Length; i++)
        {
            if(tiles[i].transform.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
            {
                sr.drawMode = SpriteDrawMode.Tiled;
                tiles[i].imageBounds = sr.bounds.size;
                resolution = tiles[i].imageBounds;
                // scrollRectSize = new Vector2(
                //     Mathf.Max((tiles[i].unitLocalOffset.x + 1) * resolution.x, scrollRectSize.x),
                //     Mathf.Max((tiles[i].unitLocalOffset.y + 1) * resolution.y, scrollRectSize.y)
                // );
                //set grid formation
                tiles[i].transform.position = startpos + new Vector2
                    (tiles[i].imageBounds.x * tiles[i].unitLocalOffset.x
                    , tiles[i].imageBounds.y * tiles[i].unitLocalOffset.y);

                Bounds b = sr.bounds;
                rectMin = Vector2.Min(rectMin, b.min);
                rectMax = Vector2.Max(rectMax, b.max);
                // scrollRectPos = new Vector2(
                //     Mathf.Min(sr.bounds.min.x, scrollRectPos.x),
                //     Mathf.Min(sr.bounds.min.y, scrollRectPos.y)
                // );
            }

            unitScrollRectSize = new Vector2Int(
                    Mathf.Max(tiles[i].unitLocalOffset.x, unitScrollRectSize.x),
                    Mathf.Max(tiles[i].unitLocalOffset.y, unitScrollRectSize.y)
                );
        }

        scrollRectPos = rectMin;
        scrollRectSize = rectMax - rectMin;

        cam = Camera.main.gameObject;
        CalculateCameraSize(true);

        Debug.Log($"Cam size: {camSize}, scroll rect size: {scrollRectSize}, scroll rect pos: {scrollRectPos}");
    }

    void CalculateCameraSize(bool pixelPerfect)
    {
        var comp = cam.GetComponent<Camera>();
        ppc = cam.GetComponent<PixelPerfectCamera>();
        Vector2 viewSize;
        if ((ppc != null && ppc.isActiveAndEnabled) && pixelPerfect)
        {
            // world units per screen pixel = 1 / (PPU * zoom)
            float unitsPerPixel = 1f / (ppc.assetsPPU * ppc.pixelRatio);
            viewSize = new Vector2(comp.pixelWidth * unitsPerPixel,
                               comp.pixelHeight * unitsPerPixel);
        }
        else
        {
            float height = comp.orthographicSize * 2f;
            float width = height * comp.aspect;
            viewSize = new Vector2(width, height);
        }
        
        camSize = viewSize;
        Debug.Log(camSize);
    }

    // 
    // needs to be a LateUpdate because the camera uses LateUpdate (otherwise some layers studder)
    void LateUpdate()
    {
        float buffer = 1f;
        //because camera sizes change, size need to be recalculated every frame.
        CalculateCameraSize(false);
        // Calculate new position with fixed parallax direction
        float newX = startpos.x + (cam.transform.position.x - startpos.x) * parallaxEffectX;
        float newY = transform.position.y;

        if (enableVerticleParallax)
        {
            newY = startpos.y + (cam.transform.position.y - startpos.y) * parallaxEffectY;

            // Handle vertical wrapping
            if(cam.transform.position.y + (camSize.y / 2f) + buffer > scrollRectPos.y + scrollRectSize.y) //up shift
            {
                ScrollTiles(Vector2.up, 0, resolution.y, unitScrollRectSize.y, false);
            }

            if(cam.transform.position.y - (camSize.y / 2f) - buffer < scrollRectPos.y) //down shift
            {
                ScrollTiles(Vector2.down, unitScrollRectSize.y, resolution.y, unitScrollRectSize.y, false);
            }
        }

        transform.position = new Vector3(newX, newY, transform.position.z);
        scrollRectPos += (Vector2)transform.position - oldPos;
        oldPos = transform.position;
        //Horizontal wraping
        if(cam.transform.position.x + (camSize.x / 2f) + buffer > scrollRectPos.x + scrollRectSize.x) //right shift
        {
            ScrollTiles(Vector2.right, 0, resolution.x, unitScrollRectSize.x, true);
        }

        if(cam.transform.position.x - (camSize.x / 2f) - buffer < scrollRectPos.x) //left shift
        {
            ScrollTiles(Vector2.left, unitScrollRectSize.x, resolution.x, unitScrollRectSize.x, true);
        }

    }

    void ScrollTiles(Vector2 dir, int end, float resolution, int unitScrollRectSize, bool isHorizontalShift)
    {
        Vector2 displacement = resolution * (unitScrollRectSize + 1) * dir;
        for (int i = 0; i < tiles.Length; i++)
        {
            if((isHorizontalShift ? tiles[i].unitLocalOffset.x : tiles[i].unitLocalOffset.y) == end)
            {
                tiles[i].transform.Translate(displacement);
                tiles[i].unitLocalOffset += Vector2Int.RoundToInt(dir) * (unitScrollRectSize + 1);
            }
            tiles[i].unitLocalOffset += Vector2Int.RoundToInt(-dir);
        }
        scrollRectPos += resolution * dir;
    }

    void OnDrawGizmos()
    {
        
        // Draw wireframe box
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(scrollRectPos + scrollRectSize / 2, scrollRectSize);
    }

}
