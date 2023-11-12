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

    // Gets the clean energy contained within this controllable
    public float GetEnergy()
    {
        return cleanEnergy;
    }

    // Gets the energy cap of this controllable
    public float GetMaxCharge()
    {
        return maxCharge;
    }

    // Gets the virus energy contained within this controllable
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
        if (amount <= 0 || totalEnergy >= maxCharge || playerInfo.battery <= 1f)
        {
            return;
        }

        amount = Mathf.Min(amount, maxCharge - totalEnergy);
        amount = Mathf.Min(amount, playerInfo.battery - 1f);

        float virusProportion = playerInfo.virus / playerInfo.battery;

        playerInfo.battery -= amount;
        playerInfo.virus -= amount * virusProportion;

        cleanEnergy += amount * (1f - virusProportion);
        virus += amount * virusProportion;

        VirusChange(virus / totalEnergy);
        EnergyChange(totalEnergy);

        //Debug.Log("battery: " + cleanEnergy + " clean energy units, " + virus + " virus units.");
        //Debug.Log("player: " + (playerInfo.battery - playerInfo.virus) + " clean energy units, " + playerInfo.virus + " virus units");
    }

    /// <summary>
    /// This controllable gains the given amount of virus and takes it from the player health.
    /// <param name="amount"> float amount of virus for this controllable to gain </param>
    /// </summary>
    public void GainVirus(float amount)
    {
        // in theory this function should be removed, as under this model, it no longer serves a purpose
    }

    /// <summary>
    /// This controllable loses the given amount of energy and gives it to the player health.
    /// <param name="amount"> float amount of energy for this controllable to lose </param>
    /// </summary>
    public void LoseEnergy(float amount)
    {
        if (amount <= 0 || totalEnergy <= 0 || playerInfo.battery >= playerInfo.maxBattery)
        {
            return;
        }

        amount = Mathf.Min(amount, totalEnergy);
        amount = Mathf.Min(amount, playerInfo.maxBattery - playerInfo.battery);

        float virusProportion = virus / totalEnergy;

        playerInfo.battery += amount;
        playerInfo.virus += amount * virusProportion;

        cleanEnergy -= amount * (1f - virusProportion);
        virus -= amount * virusProportion;

        VirusChange(virus / totalEnergy);
        EnergyChange(totalEnergy);

        //Debug.Log("battery: " + cleanEnergy + " clean energy units, " + virus + " virus units.");
        //Debug.Log("player: " + (playerInfo.battery - playerInfo.virus) + " clean energy units, " + playerInfo.virus + " virus units");
    }

    /// <summary>
    /// This controllable loses the given amount of virus and gives it to the player health.
    /// <param name="amount"> float amount of virus for this controllable to lose </param>
    /// </summary>
    public void LoseVirus(float amount)
    {
        // in theory this function should be removed, as under this model, it no longer serves a purpose
    }

    /// <summary>
    /// Returns the percentage of energy that the player has.
    /// </summary>
    public float GetPercentFull()
    {
        return totalEnergy / maxCharge;
    }

    /// <summary>
    /// Can the controllable lose the given amount of energy?
    /// <param name="amount"> float to compare to player energy </param>
    /// </summary>
    bool canLoseEnergy(float amount)
    {
        return totalEnergy >= amount;
    }


    /// <summary>
    /// Can the player gain the given amount of energy?
    /// <param name="amount"> float to use as difference from max energy </param>
    /// </summary>
    bool canGainEnergy(float amount)
    {
        return totalEnergy + amount <= maxCharge;
    }

    //Debug Control
    /*
    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log("player virus percentage: " + (playerInfo.virus / playerInfo.battery));
        }
    }
    */
}
