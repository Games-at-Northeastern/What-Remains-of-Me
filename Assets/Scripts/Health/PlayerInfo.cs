using System;
using System.Collections;
using System.Collections.Generic;
using SmartScriptableObjects.FloatEvent;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class PlayerInfo : ScriptableObject
{
    [Header("Dependency Injection")]
    [FormerlySerializedAs("_virusSO")]
    [SerializeField] private PercentageFloatReactivePropertySO _virusPercentageSO;
    [SerializeField] private PercentageFloatReactivePropertySO _batteryPercentageSO;

    // any information about the player we want tracked can
    // be stored here.
    [Header("Info")]
    [HideInInspector] public IReactiveProperty<float> batteryPercentage;

    public float battery
    {
        get => batteryPercentage.Value * maxBattery;
        set => batteryPercentage.Value = value / maxBattery;
    }

    private float _maxBattery;
    public float maxBattery
    {
        get => _maxBattery;
        set
        {
            batteryPercentage.Value = battery / value;
            _maxBattery = value;
        }
    }
    [HideInInspector] public IReactiveProperty<float> virusPercentage;
    public float virus
    {
        get => virusPercentage.Value * maxVirus;
        set => virusPercentage.Value = value / maxVirus;
    }
    public float maxVirus;
    public float iframesTime;

    private void OnValidate()
    {
        batteryPercentage = _batteryPercentageSO;
        virusPercentage = _virusPercentageSO;
    }
}
