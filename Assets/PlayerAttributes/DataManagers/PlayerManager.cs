using System.Collections.Generic;
using PlayerController;
using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerController2D playerObject;
    [SerializeField] private PlayerSettings startingStats;

    public AbilityManager AbilityManager {
        get;
        private set;
    }

    public UpgradeManager UpgradeManager {
        get;
        private set;
    }

    public EnergyManager EnergyManager {
        get;
        private set;
    }

    public PlayerSettings PlayerStats {
        get;
        private set;
    }

    private void Awake()
    {
        // Sets the PlayerRef scriptable object to see this script as the PlayerManager
        PlayerRef.PlayerManager = this;

        // Gets the ability, upgrade and energy managers
        // Assumes the managers are children of the player manager's GameObject 
        AbilityManager = GetComponentInChildren<AbilityManager>();
        UpgradeManager = GetComponentInChildren<UpgradeManager>();
        EnergyManager = GetComponentInChildren<EnergyManager>();

        // Dictionary of player-related managers, this is only used for organizational purposes
        Dictionary<string, IManager> managers = new Dictionary<string, IManager>
        { { nameof(AbilityManager), AbilityManager },
          { nameof(UpgradeManager), UpgradeManager },
          { nameof(EnergyManager), EnergyManager } };

        // Checks to see if all the managers are found
        CheckRequirements(managers);

        // Applies abilities and upgrades to the player character's stats
        SetAbilitiesAndUpgrades();
    }

    // Helper function that checks if all required managers are being referenced
    private void CheckRequirements(Dictionary<string, IManager> managers)
    {
        foreach (KeyValuePair<string, IManager> manager in managers) {
            if (manager.Value == null) {
                Debug.LogError($"PlayerManager doesn't have the {manager.Key} component in children");
            }
        }
    }

    // Reloads all the abilities/upgrades
    // Function made to run if the player picks up an ability or upgrade
    public void SetAbilitiesAndUpgrades()
    {
        // Applies all collected abilities to the player
        AbilityManager.ApplyAbilities(playerObject);

        // Applies all collected upgrades to the player and sets the player's stats being used
        PlayerStats = UpgradeManager.ApplyUpgrades(startingStats);
    }
}
