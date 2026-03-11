using UnityEngine;
public class EnergyButtons : MonoBehaviour
{
    [Range(0.015f, 1f)]
    [SerializeField] private float buttonEnergyAmount;
    [Range(0f, 1f)]
    [SerializeField] private float buttonVirusAmount;

    private EnergyManager energyManager;

    private void Start() => energyManager = PlayerRef.PlayerManager.EnergyManager;

    public void OnClick()
    {
        energyManager.Battery = energyManager.MaxBattery * buttonEnergyAmount;
        energyManager.Virus = energyManager.MaxVirus * buttonVirusAmount;
    }
}
