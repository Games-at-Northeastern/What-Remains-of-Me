using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBatteryUI : MonoBehaviour
{
    [SerializeField] private PlayerInfo playerEnergy;

    [SerializeField] private Image _batteryUIBar;

    private Material _batteryUIBarMaterial;

    private void Start()
    {
        _batteryUIBarMaterial = _batteryUIBar.material;
        _batteryUIBar.material = _batteryUIBarMaterial;
    }

    private void Update()
    {
        _batteryUIBarMaterial.SetFloat("_Virus_Amount", playerEnergy.virus / 100);
        _batteryUIBarMaterial.SetFloat("_Battery_Amount", playerEnergy.battery / 100);
    }
}
