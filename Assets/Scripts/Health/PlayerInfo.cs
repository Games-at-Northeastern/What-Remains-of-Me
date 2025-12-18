using System.Collections.Generic;
using Ink.Runtime;
using UniRx;
using UnityEngine;
using Object = Ink.Runtime.Object;
[CreateAssetMenu]
public class PlayerInfo : ScriptableObject
{
    public float iframesTime;
    public List<UpgradeType> currentActivatedUpgrades;

    // any information about the player we want tracked can
    // be stored here.
    [Header("Info")]
    public Dictionary<UpgradeType, IUpgrade> upgrades = new Dictionary<UpgradeType, IUpgrade>();

    public float battery {
        get {
            return EnergyManager.Instance.GetBattery();
        }
        set {
            EnergyManager.Instance.SetBattery(value);
        }

    }
    public float maxBattery {
        get {
            return EnergyManager.Instance.GetMaxBattery();
        }
        // This shouldn't really be done and in the PlayerHealth script it makes no sense
        set {
            /*batteryPercentage.Value = battery / value;
            _maxBattery = value;*/
        }
    }
    public float virus {
        get {
            return EnergyManager.Instance.GetVirus();
        }

        set {
            EnergyManager.Instance.SetVirus(value);
        }
    }

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
    }



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
