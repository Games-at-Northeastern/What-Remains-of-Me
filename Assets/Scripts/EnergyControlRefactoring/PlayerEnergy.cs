using UnityEngine;
using UnityEngine.Events;


// This is extending energy control because it needs to update playerinfo as well.
// This could also be done in the scriptable object though with, you guessed it, events and listeners...
public class PlayerEnergy : EnergyControl
{
    public UnityEvent OnHealthChanged;
    public UnityEvent OnDamageTaken;

    /// <summary>
    ///     Represents any necessary steps to handle the player death when they hold their max Virus amount.
    /// </summary>
    private void VirusFullDeath()
    {
        Debug.Log("Death from virus full");
        LevelManager.PlayerDeath();
    }

    /// <summary>
    ///     Represents any necessary steps to handle the player death when their battery level reaches 0 from depletion.
    /// </summary>
    private void EnergyDepletedDeath()
    {
        Debug.Log("Death from energy depleted");
        LevelManager.PlayerDeath();
    }
    // TODO : trigger this upon energy change in EnergyControl
    private void updatePlayerInfo()
    {

        if (EnergyManager.Instance.Virus >= EnergyManager.Instance.MaxVirus - 0.01) {
            VirusFullDeath();
        } else if (EnergyManager.Instance.Battery <= 0.01) {
            EnergyDepletedDeath();
        }
    }
}
