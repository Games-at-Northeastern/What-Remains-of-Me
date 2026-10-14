using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using Object = Ink.Runtime.Object;
public class PlayerInfo : ScriptableObject
{
    // NOTE: Energy is now handled by a single EnergyManager instance.
    // To migrate away from using scriptable objects for runtime data and not edit a ton of scripts,
    // PlayerInfo now delegates all battery/virus related requests to the EnergyManager instance

    public float iframesTime;
    public List<UpgradeType> currentActivatedUpgrades;

    // any information about the player we want tracked can
    // be stored here.
    [Header("Info")]
    public Dictionary<UpgradeType, IUpgrade> upgrades = new Dictionary<UpgradeType, IUpgrade>();

    /*// Gets/Sets the current battery from the EnergyManager instance
    public float battery {
        get {
            return EnergyManager.Instance.GetBattery();
        }
        set {
            EnergyManager.Instance.SetBattery(value);
        }

    }

    // Gets the current maxBattery from the EnergyManager instance
    public float maxBattery {
        get {
            return EnergyManager.Instance.GetMaxBattery();
        }
        // This shouldn't really be done and in the PlayerHealth script it makes no sense
        set {
            /*batteryPercentage.Value = battery / value;
            _maxBattery = value;#1#
        }
    }

    // Gets/Sets the current virus from the EnergyManager instance
    public float virus {
        get {
            return EnergyManager.Instance.GetVirus();
        }

        set {
            EnergyManager.Instance.SetVirus(value);
        }
    }

    // Gets the current maxVirus from the EnergyManager instance
    public float maxVirus {
        get {
            return EnergyManager.Instance.GetMaxVirus();
        }
    }

    public ReactiveProperty<float> batteryPercentage {
        get {
            return new ReactiveProperty<float>(EnergyManager.Instance.GetBatteryPercentage());
        }
    }
    public ReactiveProperty<float> virusPercentage {
        get {
            return new ReactiveProperty<float>(EnergyManager.Instance.GetVirusPercentage());
        }
    }*/



    private void OnValidate() => currentActivatedUpgrades.Clear();

    public void ResetUpgrades(List<UpgradeType> upgrades)
    {
        currentActivatedUpgrades.Clear();

        foreach (UpgradeType upgrade in upgrades) {
            currentActivatedUpgrades.Add(upgrade);
        }
    }

    public void AddVoices(List<VoiceModule.VoiceTypes> voices)
    {
        foreach (VoiceModule.VoiceTypes type in voices) {
            if (type != VoiceModule.VoiceTypes.NONE) {
                Object obj = new BoolValue(true);
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
        foreach (UpgradeType upgrade in currentActivatedUpgrades) {
            upgrades[upgrade].TurnOn();
        }
    }

    // Because the EnergyManager class handles the current energy, this should do nothing
    public void ResetMaxBattery() {}
}
