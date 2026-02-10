using System.Collections.Generic;
using System.Linq;
using PlayerController;
using UnityEngine;
public class UpgradeManager : IManager
{
    [SerializeField] private List<Upgrade> upgrades;

    private List<Upgrade> copiedUpgrades;

    public PlayerSettings ApplyUpgrades(PlayerManager playerManager, PlayerLoadout loadout)
    {
        PlayerSettings pStats = Instantiate(playerManager.PlayerStats);
        copiedUpgrades = new List<Upgrade>();
        foreach (Upgrade upgrade in loadout.Upgrades) {
            copiedUpgrades.Add(Instantiate(upgrade));
            copiedUpgrades.Last().ApplyUpgrade(pStats);
        }

        playerManager.EnergyManager.SetMaxBatteryAndVirus(pStats);
        return pStats;
    }

    public Upgrade[] GetUpgrades() => copiedUpgrades.ToArray();
}
