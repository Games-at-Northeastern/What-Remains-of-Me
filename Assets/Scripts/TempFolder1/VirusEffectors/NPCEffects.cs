using UnityEngine;
using UnityEngine.VFX;

public class NPCEffects : AControllable
{
    [SerializeField]
    private VisualEffect[] virusEffect;

    [SerializeField]
    private float doVirusEffectAt;

    private float initDensity;

    private void Awake()
    {
        if(virusEffect.Length > 0){
            initDensity = virusEffect[0].GetFloat("Density");
        }
    }

    void Update()
    {
        float? virusPercent = GetVirusPercent();
        float totalEnergy = GetEnergy() + GetVirus();

        if (totalEnergy < 1)
        {
            foreach(var virus in virusEffect){
                virus.SetFloat("Density", 0f);
            }
        } else
        {
            foreach(var virus in virusEffect){
                virus.SetFloat("Density", initDensity);
            }
        }
        if (virusPercent != null) {
            if (virusPercent < doVirusEffectAt) {
                foreach(var virus in virusEffect){
                    virus.SetFloat("Density", 0f);
                }
            }
        }
    }
}