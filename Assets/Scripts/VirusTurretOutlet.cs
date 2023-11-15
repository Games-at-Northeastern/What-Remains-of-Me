using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VirusTurretOutlet : AControllable
{
    [SerializeField] private VirusTurret virusTurret;
    [SerializeField] private Light2D[] turretLights;

    private void Start()
    {
        if (virusTurret == null)
        {
            virusTurret = GetComponentInParent<VirusTurret>();
        }

        OnEnergyChange.AddListener(CheckForTurretPower);
    }

    private void CheckForTurretPower(float totalEnergyAmount)
    {
        if (totalEnergyAmount <= 0.02f && virusTurret.turnedOn)
        {
            virusTurret.turnedOn = false;
            Array.ForEach(turretLights, turretLight => turretLight.gameObject.SetActive(false));
        }
        else if (totalEnergyAmount >= 0.02f && !virusTurret.turnedOn)
        {
            virusTurret.turnedOn = true;
            Array.ForEach(turretLights, turretLight => turretLight.gameObject.SetActive(true));
        }
    }
}
