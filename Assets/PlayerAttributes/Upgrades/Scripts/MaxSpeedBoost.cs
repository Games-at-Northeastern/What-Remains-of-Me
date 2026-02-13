using System;
using PlayerController;
using UnityEngine;
[CreateAssetMenu(menuName = "Upgrades/MaxSpeedBoost")]
public class MaxSpeedBoost : Upgrade
{
    [Header("Speed Increase Info")]
    [SerializeField] private float speedIncrease;
    public override void ApplyUpgrade(PlayerSettings playerStats)
    {
        switch (upgradeType) {
            case UpgradeType.Additive:
                playerStats.maxRunSpeed += speedIncrease;
                playerStats.maxAirSpeed += speedIncrease;
                break;
            case UpgradeType.Multiplicative:
                playerStats.maxRunSpeed *= speedIncrease;
                playerStats.maxAirSpeed *= speedIncrease;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }
}
