using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkIntensityController : MonoBehaviour
{
    [Header("CHANGE THE VALUE OF THIS SLIDER FOR INTENSITY")]
    [Range(0.0f, 1.0f)]
    [SerializeField] float intensity;
    [SerializeField] bool isActive;

    [Header("These variables are somewhat fine tuned so don't mess with them if you don't have to")]
    [SerializeField] ParticleSystem particleSystem;

    [Header("Emission")]
    [SerializeField] float rateOverTimeLowerMin;
    [SerializeField] float rateOverTimeLowerMax;
    [SerializeField] float rateOverTimeHigherMin;
    [SerializeField] float rateOverTimeHigherMax;

    [Header("Speeds")]
    [SerializeField] float gravityLowerMin;
    [SerializeField] float gravityLowerMax;
    [SerializeField] float gravityHigherMin;
    [SerializeField] float gravityHigherMax;
    [SerializeField] float speedLowerMin;
    [SerializeField] float speedLowerMax;
    [SerializeField] float speedHigherMin;
    [SerializeField] float speedHigherMax;

    [Header("Material/Lighting")]
    [SerializeField] Material sparkMat;
    [SerializeField] Color baseColor;
    [SerializeField] [ColorUsageAttribute(false, true)] Color emissionColor;

    [Header("Audio")]
    [SerializeField] SoundPlayer3D source;
    [SerializeField] float volumeLower;
    [SerializeField] float volumeHigher;

    // Start is called before the first frame update
    private void Start()
    {
        UpdateParticlesAndAudio();

        //if inactive disable volume
        if (!isActive)
        {
            particleSystem.gameObject.SetActive(false);
        }
    }

    void UpdateParticlesAndAudio()
    {
        //if not active return
        if (!isActive)
            return;

        //emission
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        float minRate = Mathf.Lerp(rateOverTimeLowerMin, rateOverTimeHigherMin, intensity);
        float maxRate = Mathf.Lerp(rateOverTimeLowerMax, rateOverTimeHigherMax, intensity);
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(minRate, maxRate);

        //speeds
        ParticleSystem.MainModule main = particleSystem.main;
        float minGravity = Mathf.Lerp(gravityLowerMin, gravityHigherMin, intensity);
        float maxGravity = Mathf.Lerp(gravityHigherMin, gravityHigherMax, intensity);
        main.gravityModifier = new ParticleSystem.MinMaxCurve(minGravity, maxGravity);

        float minSpeed = Mathf.Lerp(speedLowerMin, speedHigherMin, intensity);
        float maxSpeed = Mathf.Lerp(speedLowerMax, speedHigherMax, intensity);
        main.startSpeed = new ParticleSystem.MinMaxCurve(minSpeed, maxSpeed);

        //mat / lighting
        var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        sparkMat.color = baseColor;
        sparkMat.SetColor("_EmissionColor", emissionColor);
        renderer.material = sparkMat;

        //audio
        source.sound.baseVolume = Mathf.Lerp(volumeLower, volumeHigher, intensity);

    }

    private void OnValidate()
    {
        UpdateParticlesAndAudio();
        if(source)
        source.source.volume = Mathf.Lerp(volumeLower, volumeHigher, intensity);
    }
}
