using UnityEngine;
public class EnergyManager : MonoBehaviour
{

    public static EnergyManager Instance;

    // If the scene wants to start with no energy
    [SerializeField] private bool startWithNoEnergy;

    [SerializeField] private float maxBattery;
    [SerializeField] private float maxVirus;

    // For now, these will be floats, but I do want to change them to integers in the future
    [SerializeField] private float batteryPercentage;
    [SerializeField] private float virusPercentage;

    // Initialize the energy manager in the first moment the scene is run
    private void Awake()
    {
        InitializeManagerInstance();

        batteryPercentage = 0;
        virusPercentage = 0;

        if (!startWithNoEnergy) {
            batteryPercentage = 1;
        }
    }

    // Returns the current battery percentage 
    public float GetBatteryPercentage() => batteryPercentage;

    // Returns the current virus percentage 
    public float GetVirusPercentage() => virusPercentage;

    // Returns the current battery value 
    public float GetBattery() => batteryPercentage * maxBattery;

    // Sets the current battery percentage 
    public void SetBattery(float value) => batteryPercentage = value / maxBattery;

    // Returns the current virus value 
    public float GetVirus() => virusPercentage * maxVirus;

    // Sets the current virus percentage 
    public void SetVirus(float value) => virusPercentage = value / maxVirus;

    // Gets the current maxBattery value 
    public float GetMaxBattery() => maxBattery;

    // Gets the current maxVirus value 
    public float GetMaxVirus() => maxVirus;

    // Initializes and enforces a singular instance of this EnergyManager
    private void InitializeManagerInstance()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(Instance);
    }
}
