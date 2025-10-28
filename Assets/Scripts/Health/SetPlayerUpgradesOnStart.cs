using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerUpgradesOnStart : MonoBehaviour
{
    [SerializeField] private PlayerInfo playerInfo;

    [Header("What upgrades should this player have for this scene?")]
    [SerializeField] private List<UpgradeType> upgrades;

    //Keeps track if this is the games first start. We will probably have to remove this once going to a save/load system.
    public static bool firstStart = true;
    [SerializeField] private bool useDefaultValues = false;

    void Awake()
    {

        if (firstStart || useDefaultValues)
        {
            ResetUpgrades();
            firstStart = false;
        }
    }

    void ResetUpgrades()
    {
        playerInfo.ResetUpgrades(upgrades);
    }
}
