using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class which gives Controllable classes certain utilities,
/// such as a general implementation of the GainEnergy() and LoseEnergy()
/// methods, and a representation of energy.
/// </summary>
public abstract class AControllable : MonoBehaviour, IControllable
{
    [SerializeField] protected float energy;
    [SerializeField] protected float maxEnergy;
    [SerializeField] protected float virus;
    public PlayerInfo playerInfo;

    /// <summary>
    /// This controllable gains the given amount of energy and takes it to the player health.
    /// <param name="amount"> float amount of energy for this controllable to gain </param>
    /// </summary>
    public void GainEnergy(float amount)
    {
        if (amount <= 0 || energy >= maxEnergy || playerInfo.batteryPercentage.Value <= 0)
        {
            return;
        }

        // Can only accept what the player can offer
        amount = Mathf.Min(amount, playerInfo.battery);

        playerInfo.battery -= amount;

        energy = Mathf.Clamp(energy + amount, 0, maxEnergy);
    }

    /// <summary>
    /// This controllable loses the given amount of energy and gives it to the player health.
    /// <param name="amount"> float amount of energy for this controllable to lose </param>
    /// </summary>
    public void LoseEnergy(float amount)
    {
        if (amount <= 0 || energy <= 0 || playerInfo.batteryPercentage.Value >= 1f)
        {
            return;
        }

        // Can only provide what the player can take
        float remainingEmptyBatteryAmount = playerInfo.maxBattery - playerInfo.battery;
        amount = Mathf.Min(remainingEmptyBatteryAmount, amount);

        playerInfo.battery += amount;
        playerInfo.virus += amount;

        energy = Mathf.Clamp(energy - amount, 0, maxEnergy);
        virus = Mathf.Clamp(virus - amount, 0, virus);
    }

    /// <summary>
    /// Returns the percentage of energy that the player has.
    /// </summary>
    public float GetPercentFull()
    {
        return energy / maxEnergy;
    }

    /// <summary>
    /// Can the controllable lose the given amount of energy?
    /// <param name="amount"> float to compare to player energy </param>
    /// </summary>
    bool canLoseEnergy(float amount)
    {
        return energy >= amount;
    }


    /// <summary>
    /// Can the player gain the given amount of energy?
    /// <param name="amount"> float to use as difference from max energy </param>
    /// </summary>
    bool canGainEnergy(float amount)
    {
        return energy <= maxEnergy - amount;
    }
}
