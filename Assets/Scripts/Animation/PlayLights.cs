using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayLights : Effects
{
    [SerializeField] Light2D[] lights;
    public override void PlayEffect()
    {
        foreach (Light2D light in lights)
        {
            light.enabled = true;
        }

    }

    public override void CancelEffect()
    {
        foreach (Light2D light in lights)
        {
            light.enabled = false;
        }
    }
}
