using UnityEngine;
using UnityEngine.UI;
public class SetBatteryUI : MonoBehaviour
{
    [SerializeField] private Image _batteryUIBar;

    private void Start() => _batteryUIBar.material = new Material(_batteryUIBar.material);

    private void Update()
    {
        _batteryUIBar.material.SetFloat("_Virus_Amount", PlayerRef.PlayerManager.EnergyManager.VirusPercentage);
        _batteryUIBar.material.SetFloat("_Battery_Amount", PlayerRef.PlayerManager.EnergyManager.BatteryPercentage);
    }
}
