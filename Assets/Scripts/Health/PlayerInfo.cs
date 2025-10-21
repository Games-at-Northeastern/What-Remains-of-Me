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
        set => virusPercentage.Value = (value / maxVirus);
    }
    public float maxVirus = 100;
    [SerializeField] private float initialMaxBattery;
    public float iframesTime;
    public Dictionary<UpgradeType, IUpgrade> upgrades = new();
    public List<UpgradeType> currentActivatedUpgrades;

    public VoiceModule.VoiceTypes _currentVoice;
    private void OnValidate()
    {
        batteryPercentage = _batteryPercentageSO;
        virusPercentage = _virusPercentageSO;
        _currentVoice = VoiceModule.VoiceTypes.NONE;
        currentActivatedUpgrades.Clear();
    }

    public void GainModule(UpgradeType type)
    {
        currentActivatedUpgrades.Add(type);
        upgrades[type].Aquire();
    } 

    public void ActivateModules()
    {
        foreach (UpgradeType upgrade in currentActivatedUpgrades)
        {
            upgrades[upgrade].TurnOn();
        }
    }

    public void ResetMaxBattery()
    {
        _maxBattery = initialMaxBattery;
    }
}
