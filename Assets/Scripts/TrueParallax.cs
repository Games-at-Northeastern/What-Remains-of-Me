using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

// if any questions come up @Thomas Belk in discord
// additionally, this script comes from this tutorial with some light modifications to better suit our game
// https://www.youtube.com/watch?v=zit45k6CUMk&t=317s 
public class TrueParallax : MonoBehaviour
{

    private float lenght, startpos, startpos2, yMultiple;
    [SerializeField] private GameObject cam;
    [SerializeField] private float parallaxEffect;
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        startpos2 = transform.position.y;
        yMultiple = 0.05f;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // 
    // needs to be a LateUpdate because the camera uses LateUpdate (otherwise some layers studder)
    void LateUpdate()
    {
        float temp = cam.transform.position.x * (1 - parallaxEffect);
        float dist = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startpos + dist, startpos2, transform.position.z);

        if (temp > startpos + lenght)
        {
            startpos += lenght;
        }
        else if (temp < startpos - lenght)
        {
            startpos -= lenght;
        }
    }

}
