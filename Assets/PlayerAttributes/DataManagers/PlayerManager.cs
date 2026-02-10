using System.Collections.Generic;
using PlayerController;
using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerController2D playerController;
    [SerializeField] private PlayerLoadout startingLoadout;
    [SerializeField] private PlayerLoadout godModeLoadout;

    private PlayerLoadout currentLoadout;

    // Getters and setters for the managers/stat-holding scriptable objects
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

    public PlayerController2D PlayerController {
        get => playerController;
    }

    public bool GodMode {
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
        PlayerStats = Instantiate(startingLoadout.BasePlayerStats);
        EnergyManager.SetMaxBatteryAndVirus(PlayerStats);
        // Applies abilities and upgrades to the player character's stats
        SetAbilitiesAndUpgrades(startingLoadout);
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

    private void SetLoadout(PlayerLoadout loadout)
    {
        Debug.Log(loadout.BasePlayerStats.maxRunSpeed);
        PlayerStats = Instantiate(loadout.BasePlayerStats);
        SetAbilitiesAndUpgrades(loadout);
        EnergyManager.SetMaxBatteryAndVirus(PlayerStats);
        Debug.Log(PlayerStats.maxRunSpeed);
    }

    // Reloads all the abilities/upgrades
    private void SetAbilitiesAndUpgrades(PlayerLoadout loadout)
    {
        // Applies all collected abilities to the player
        AbilityManager.ApplyAbilities(this, loadout);

        // Applies all collected upgrades to the player and sets the player's stats being used
        PlayerStats = UpgradeManager.ApplyUpgrades(this, loadout);

        playerController.SetPlayerStats(this);
    }

    public void SetGodModeStats(GodModeButton button, bool godModeOn)
    {
        if (godModeOn) {
            currentLoadout = ScriptableObject.CreateInstance<PlayerLoadout>();
            currentLoadout.Init(Instantiate(PlayerStats), AbilityManager.GetAbilities(), UpgradeManager.GetUpgrades());
            SetLoadout(godModeLoadout);
            GodMode = true;
        } else {
            SetLoadout(currentLoadout);
            GodMode = false;
        }
    }
}
