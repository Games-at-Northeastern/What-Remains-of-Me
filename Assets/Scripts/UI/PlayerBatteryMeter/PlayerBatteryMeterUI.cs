using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI view of the player's health.
/// </summary>
public class PlayerBatteryMeterUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    public PlayerInfo playerInfo;

    void Start()
    {
        //deleted the instace listener that was originally here becuase we are
        //updating the slider to costantly be the percentage of the battery to
        //max battery in the scriptable object
        UpdateHealthView();
    }

    void UpdateHealthView()
    {
        slider.value = ((float) playerInfo.battery/playerInfo.maxBattery) * slider.maxValue;
    }
}
