using SmartScriptableObjects.ReactiveProperties;
using UnityEngine;

// NOTE: THIS SCRIPT HAS BEEN FULLY DEPRECATED

public class SetPlayerEnergyLevelOnStart : MonoBehaviour
{

    //Keeps track if this is the games first start. We will probably have to remove this once going to a save/load system.
    public static bool firstStart = true;

    [SerializeField] private PercentageFloatReactivePropertySO playerBatterySO;
    [SerializeField] private PercentageFloatReactivePropertySO playerVirusSO;

    [Header("How much energy the player should start with between 0 and 1")]
    [SerializeField] private float PlayerEnergyStartingValue = 1f;
    [Header("How much virus the player should start with between 0 and 1")]
    [SerializeField] private float PlayerVirusStartingValue;
    [SerializeField] private bool useDefaultValues;

    private void Start()
    {

        if (firstStart || useDefaultValues) {
            ResetHealth();
            firstStart = false;
        }


        // Register the reset health event to occur when the player dies
        LevelManager.OnPlayerDeath.AddListener(ResetVirus);
    }

    /// <summary>
    ///     Resets the player health to the original starting value
    /// </summary>
    private void ResetHealth()
    {
        playerBatterySO.Value = PlayerEnergyStartingValue;
        playerVirusSO.Value = PlayerVirusStartingValue;
    }

    private void ResetVirus() => playerVirusSO.Value = PlayerVirusStartingValue;
}
