using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// This is extending energy control because it needs to update playerinfo as well.
// This could also be done in the scriptable object though with, you guessed it, events and listeners...
public class PlayerEnergy : EnergyControl
{
    public PlayerInfo playerInfo;
    public UnityEvent OnHealthChanged;
    public UnityEvent OnDamageTaken;

    private void Start()
    {
        playerInfo.ResetMaxBattery();
    }


    /// <summary>
    /// Represents any necessary steps to handle the player death when they hold their max Virus amount.
    /// </summary>
    private void VirusFullDeath()
    {
        Debug.Log("Death from virus full");
        LevelManager.Instance.PlayerDeath();
    }

    /// <summary>
    /// Represents any necessary steps to handle the player death when their battery level reaches 0 from depletion.
    /// </summary>
    private void EnergyDepletedDeath()
    {
        Debug.Log("Death from energy depleted");
        LevelManager.Instance.PlayerDeath();
    }
    // TODO : trigger this upon energy change in EnergyControl
    private void updatePlayerInfo()
    {

        if (playerInfo.virus >= playerInfo.maxVirus - 0.01)
        {
            VirusFullDeath();
        }
        else if (playerInfo.battery <= 0.01)
        {
            EnergyDepletedDeath();
        }
    }
}
