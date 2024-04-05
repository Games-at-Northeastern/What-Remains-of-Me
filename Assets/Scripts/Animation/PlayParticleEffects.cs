using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleEffects : Effects
{
    [SerializeField] ParticleSystem[] particleSystems;
    public override void PlayEffect() 
    {
        foreach (ParticleSystem particle in particleSystems) 
        {
            particle.Play();
        }

    }

    public override void CancelEffect()
    {
        foreach (ParticleSystem particle in particleSystems)
        {
            particle.Stop();
        }
    }
}
