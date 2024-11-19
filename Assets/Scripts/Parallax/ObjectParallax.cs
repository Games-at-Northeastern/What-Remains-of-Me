using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectParallax : MonoBehaviour
{
    [SerializeField] private float xFactor;
    [SerializeField] private float yFactor;
    [SerializeField] private bool parallaxX;
    [SerializeField] private bool parallaxY;

    private GameObject cam;
    private Vector3 initialPos;

    public ObjectParallaxTuner ParallaxTuner { get; set; }

    void Start()
    {
        cam = Camera.main.gameObject;
        initialPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void LateUpdate()
    {
        bool doParallaxX = parallaxX;
        bool doParallaxY = parallaxY;
        float doXFactor = xFactor;
        float doYFactor = yFactor;

        if (ParallaxTuner != null)
        {
            if (ParallaxTuner.OverrideParallaxX)
            {
                doParallaxX = ParallaxTuner.SetParallaxX;
                doXFactor = ParallaxTuner.ParallaxX;
            }

            if (ParallaxTuner.OverrideParallaxY)
            {
                doParallaxY = ParallaxTuner.SetParallaxY;
                doYFactor = ParallaxTuner.ParallaxY;
            }

            doXFactor *= ParallaxTuner.ParallaxXMultiplier;
            doYFactor *= ParallaxTuner.ParallaxYMultiplier;
        }

        Vector3 diff = cam.transform.position - initialPos;

        Debug.Log(cam.transform.position);

        float newX = doParallaxX ? initialPos.x + (diff.x * doXFactor) : initialPos.x;
        float newY = doParallaxY ? initialPos.y + (diff.y * doYFactor) : initialPos.y;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}
