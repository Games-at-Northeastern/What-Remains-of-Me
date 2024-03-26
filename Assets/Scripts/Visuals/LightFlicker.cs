using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] float chanceToFlickerPerFrame = 0.02f;
    [SerializeField] float minDarknessTime = 0.1f;
    [SerializeField] float maxDarknessTime = 0.3f;

    Light2D light;
    float intensity;

    bool dark = false;

    // Start is called before the first frame update
    void Awake()
    {
        light = GetComponent<Light2D>();
        intensity = light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if(!dark && Random.Range(0f, 1f) <= chanceToFlickerPerFrame)
        {
            dark = true;
            Invoke("resetDark", Random.Range(minDarknessTime, maxDarknessTime));
        }

        if (dark)
            light.intensity = 0f;
        else
            light.intensity = intensity;
    }

    void resetDark()
    {
        dark = false;
    }
}
