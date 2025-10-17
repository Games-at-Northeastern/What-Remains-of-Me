using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

/*
Purpose: This class is used to add effects to lighting objects. This can be added to in the
future but make sure any effect is disabled by default.
Currently Supported Light Effects Include:
- Flickering lights
*/

public class LightEffects : MonoBehaviour
{
    [Header("Flickering Lights Feature")]
    [SerializeField] private bool enableFlickerEffect = false;

    [Tooltip("Minimum intensity of the light during flicker.")]
    [SerializeField] private float minIntensity = 0.0f;

    [Tooltip("How quickly the light flickers (seconds).")]
    [SerializeField] private float minFlickerSpeed = 0.1f;

    [Tooltip("How quickly the light flickers (seconds).")]
    [SerializeField] private float maxFlickerSpeed = 0.2f;

    [Tooltip("How many times the light should turn on and off during a 'flicker'")]
    [SerializeField] private float minFlickers = 1;

    [Tooltip("How many times the light should turn on and off during a 'flicker'")]
    [SerializeField] private float maxFlickers = 5;

    [Tooltip("How often should the flicker happen on average (seconds).")]
    [SerializeField] private float flickerInterval = 10f;

    [Tooltip("Variance of flicker interval (+/- seconds).")]
    [SerializeField] private float flickerVariance = 2f;

    [Tooltip("If you want a sound to play each flicker.")] // NOTE: Not totally sure what our audio system is so maybe this would have to be implemented differently
    [SerializeField] private AudioClip flickerSFX;

    private Light2D light;
    private float defaultIntensity;


    // For Flicker Effect Specifically
    private float nextFlickerTimer;
    private bool isFlickerEffectPlaying;

    private void Start()
    {
        light = GetComponent<Light2D>();
        if (light == null)
        {
            Debug.LogWarning("Could not find light component on " + gameObject.name);
        }
        else
        {
            nextFlickerTimer = flickerInterval + Random.Range(-flickerVariance, flickerVariance);
            defaultIntensity = light.intensity;
        }
    }

    private void Update()
    {
        if (light == null)
        {
            return;
        }

        if (enableFlickerEffect)
        {
            FlickerEffect();
        }
    }

    private void FlickerEffect()
    {
        nextFlickerTimer -= Time.deltaTime;

        if ((nextFlickerTimer <= 0f) && (!isFlickerEffectPlaying))
        {
            isFlickerEffectPlaying = true;
            StartCoroutine(FlickerLight());
            nextFlickerTimer = flickerInterval + Random.Range(-flickerVariance, flickerVariance);
            isFlickerEffectPlaying = false;
        }
    }

    private IEnumerator FlickerLight()
    {
        int numberOfFlickers = (int)Random.Range(minFlickers, maxFlickers);
        float flickerSpeed;

        for (int i = 0; i < numberOfFlickers; i++)
        {
            flickerSpeed = Random.Range(minFlickerSpeed, maxFlickerSpeed);
            light.intensity = minIntensity;
            if (flickerSFX != null)
            {
                AudioSource.PlayClipAtPoint(flickerSFX, gameObject.transform.position);
            }
            yield return new WaitForSeconds(flickerSpeed);
            light.intensity = defaultIntensity;
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

}
