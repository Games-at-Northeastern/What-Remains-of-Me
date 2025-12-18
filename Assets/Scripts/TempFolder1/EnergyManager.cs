using UnityEngine;
public class EnergyManager : MonoBehaviour
{

    public static EnergyManager Instance;

    [SerializeField] private bool startWithNoEnergy;
    [SerializeField] private float maxBattery;
    [SerializeField] private float maxVirus;

    // For now these will be floats, but I do want to change them to integers in the future
    [SerializeField] private float batteryPercentage;
    [SerializeField] private float virusPercentage;

    private void Awake()
    {
        InitializeManagerInstance();

        batteryPercentage = 0;
        virusPercentage = 0;

        if (!startWithNoEnergy) {
            batteryPercentage = 1;
        }
    }

    public float GetBatteryPercentage() => batteryPercentage;
    public float GetVirusPercentage() => virusPercentage;

    public float GetBattery() => batteryPercentage * maxBattery;

    public void SetBattery(float value) => batteryPercentage = value / maxBattery;

    public float GetVirus() => virusPercentage * maxVirus;

    public void SetVirus(float value) => virusPercentage = value / maxVirus;

    public float GetMaxBattery() => maxBattery;
    public float GetMaxVirus() => maxVirus;

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
