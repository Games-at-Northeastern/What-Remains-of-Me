using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartScriptableObjects.ReactiveProperties;

public class SetPlayerEnergyLevelOnStart : MonoBehaviour
{

    [SerializeField] private PercentageFloatReactivePropertySO playerBatterySO;
    [SerializeField] private PercentageFloatReactivePropertySO playerVirusSO;

    [Header("How much energy the player should start with between 0 and 1")]
    [SerializeField] private float PlayerEnergyStartingValue = 1f;
    [Header("How much virus the player should start with between 0 and 1")]
    [SerializeField] private float PlayerVirusStartingValue = 0f;

    //Keeps track if this is the games first start. We will probably have to remove this once going to a save/load system.
    public static bool firstStart = true;
    [SerializeField] private bool useDefaultValues = false;

    void Start()
    {

        if (firstStart || useDefaultValues)
        {
            ResetHealth();
            firstStart = false;
        }


        // Register the reset health event to occur when the player dies
        LevelManager.OnPlayerDeath.AddListener(ResetHealth);
    }

    /// <summary>
    /// Resets the player health to the original starting value
    /// </summary>
    void ResetHealth()
    {
        playerBatterySO.Value = PlayerEnergyStartingValue;
        playerVirusSO.Value = PlayerVirusStartingValue;
    }
}
