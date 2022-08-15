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

    public void GainEnergy(float amount)
    {
        if (PlayerHealth.CanGiveEnergy(amount))
        {
            float initEnergy = energy;
            energy = Mathf.Clamp(energy + amount, 0, maxEnergy);
            PlayerHealth.instance.LoseEnergy(energy - initEnergy);
        }
    }

    public void LoseEnergy(float amount)
    {
        if (PlayerHealth.CanTakeEnergy(amount))
        {
            // Can lose none / some of energy being taken by player
            float initEnergy = energy;
            float initVirus = virus;
            energy = Mathf.Clamp(energy - amount, 0, maxEnergy);
            virus = Mathf.Clamp(virus - amount, 0, virus);
            PlayerHealth.instance.GainEnergy(initEnergy - energy);
            PlayerHealth.instance.AddVirus(initVirus - virus);
        }
    }

    public float GetPercentFull()
    {
        return energy / maxEnergy;
    }

    bool canLoseEnergy(float amount)
    {
        return energy >= amount;
    }

    bool canGainEnergy(float amount)
    {
        return energy <= maxEnergy - amount;
    }
}
