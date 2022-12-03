using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPercentageInitializer : MonoBehaviour
{
    public PlayerInfo playerInfo;

    private void Start()
    {
        playerInfo.batteryPercentage = playerInfo._batteryPercentageSO;
        playerInfo.virusPercentage = playerInfo._virusPercentageSO;
    }
}
