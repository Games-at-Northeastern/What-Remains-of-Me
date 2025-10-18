using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;

public class UpgradeHolder : MonoBehaviour
{
    [SerializeField] private List<IUpgrade> upgradeList;
    [SerializeField] private PlayerInfo playerInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInfo.upgrades.Clear();
        foreach (IUpgrade upgrade in upgradeList)
        {
            playerInfo.upgrades[upgrade.GetUpgradeType()] = upgrade;
        }
        playerInfo.ActivateModules();
    }
}
