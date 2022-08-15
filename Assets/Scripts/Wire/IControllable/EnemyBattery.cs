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
    private void Update()
    {
        if (energy / maxEnergy != 0 && energy / maxEnergy != 1)
        {
            // Debug.Log("Energy: " + energy / maxEnergy);
        }
        else if (energy / maxEnergy == 0)
        {
            // Debug.Log("Dead: Drained");
        }
        else if (energy / maxEnergy == 1)
        {
            // Debug.Log("Dead: Overloaded");
        }
    }
}
