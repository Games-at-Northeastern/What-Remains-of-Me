using PlayerController;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/DoubleJumpTest")]
public class AirJump : Ability
{
    [Header("Jump Settings")]
    [SerializeField] private int airJumps = 1;
    private int currentAirJumps;
    private bool inputReceived;

    public override void UpdateStatus()
    {
        if (playerController.Grounded) {
            currentAirJumps = 0;
        }
    }
    public override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            inputReceived = true;
        }
    }

    public override void ExecuteAbility()
    {
        if (!inputReceived) return;

        inputReceived = false;

        if (!playerController.Grounded &&
            playerController.currentState != PlayerController2D.PlayerState.Swinging &&
            currentAirJumps < airJumps) {
            currentAirJumps++;
            playerController.TriggerJump();
        }
    }
}
