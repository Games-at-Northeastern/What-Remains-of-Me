using UnityEngine;

/// <summary>
/// Activates the platform when it contains enough clean energy.
/// Virus energy does not count toward the requirement.
/// Used for GS-8583: Scene 3 Adjust the platform's energy requirement.
/// </summary>
[RequireComponent(typeof(MovingElementController))]
public class CleanEnergyPlatformController : MonoBehaviour
{
    [SerializeField] private float requiredCleanEnergy = 70f;

    private MovingElementController controller;
    private bool isActive = false;

    private void Awake()
    {
        controller = GetComponent<MovingElementController>();
    }

    private void Update()
    {
        // GetEnergy() returns only clean energy.
        bool hasEnoughEnergy = controller.GetEnergy() >= requiredCleanEnergy;

        // Only update the platform when its state changes.
        if (hasEnoughEnergy == isActive)
        {
            return;
        }

        if (hasEnoughEnergy)
        {
            controller.ApplyToAll(element => element.Activate());

            if (controller.audioSource != null)
            {
                controller.audioSource.Play();
            }
        }
        else
        {
            controller.ApplyToAll(element => element.Deactivate());

            if (controller.audioSource != null)
            {
                controller.audioSource.Stop();
            }
        }

        isActive = hasEnoughEnergy;
    }
}
