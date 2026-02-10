using PlayerController;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/PlayerLoadout")]
public class PlayerLoadout : ScriptableObject
{
    public PlayerSettings BasePlayerStats;
    public Ability[] Abilities;
    public Upgrade[] Upgrades;

    public void Init(PlayerSettings basePlayerStats, Ability[] abilities, Upgrade[] upgrades)
    {
        BasePlayerStats = basePlayerStats;
        Abilities = abilities;
        Upgrades = upgrades;
    }
}
