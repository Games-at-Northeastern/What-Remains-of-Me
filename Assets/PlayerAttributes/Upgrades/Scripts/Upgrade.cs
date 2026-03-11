using System.Collections.Generic;
using System.Reflection;
using PlayerController;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Upgrade")]
public class Upgrade : ScriptableObject
{

    [Header("Upgrade Info")]
    [SerializeField] protected string upgradeName = "UpgradeName";
    [SerializeField] protected string upgradeDescription = "UpgradeDescription";
    public List<StatChange> StatChanges;

    public void ApplyUpgrade(PlayerSettings playerStats)
    {
        foreach (StatChange reference in StatChanges) {
            FieldInfo field = typeof(PlayerSettings).GetField(reference.fieldName);
            if (field == null || field.FieldType != typeof(float) && field.FieldType != typeof(int)) {
                Debug.LogError($"Field {reference.fieldName} can't be found in PlayerSettings");
                continue;
            }

            switch (reference.upgradeType) {
                case StatChangeType.Additive:
                    field.SetValue(playerStats, (float)field.GetValue(playerStats) + reference.upgradeNum);
                    break;
                case StatChangeType.Multiplicative:
                    field.SetValue(playerStats, (float)field.GetValue(playerStats) * reference.upgradeNum);
                    break;
            }
        }
    }
}
