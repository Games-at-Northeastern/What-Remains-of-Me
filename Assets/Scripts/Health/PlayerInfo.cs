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



    private void OnValidate()
    {
        batteryPercentage = _batteryPercentageSO;
        virusPercentage = _virusPercentageSO;
        currentActivatedUpgrades.Clear();
    }

    public void ResetUpgrades(List<UpgradeType> upgrades)
    {
        currentActivatedUpgrades.Clear();

        foreach(var upgrade in upgrades)
        { 
            currentActivatedUpgrades.Add(upgrade);
        }
    }

    public void AddVoices(List<VoiceModule.VoiceTypes> voices)
    {
        foreach (var type in voices)
        {
            if(type != VoiceModule.VoiceTypes.NONE)
            {
                Ink.Runtime.Object obj = new Ink.Runtime.BoolValue(true);
                // call the DialogueManager to set the variable in the globals dictionary
                InkDialogueManager.GetInstance().SetVariableState(VoiceModule.VoiceTypeString(type), obj);
            }
        }
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
