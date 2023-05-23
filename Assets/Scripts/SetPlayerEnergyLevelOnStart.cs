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
    [SerializeField] private float PlayerVirusStartingValue = 1f;


    void Start()
    {
        ResetHealth();

        // Register the reset health event to occur when the player dies
        LevelManager.Instance.OnPlayerDeath.AddListener(ResetHealth);
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
