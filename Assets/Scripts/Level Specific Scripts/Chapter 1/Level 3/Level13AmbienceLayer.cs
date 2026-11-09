using UnityEngine;

// Starts only the additional Server Room layer through the existing sound system.
public sealed class Level13AmbienceLayer : MonoBehaviour
{
    [SerializeField] private SoundController controller;

    private void Start()
    {
        if (controller != null)
        {
            controller.PlaySound("Level13_AmbienceLayer");
        }
    }
}
