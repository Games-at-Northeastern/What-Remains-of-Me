using System.Collections.Generic;
using System.Linq;
using PlayerController;
using UnityEngine;
public class AbilityManager : IManager
{
    [SerializeField] private List<Ability> abilities;

    private List<Ability> copiedAbilities;

    private void Awake() => copiedAbilities ??= new List<Ability>();

    // Update is called once per frame
    private void Update()
    {
        foreach (Ability ability in copiedAbilities) {
            ability.HandleInput();
        }
    }

    private void FixedUpdate()
    {
        foreach (Ability ability in copiedAbilities) {
            ability.UpdateStatus();
            ability.ExecuteAbility();
        }
    }

    public void ApplyAbilities(PlayerController2D player)
    {
        copiedAbilities = new List<Ability>();
        foreach (Ability ability in abilities) {
            copiedAbilities.Add(Instantiate(ability));
            copiedAbilities.Last().SetPlayerController(player);
        }
    }
}
