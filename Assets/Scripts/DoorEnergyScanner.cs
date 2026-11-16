using UnityEngine;

/// <summary>
/// Checks Atlas's energy and virus while inside the scan area.
/// Used for the Scene 1.3 task to match the first door's behavior
/// with the Handler's dialogue.
/// </summary>
public class DoorEnergyScanner : MonoBehaviour
{
    [SerializeField] private ControllableDoor door;
    [SerializeField] private float requiredEnergy = 50f;
    [SerializeField] private bool virusAllowed = false;
    [SerializeField] private float openingTime = 1f;

    private bool hasOpened;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || hasOpened || door == null)
        {
            return;
        }

        var playerManager = PlayerRef.PlayerManager;
        if (playerManager == null || playerManager.EnergyManager == null)
        {
            return;
        }

        var energy = playerManager.EnergyManager;

        bool enoughEnergy = energy.Battery >= requiredEnergy;
        bool virusAccepted = virusAllowed || energy.Virus <= 0f;

        if (enoughEnergy && virusAccepted)
        {
            hasOpened = true;
            door.GraduallyFillDoor(openingTime);
        }
    }
}
