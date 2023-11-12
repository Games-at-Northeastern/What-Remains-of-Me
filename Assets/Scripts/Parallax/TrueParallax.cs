using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// if any questions come up @Thomas Belk in discord
// additionally, this script comes from this tutorial with some light modifications to better suit our game
// https://www.youtube.com/watch?v=zit45k6CUMk&t=317s 
public class TrueParallax : MonoBehaviour
{

    private float lengthX, lengthY, startpos, startpos2;
    private GameObject cam;
    [SerializeField] private float parallaxEffectX;
    [SerializeField] private bool enableVerticleParallax = false;
    [SerializeField] private float parallaxEffectY = .05f;
    [SerializeField] private bool tileY = false;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        startpos2 = transform.position.y;
        if(GetComponent<SpriteRenderer>() != null)
        {
            lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
            lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            if (tileY)
            {
                spriteRenderer.size = new Vector2(spriteRenderer.size.x * 3, spriteRenderer.size.y * 3);
            }
            else
            {
                spriteRenderer.size = new Vector2(spriteRenderer.size.x * 3, spriteRenderer.size.y);
            }
        }

        cam = Camera.main.gameObject;

    }

    // 
    // needs to be a LateUpdate because the camera uses LateUpdate (otherwise some layers studder)
    void LateUpdate()
    {
        float temp = cam.transform.position.x * (1 - parallaxEffectX);
        float temp2 = cam.transform.position.y * (1 - parallaxEffectY);
        float dist = cam.transform.position.x * parallaxEffectX;
        float dist2 = cam.transform.position.y * parallaxEffectY;
        float moveY = startpos2;

        if (enableVerticleParallax)
        {
            moveY = startpos2 + dist2;

            if (temp2 > startpos2 + lengthY)
            {
                startpos2 += lengthY;
            }
            else if (temp2 < startpos2 - lengthY)
            {
                startpos2 -= lengthY;
            }

        }

        transform.position = new Vector3(startpos + dist, moveY, transform.position.z);

        if (temp > startpos + lengthX)
        {
            startpos += lengthX;
        }
        else if (temp < startpos - lengthX)
        {
            startpos -= lengthX;
        }
    }

}
