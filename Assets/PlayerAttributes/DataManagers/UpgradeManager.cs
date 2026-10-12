using System.Collections.Generic;
using System.Linq;
using PlayerController;
using UnityEngine;
public class UpgradeManager : IManager
{
    [SerializeField] private List<Upgrade> upgrades;

    private List<Upgrade> copiedUpgrades;

    public PlayerSettings ApplyUpgrades(PlayerLoadout loadout)
    {
        PlayerSettings pStats = Instantiate(PlayerRef.PlayerManager.PlayerStats);
        copiedUpgrades = new List<Upgrade>();
        foreach (Upgrade upgrade in loadout.Upgrades)
        {
            copiedUpgrades.Add(Instantiate(upgrade));
            copiedUpgrades.Last().ApplyUpgrade(pStats);
        }

        PlayerRef.PlayerManager.EnergyManager.SetMaxBatteryAndVirus(pStats);
        return pStats;
    }

    public Upgrade[] GetUpgrades() => copiedUpgrades.ToArray();
}
