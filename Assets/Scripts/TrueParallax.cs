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
    public int accForTrueParallax;


    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        startpos2 = transform.position.y;
        yMultiple = 0.05f;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;

        // creates two extra versions of the backround and places them next to the right and left of the original
        // (makes implementation easier, because previously you had to make 2 copies of each backround element in
        // order to make sure you didn't run out of image.
        if (accForTrueParallax == 1)
        {
            accForTrueParallax = 0;
            var g1 = Instantiate(gameObject, new Vector3(startpos - lenght, startpos2, 0), Quaternion.identity);
            var g2 = Instantiate(gameObject, new Vector3(startpos + lenght, startpos2, 0), Quaternion.identity);

        }

    }

    // 
    // needs to be a LateUpdate because the camera uses LateUpdate (otherwise some layers studder)
    void LateUpdate()
    {
        var temp = cam.transform.position.x * (1 - parallaxEffect);
        var dist = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startpos + dist, startpos2 + (transform.position.y * yMultiple), transform.position.z);

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
