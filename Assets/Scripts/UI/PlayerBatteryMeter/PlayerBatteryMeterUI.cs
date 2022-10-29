using System.Collections;
using System.Collections.Generic;
using UI.PlayerBatteryMeter;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Handles the UI view of the player's health.
/// </summary>
public class PlayerBatteryMeterUI : MonoBehaviour, IPlayerBatteryMeterUI
{
    [FormerlySerializedAs("slider")] [SerializeField] private Slider _slider;
    public PlayerInfo playerInfo;

    public void SetCurrBatteryPercentage(float percentage)
    {
        _slider.value = playerInfo.batteryPercentage.Value * _slider.maxValue;
    }

    public void SetCurrVirusPercentage(float percentage)
    {

    }
}
