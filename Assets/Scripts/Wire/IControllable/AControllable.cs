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
        if (playerInfo.battery > amount)
        {
            float initEnergy = energy;
            energy = Mathf.Clamp(energy + amount, 0, maxEnergy);
            playerInfo.battery -= (energy - initEnergy);
        }
    }

    /// <summary> 
    /// This controllable loses the given amount of energy and gives it to the player health.
    /// <param name="amount"> float amount of energy for this controllable to lose </param>
    /// </summary>
    public void LoseEnergy(float amount)
    {
        if (playerInfo.battery > amount)
        {
            // Can lose none / some of energy being taken by player
            float initEnergy = energy;
            float initVirus = virus;
            energy = Mathf.Clamp(energy - amount, 0, maxEnergy);
            virus = Mathf.Clamp(virus - amount, 0, virus);
            playerInfo.battery += (initEnergy - energy);
            playerInfo.virus += (initVirus - virus);
        }
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
