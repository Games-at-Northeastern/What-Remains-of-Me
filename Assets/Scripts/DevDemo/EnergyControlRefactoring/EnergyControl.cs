using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyControl : MonoBehaviour
{
    [SerializeField] protected float cleanEnergy;
    [SerializeField] protected float virus;
    [SerializeField] protected float maxTotalEnergy;
    protected float totalEnergy => cleanEnergy + virus;


    [SerializeField] private bool canOvercharge = false;


    public float GetEnergy()
    {
        return cleanEnergy;
    }

    public float GetMaxCharge()
    {
        return maxTotalEnergy;
    }

    public float GetVirus()
    {
        return virus;
    }

    // TODO : we may also want to have a method that simply takes in an energy amount and a prefered ratio
    // This could be more abstracted and prevent repeated code - it also allows for a simpler way to do 
    // i.e. TransferEnergy(float amount, float virusPercentage) { actualVirusAmount = Mathf.Min(amount * virusPercentage, virus)... }

    /// <summary>
    /// This controllable gains the given amount of energy, within it's current thresholds.
    /// <param name="amount"> float amount of energy for this object to gain </param>
    /// <returns>The amount of energy that was actually gained by this object</returns>
    /// </summary>
    public float GainEnergy(float amount)
    {
        return GainEnergyOrVirus(amount, false);
    }

    /// <summary>
    /// This controllable gains the given amount of virus, within it's current thresholds.
    /// <param name="amount"> float amount of virus for this object to gain </param>
    /// <returns>The amount of virus that was actually gained by this object</returns>
    /// </summary>
    public float GainVirus(float amount)
    {
        return GainEnergyOrVirus(amount, true);
    }

    private float GainEnergyOrVirus(float amount, bool isVirus)
    {
        if (amount <= 0 || !canGainEnergy())
        {
            return 0;
        }

        float energyGained = Mathf.Min(amount, maxTotalEnergy - totalEnergy);
        if (isVirus)
        {
            virus += energyGained;
        }
        else
        {
            cleanEnergy += energyGained;
        }

        return energyGained;
    }

    /// <summary>
    /// This controllable loses the given amount of energy, within its thresholds.
    /// <param name="amount"> float amount of energy for this controllable to lose </param>
    /// <returns>The amount of energy that was actually lost by this object</returns>

    /// </summary>
    public float LoseEnergy(float amount)
    {
        if (amount <= 0 || cleanEnergy <= 0)
        {
            return 0;
        }

        float energyLost = Mathf.Min(amount, cleanEnergy);
        cleanEnergy -= energyLost;
        return energyLost;
    }

    /// <summary>
    /// This controllable loses the given amount of virus, within its thresholds.
    /// <param name="amount"> float amount of virus for this controllable to lose </param>
    /// <returns>The amount of virus that was actually lost by this object</returns>
    /// </summary>
    public float LoseVirus(float amount)
    {
        if (amount <= 0 || virus <= 0)
        {
            return 0;
        }

        float energyLost = Mathf.Min(amount, virus);
        virus -= energyLost;
        return energyLost;
    }

    // TODO : This could likely all be bundled into one function that takes in a few more parameters
    // or an 'EnergyRequest' object that contains this information

    /// <summary>
    /// This controllable gains the given amount of energy from the specified object, within it's current thresholds.
    /// <param name="amount"> float amount of energy for this object to gain </param>
    /// <param name="targetEnergy"> The object to take the energy from </param>
    /// </summary>
    public void GainEnergyFrom(EnergyControl targetEnergy, float amount)
    {
        float actualEnergyGained = targetEnergy.LoseEnergy(amount);
        cleanEnergy += actualEnergyGained;
    }

    /// <summary>
    /// This controllable gains the given amount of virus from the specified object, within it's current thresholds.
    /// <param name="amount"> float amount of virus for this object to gain </param>
    /// <param name="targetEnergy"> The object to take the virus from </param>
    /// </summary>
    public void GainVirusFrom(EnergyControl targetEnergy, float amount)
    {
        float actualVirusGained = targetEnergy.LoseVirus(amount);
        virus += actualVirusGained;
    }


    /// <summary>
    /// This controllable loses the given amount of energy from the specified object, within it's current thresholds.
    /// <param name="amount"> float amount of energy for this object to lose </param>
    /// <param name="targetEnergy"> The object to gain the energy </param>
    /// </summary>
    public void LoseEnergyTo(EnergyControl targetEnergy, float amount)
    {
        float actualEnergyLost = targetEnergy.GainEnergy(amount);
        cleanEnergy -= actualEnergyLost;
    }

    /// <summary>
    /// This controllable loses the given amount of virus from the specified object, within it's current thresholds.
    /// <param name="amount"> float amount of virus for this object to lose </param>
    /// <param name="targetEnergy"> The object to gain the virus </param>
    /// </summary>
    public void LoseVirusTo(EnergyControl targetEnergy, float amount)
    {
        float actualVirusLost = targetEnergy.GainVirus(amount);
        virus += actualVirusLost;
    }

    private bool canGainEnergy()
    {
        return totalEnergy < maxTotalEnergy || canOvercharge;
    }
}
