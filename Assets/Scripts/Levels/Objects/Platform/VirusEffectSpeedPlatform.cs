using System.Collections;
using System.Collections.Generic;
using Levels.Objects.Platform;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Applies a movement speed 'glitch' effect to the platforms controlled by this object,
/// dependent on the ratio of virus to clean energy in the controller (i.e. a higher virus percentage = more glitching).
/// </summary>
[RequireComponent(typeof(ControllablePlatform))]
public class VirusEffectSpeedPlatform : MonoBehaviour
{
    [SerializeField] private ControllablePlatform platformController;
    [SerializeField] private VisualEffect virusEffect;

    private float currentVirusPercentage;

    private void Start()
    {
        if (platformController == null)
        {
            platformController = GetComponent<ControllablePlatform>();
        }
        platformController.OnVirusChange.AddListener(updatePlatformSpeeds);
        platformController.OnVirusChange.AddListener(updateVirusParticles);

    }

    private void updatePlatformSpeeds(float virusPercentage)
    {
        currentVirusPercentage = virusPercentage;
        platformController.ApplyToPlatforms(modifySpeed);
    }

    private void updateVirusParticles(float virusPercentage)
    {
        if (virusEffect)
        {
            virusEffect.SetFloat("Density", virusPercentage);
        }
    }

    private void modifySpeed(Platform platform)
    {
        platform.SetSpeedModifier((currentVirusPercentage * 3) + 1);
    }
}

