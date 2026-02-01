using System.Collections.Generic;
using System.Linq;
using PlayerController;
using UnityEngine;
public class UpgradeManager : IManager
{
    [SerializeField] private List<Upgrade> upgrades;
    private List<Upgrade> copiedUpgrades;

    public PlayerSettings ApplyUpgrades(PlayerSettings playerStats)
    {
        PlayerSettings pStats = Instantiate(playerStats);
        copiedUpgrades = new List<Upgrade>();
        foreach (Upgrade upgrade in upgrades) {
            copiedUpgrades.Add(Instantiate(upgrade));
            copiedUpgrades.Last().ApplyUpgrade(pStats);
        }
        return pStats;
    }
}
