using System;
using PlayerController;
using SmartScriptableObjects.ReactiveProperties;
using UnityEngine;

public class AtlasFullEnergy : MonoBehaviour
{
    [SerializeField] private PercentageFloatReactivePropertySO playerBattery;

    private void Start()
    {
        playerBattery.Value = 1;
    }
}
