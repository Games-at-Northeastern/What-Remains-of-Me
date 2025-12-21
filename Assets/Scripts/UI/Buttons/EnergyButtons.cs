using UnityEngine;
public class EnergyButtons : MonoBehaviour
{
    [Range(0.015f, 1f)]
    [SerializeField] private float buttonEnergyAmount;
    [Range(0f, 1f)]
    [SerializeField] private float buttonVirusAmount;

    public void OnClick()
    {
        EnergyManager.Instance.SetBattery(EnergyManager.Instance.GetMaxBattery() * buttonEnergyAmount);
        EnergyManager.Instance.SetVirus(EnergyManager.Instance.GetMaxVirus() * buttonVirusAmount);
    }
}
