using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class FlickerControl : MonoBehaviour
{
    private Light2D light;

    // the minimum intensity of the light
    public float minIntensity = 0.1f;
    // the maximum intensity of the light, 1.5 is the normal light intensity 
    public float maxIntensity = 1.5f;
    // the rate at which the light will flicker, higher value is slower
    public float flickerSpeed = 0.09f;

    // Update is called once per frame
    private void Start()
    {
        light = GetComponent<Light2D>();

        InvokeRepeating("Flicker", 0f, flickerSpeed);
    }

    // the light will be assigned a random intensity between the min and max values
    // which makes the flickering happen
    private void Flicker() 
    {
        float randomIntensity = Random.Range(minIntensity, maxIntensity);
        light.intensity = randomIntensity;
    }
    
    
}
