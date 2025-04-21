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

        if (totalEnergy <= 1 || (cleanEnergy <= 1 && virusPercent <= 1))
        {
            Debug.Log("total energy less than 0");
            foreach(var v in virusEffect){
                Debug.Log("turn off");
                v.SetFloat("Density", 0f);
            }
        }
        else
        {
            if (virusPercent != null)
            {
                Debug.Log("virus percent is not null");
                if (virusPercent < doVirusEffectAt)
                {
                    Debug.Log("virus percent less than total virus");
                    foreach (var v in virusEffect)
                    {
                        Debug.Log("turn off");
                        v.SetFloat("Density", 0f);
                    }
                }
                else
                {
                    foreach (var v in virusEffect)
                    {
                        v.SetFloat("Density", initDensity);
                    }
                }
            }
            else
            {
                foreach (var v in virusEffect)
                {
                    Debug.Log("turn off");
                    v.SetFloat("Density", 0f);
                }
            }
        } 
    }
}