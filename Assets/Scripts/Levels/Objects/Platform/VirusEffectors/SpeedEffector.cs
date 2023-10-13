using System.Collections;
using System.Collections.Generic;
using Levels.Objects.Platform;
using UnityEngine;

// TODO: Is this script deprecated? There's another one just like it titled "VirusEffectSpeedPlatform," but it
// deals with the regular speed modifier. It also uses a VFX graph instead of a PS.

/// <summary>
/// Applies a movement speed 'glitch' effect to the platforms controlled by this object,
/// dependent on the ratio of virus to clean energy in the controller (i.e. a higher virus percentage = more glitching).
/// </summary>
[RequireComponent(typeof(MovingElementController))]
public class SpeedEffector : MonoBehaviour
{
    [SerializeField] private MovingElementController platformController;
    [SerializeField] private ParticleSystem virusParticles;

    private float currentVirusPercentage;

    private void Start()
    {
        if (platformController == null)
        {
            platformController = GetComponent<MovingElementController>();
        }
        platformController.OnVirusChange.AddListener(updatePlatformSpeeds);
        platformController.OnVirusChange.AddListener(updateVirusParticles);

    }

    private void updatePlatformSpeeds(float virusPercentage)
    {
        currentVirusPercentage = virusPercentage;
        platformController.ApplyToAll(modifySpeed);
    }

    private void updateVirusParticles(float virusPercentage)
    {
        if (virusParticles)
        {
            virusParticles.maxParticles = Mathf.FloorToInt(virusPercentage * 8);
        } 
    }

    private void modifySpeed(MovingElement platform)
    {
        platform.SetRandomSpeedModifier(currentVirusPercentage * 5);
    }
}
