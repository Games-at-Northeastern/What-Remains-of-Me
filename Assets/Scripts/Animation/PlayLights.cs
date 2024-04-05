using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLights : Effects
{
    [SerializeField] Light[] lights;
    public override void PlayEffect() 
    {
        foreach (Light light in lights) 
        {
            light.enabled = true;
        }

    }

    public override void CancelEffect()
    {
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }
}
