using SmartScriptableObjects.ReactiveProperties;
using UnityEngine;

public class EnergyButtons : MonoBehaviour
{
    [Range (0.015f, 1f)]
    [SerializeField] private float buttonEnergyAmount;
    [Range(0f, 1f)]
    [SerializeField] private float buttonVirusAmount;

    [SerializeField] private PercentageFloatReactivePropertySO playerEnergy;
    [SerializeField] private PercentageFloatReactivePropertySO playerVirusAmount;

    public void OnClick() {
        playerEnergy.Value = buttonEnergyAmount;
        playerVirusAmount.Value = buttonVirusAmount;
    }
}
