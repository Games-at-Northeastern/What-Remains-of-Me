using PlayerController;
using UnityEngine;
public abstract class Upgrade : ScriptableObject
{
    [Header("Upgrade Info")]
    [SerializeField] protected string upgradeName = "UpgradeName";
    [SerializeField] protected string upgradeDescription = "UpgradeDescription";
    [SerializeField] protected UpgradeType upgradeType = UpgradeType.Additive;

    public abstract void ApplyUpgrade(PlayerSettings playerStats);
    protected enum UpgradeType
    {
        Additive,
        Multiplicative
    }
}
