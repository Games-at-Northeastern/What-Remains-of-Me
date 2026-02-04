using System.Collections.Generic;
using PlayerController;
using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerController2D playerObject;
    [SerializeField] private PlayerSettings startingStats;

    public static PlayerManager Instance {
        get;
        private set;
    }

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
        InitializeManagerInstance();
        AbilityManager = GetComponentInChildren<AbilityManager>();
        UpgradeManager = GetComponentInChildren<UpgradeManager>();
        EnergyManager = GetComponentInChildren<EnergyManager>();

        Dictionary<string, IManager> managers = new Dictionary<string, IManager>
        {
            {
                nameof(AbilityManager), AbilityManager
            },
            {
                nameof(UpgradeManager), UpgradeManager
            },
            {
                nameof(EnergyManager), EnergyManager
            }
        };

        CheckRequirements(managers);

        SetAbilitiesAndUpgrades();
    }

    private void CheckRequirements(Dictionary<string, IManager> managers)
    {
        foreach (KeyValuePair<string, IManager> manager in managers) {
            if (manager.Value == null) {
                Debug.LogError($"PlayerManager doesn't have the {manager.Key} component in children");
            }
        }
    }

// Can run if the player gets an ability or upgrade. Reloads all the abilities/upgrades
    public void SetAbilitiesAndUpgrades()
    {
        AbilityManager.ApplyAbilities(playerObject);
        PlayerStats = UpgradeManager.ApplyUpgrades(startingStats);
    }

    private void InitializeManagerInstance()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(Instance);
    }
}
