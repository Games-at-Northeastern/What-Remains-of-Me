using System.Collections;
using System.Collections.Generic;
using Levels.Objects.Platform;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Adjusts a visual effect to have more or less particles depending on the percentage of virus in
/// the connected energy-controlled object (e.g. platform, door, etc.)
/// </summary>
[RequireComponent(typeof(VisualEffect))]
public class VirusParticles : MonoBehaviour
{
    [SerializeField] private AControllable controllable;
    [SerializeField] private VisualEffect virusEffect;

    // Start is called before the first frame update
    void Start()
    {
        if (virusEffect == null)
        {
            virusEffect = GetComponent<VisualEffect>();
        }
        controllable.OnVirusChange.AddListener(updateVirusParticles);
    }

    private void updateVirusParticles(float virusPercentage)
    {
        if (virusEffect)
        {
            virusEffect.SetFloat("Density", virusPercentage);
        }
    }
}
