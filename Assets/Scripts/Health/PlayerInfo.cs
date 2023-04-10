using System;
using System.Collections;
using System.Collections.Generic;
using SmartScriptableObjects.ReactiveProperties;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class PlayerInfo : ScriptableObject
{
    [Header("Dependency Injection")]
    [FormerlySerializedAs("_virusSO")]
    public PercentageFloatReactivePropertySO _virusPercentageSO;
    public PercentageFloatReactivePropertySO _batteryPercentageSO;

    // any information about the player we want tracked can
    // be stored here.
    [Header("Info")]
    [HideInInspector] public IReactiveProperty<float> batteryPercentage;

    public float battery
    {
        get => batteryPercentage.Value * maxCharge;
        set => batteryPercentage.Value = value / maxCharge;
    }

    private float _maxCharge;
    public float maxCharge
    {
        get => _maxCharge;
        set
        {
            batteryPercentage.Value = battery / value;
            _maxCharge = value;
        }
    }
    [HideInInspector] public IReactiveProperty<float> virusPercentage;
    public float virus
    {
        get => virusPercentage.Value * maxCharge;
        set => virusPercentage.Value = value / maxCharge;
    }
    
    [SerializeField] private float initialMaxBattery;
    public float iframesTime;

    private void OnValidate()
    {
        //batteryPercentage = _batteryPercentageSO;
        //virusPercentage = _virusPercentageSO;
    }

    public void ResetMaxBattery()
    {
        _maxCharge = initialMaxBattery;
    }
}
