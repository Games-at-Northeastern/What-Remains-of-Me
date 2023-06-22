using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Abstract class which gives Controllable classes certain utilities,
/// such as a general implementation of the GainEnergy() and LoseEnergy()
/// methods, and a representation of energy.
/// </summary>
public abstract class AControllable : MonoBehaviour, IControllable
{
    [SerializeField] protected float cleanEnergy;
    [SerializeField] protected float maxCharge;
    [SerializeField] protected float virus;

    protected float totalEnergy => cleanEnergy + virus;

    public PlayerInfo playerInfo;

    public float GetEnergy()
    {
        return cleanEnergy;
    }

    public float GetMaxCharge()
    {
        return maxCharge;
    }

    public float GetVirus()
    {
        return virus;
    }

    // Event to be triggered whenever the virus amount changes,
    // sending the current percentage of energy that is virus as a value between 0 and 1
    public UnityEvent<float> OnVirusChange;
    public void VirusChange(float virusPercentage) => OnVirusChange?.Invoke(virusPercentage);

    public UnityEvent<float> OnEnergyChange;
    public void EnergyChange(float totalEnergyAmount) => OnEnergyChange?.Invoke(totalEnergyAmount);

    /// <summary>
    /// This controllable gains the given amount of energy and takes it from the player health.
    /// <param name="amount"> float amount of energy for this controllable to gain </param>
    /// </summary>
    public void GainEnergy(float amount)
    {
        if (amount <= 0 || (cleanEnergy + virus) >= maxCharge || playerInfo.batteryPercentage.Value <= 0)
        {
            return;
        }

        // Can only accept what the player can offer
        amount = Mathf.Min(amount, playerInfo.battery);

        playerInfo.battery -= amount;

        cleanEnergy = Mathf.Clamp(cleanEnergy + amount, 0, maxCharge);
        EnergyChange(totalEnergy);
    }

    /// <summary>
    /// This controllable gains the given amount of virus and takes it from the player health.
    /// <param name="amount"> float amount of virus for this controllable to gain </param>
    /// </summary>
    public void GainVirus(float amount)
    {
        if (amount <= 0 || (cleanEnergy + virus) >= maxCharge || playerInfo.virusPercentage.Value <= 0)
        {
            return;
        }

        // Can only accept what the player can offer
        amount = Mathf.Min(amount, playerInfo.virus);

        playerInfo.virus -= amount;
        playerInfo.battery -= amount;



        virus = Mathf.Clamp(virus + amount, 0, maxCharge);
        VirusChange(virus / totalEnergy);
        EnergyChange(totalEnergy);
    }

    /// <summary>
    /// This controllable loses the given amount of energy and gives it to the player health.
    /// <param name="amount"> float amount of energy for this controllable to lose </param>
    /// </summary>
    public void LoseEnergy(float amount)
    {
        if (amount <= 0 || cleanEnergy <= 0 || playerInfo.batteryPercentage.Value >= 1f)
        {
            return;
        }

        // Can only provide what the player can take
        float remainingEmptyBatteryAmount = playerInfo.maxBattery - playerInfo.battery;
        amount = Mathf.Min(remainingEmptyBatteryAmount, amount);

        playerInfo.battery += amount;

        cleanEnergy = Mathf.Clamp(cleanEnergy - amount, 0, maxCharge);
        EnergyChange(totalEnergy);
    }

    /// <summary>
    /// This controllable loses the given amount of virus and gives it to the player health.
    /// <param name="amount"> float amount of virus for this controllable to lose </param>
    /// </summary>
    public void LoseVirus(float amount)
    {
        if (amount <= 0 || virus <= 0 || playerInfo.virusPercentage.Value >= 1f)
        {
            return;
        }

        // Can only provide what the player can take
        float remainingEmptyVirusAmount = playerInfo.maxVirus - playerInfo.virus;
        amount = Mathf.Min(remainingEmptyVirusAmount, amount);

        playerInfo.virus += amount;
        playerInfo.battery += amount;


        virus = Mathf.Clamp(virus - amount, 0, maxCharge);
        VirusChange(virus / totalEnergy);
        EnergyChange(totalEnergy);
    }

    /// <summary>
    /// Returns the percentage of energy that the player has.
    /// </summary>
    public float GetPercentFull()
    {
        return (cleanEnergy + virus) / maxCharge;
    }

    /// <summary>
    /// Can the controllable lose the given amount of energy?
    /// <param name="amount"> float to compare to player energy </param>
    /// </summary>
    bool canLoseEnergy(float amount)
    {
        return cleanEnergy >= amount;
    }


    /// <summary>
    /// Can the player gain the given amount of energy?
    /// <param name="amount"> float to use as difference from max energy </param>
    /// </summary>
    bool canGainEnergy(float amount)
    {
        return cleanEnergy <= maxCharge - amount;
    }
}
