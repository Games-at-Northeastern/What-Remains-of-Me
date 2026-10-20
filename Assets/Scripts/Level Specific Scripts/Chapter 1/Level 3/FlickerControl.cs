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
    //allows you to assign the sprite of the holograms to flicker with the light
    public SpriteRenderer hologramSprite;
    // the minimum intensity of the hologram transparency
    public float minAlpha = 0.2f;
    // the maximum intensity of the hologram at its brightest
    public float maxAlpha = 1f;


    // Update is called once per frame
    private void Start()
    {
        light = GetComponent<Light2D>();

        InvokeRepeating("Flicker", 0f, flickerSpeed);
         
        if(hologramSprite == null){
            hologramSprite = GetComponentInParent<Transform>().root.GetComponentInChildren<SpriteRenderer>();
        }
    }

    // the light will be assigned a random intensity between the min and max values
    // which makes the flickering happen
    private void Flicker() 
    {
        float randomIntensity = Random.Range(minIntensity, maxIntensity);
        light.intensity = randomIntensity;

        if(hologramSprite != null){
            float t = Mathf.InverseLerp(minIntensity, maxIntensity, randomIntensity);
            Color c = hologramSprite.color;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
            hologramSprite.color = c;
        }
    }

    
    
}
