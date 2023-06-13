using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a battery, with a certain charge level and max charge,
/// which can be found in an enemy.
/// This script is in proof-of-concept mode: certain things, like a true
/// enemy death sequence, are missing.
/// </summary>
public class EnemyBattery : AControllable
{
    /// <summary> 
    /// This EnemyBattery class has not been implemented, but it logs the amount of energy in an enemy.
    /// </summary>
    private void Update()
    {
        if ((cleanEnergy + virus) / maxCharge != 0 && (cleanEnergy + virus) / maxCharge != 1)
        {
            // Debug.Log("Energy: " + energy / maxEnergy);
        }
        else if ((cleanEnergy + virus) / maxCharge == 0)
        {
            // Debug.Log("Dead: Drained");
        }
        else if ((cleanEnergy + virus) / maxCharge == 1)
        {
            // Debug.Log("Dead: Overloaded");
        }
    }
}
