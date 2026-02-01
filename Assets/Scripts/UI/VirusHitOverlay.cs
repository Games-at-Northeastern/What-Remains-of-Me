using UnityEngine;
public class VirusHitOverlay : MonoBehaviour
{
    [SerializeField] private Material overlayMaterial;

    [SerializeField] private PlayerInfo playerEnergy;

    [SerializeField] private float minVirusThreshold; // Minimum amount that the virus has to increase in a single Update for the effect to start at its lowest level
    [SerializeField] private float maxVirusThreshold; // Maximum amount that the virus has to increase in a single Update for the effect to start at its lowest level
    [SerializeField] private float minIntensity; // Lower end of the intensity range to display when active (0 - 1)
    [SerializeField] private float maxIntensity; // Upper end of the intensity range to display when active (0 - 1)

    [SerializeField] private float lastVirusLevel;

    private EnergyManager energyManager;

    // Start is called before the first frame update
    private void Start()
    {
        energyManager = PlayerManager.Instance.EnergyManager;
        overlayMaterial.SetFloat("_Intensity", 0);
        lastVirusLevel = energyManager.Virus;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float virusIncrease = energyManager.Virus - lastVirusLevel;
        float virusThresholdRange = maxVirusThreshold - minVirusThreshold;
        float virusValueInRange = Mathf.Clamp((virusIncrease - minVirusThreshold) / virusThresholdRange, 0, 1);
        float intensityRange = maxIntensity - minIntensity;
        float intensityValue = minIntensity + virusValueInRange * intensityRange;

        if (virusIncrease > minVirusThreshold) {
            overlayMaterial.SetFloat("_Intensity", intensityValue);
        } else {
            overlayMaterial.SetFloat("_Intensity", 0);
        }

        lastVirusLevel = energyManager.Virus;
    }
}
