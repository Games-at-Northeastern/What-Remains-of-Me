using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// if any questions come up @Thomas Belk in discord
// additionally, this script comes from this tutorial with some light modifications to better suit our game
// https://www.youtube.com/watch?v=zit45k6CUMk&t=317s 
public class TrueParallax : MonoBehaviour
{

    private float lengthX, lengthY, startpos, startpos2, yMultiple;
    [SerializeField] private GameObject cam;
    [SerializeField] private float parallaxEffect;
    [SerializeField] private bool enableVerticleParallax = false;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        startpos2 = transform.position.y;
        yMultiple = 0.1f;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
        lengthY = GetComponent<SpriteRenderer>().bounds.size.y;

    }

    // 
    // needs to be a LateUpdate because the camera uses LateUpdate (otherwise some layers studder)
    void LateUpdate()
    {
        float temp = cam.transform.position.x * (1 - parallaxEffect);
        float temp2 = cam.transform.position.y * yMultiple;
        float dist = cam.transform.position.x * parallaxEffect;
        float moveY = startpos2;

        if (enableVerticleParallax)
        {
            moveY = startpos2 + temp2;
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

        if (temp2 > startpos2 + lengthY)
        {
            startpos2 += lengthY;
        }
        else if (temp2 < startpos2 - lengthY)
        {
            startpos2 -= lengthY;
        }
    }

}
