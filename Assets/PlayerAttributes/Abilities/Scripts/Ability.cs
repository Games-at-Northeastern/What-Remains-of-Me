using PlayerController;
using UnityEngine;
public abstract class Ability : ScriptableObject
{
    [Header("Ability Info")]
    [SerializeField] protected string abilityName = "AbilityName";
    [SerializeField] protected string abilityDescription = "AbilityDescription";
    protected PlayerController2D playerController;

    public void SetPlayerController(PlayerController2D controller) => playerController = controller;

    public abstract void UpdateStatus();
    public abstract void HandleInput();
    public abstract void ExecuteAbility();
}
