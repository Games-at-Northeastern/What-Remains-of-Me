using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class PlayLights : Effects
{
    [SerializeField]
    private Light2D[] lights;

    private void OnValidate() => lights = lights.Where(item => item != null).ToArray();

    public override void PlayEffect()
    {
        foreach (Light2D light in lights) {
            light.enabled = true;
        }

    }

    public override void CancelEffect()
    {
        foreach (Light2D light in lights) {
            light.enabled = false;
        }
    }
}
