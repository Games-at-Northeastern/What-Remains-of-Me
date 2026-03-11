using PlayerController;
using UnityEngine;

// Class holding the player's runtime energy data
public class EnergyManager : IManager
{
    // For now, these will be floats, but I do want to change them to integers in the future
    [Header("Maximum Battery/Virus Values")]
    [ReadOnly] [SerializeField] private float maxBattery;
    [ReadOnly] [SerializeField] private float maxVirus;

    [Header("Current Battery/Virus Percentages")]
    [ReadOnly] [SerializeField] private float batteryPercentage;
    [ReadOnly] [SerializeField] private float virusPercentage;

    // Returns the current battery value 
    public float Battery {
        get => batteryPercentage * MaxBattery;

        set => batteryPercentage = value / MaxBattery;
    }

    // Returns the current virus value 
    public float Virus {
        get => virusPercentage * MaxVirus;
        set => virusPercentage = value / MaxVirus;
    }

    // Returns the current battery percentage 
    public float BatteryPercentage {
        get => batteryPercentage;
    }

    // Returns the current virus percentage 
    public float VirusPercentage {
        get => virusPercentage;
    }

    // Gets the current maxBattery value 
    public float MaxBattery {
        get;
        private set;
    }

    // Gets the current maxVirus value 
    public float MaxVirus {
        get;
        private set;
    }

    // Initialize the energy manager in the first moment the scene is run
    private void Awake()
    {
        batteryPercentage = 1; // Defaults to full battery
        virusPercentage = 0; // Defaults to no virus
        maxBattery = 100; // Defaults to 100
        maxVirus = 100; // Defaults to 100
    }

    // Only way to set the max battery and virus is using a PlayerSettings scriptable object
    public void SetMaxBatteryAndVirus(PlayerSettings playerStats)
    {
        MaxBattery = playerStats.maxEnergy;
        MaxVirus = playerStats.maxVirus;
    }
}
