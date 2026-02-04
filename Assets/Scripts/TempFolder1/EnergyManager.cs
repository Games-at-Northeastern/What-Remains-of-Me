using UnityEngine;

// Class holding the player's runtime energy data
public class EnergyManager : IManager
{

    // If the scene wants to start with no energy
    [SerializeField] private bool startWithNoEnergy;

    [SerializeField] private float maxBattery;
    [SerializeField] private float maxVirus;

    // For now, these will be floats, but I do want to change them to integers in the future
    [ReadOnly] [SerializeField] private float batteryPercentage;
    [ReadOnly] [SerializeField] private float virusPercentage;

    // Returns the current battery value 
    public float Battery {
        get => batteryPercentage * maxBattery;

        set => batteryPercentage = value / maxBattery;
    }

    // Returns the current virus value 
    public float Virus {
        get => virusPercentage * maxVirus;
        set => virusPercentage = value / maxVirus;
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
        get => maxBattery;
    }

    // Gets the current maxVirus value 
    public float MaxVirus {
        get => maxVirus;
    }

    // Initialize the energy manager in the first moment the scene is run
    private void Awake()
    {
        Debug.Log(PlayerRef.PlayerManager.AbilityManager.transform.position);
        batteryPercentage = 0;
        virusPercentage = 0;

        if (!startWithNoEnergy) {
            batteryPercentage = 1;
        }
    }
}
