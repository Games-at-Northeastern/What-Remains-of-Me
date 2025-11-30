using System;
using UnityEngine;

// NOTE --> This is just a copy of the old MovingElementController with a changed serialized field

/// <summary>
///     If this controller gets enough energy, it tells the given moving elements to activate.
/// </summary>
public class MovingElementControllerScript : AControllable
{
    // Moving element path that can be turned on and off with energy
    [SerializeField] private MovingElementPathScript movingElementPath;

    public AudioSource audioSource;

    private bool hasMoved;

    /// <summary>
    ///     Updates the platforms by moving them when the battery percentage of the platform is full or empty.
    /// </summary>
    private void FixedUpdate()
    {
        if (Math.Abs(GetPercentFull() - 1) < 0.01f && !hasMoved) {
            movingElementPath.Activate();
            hasMoved = true;

            /// <summary>
            /// Used for when an audio source has to play when something is moving.
            /// </summary>
            if (audioSource) {
                audioSource.Play();
            }
        } else if (GetPercentFull() < 0.99f && hasMoved) {
            movingElementPath.Deactivate();
            hasMoved = false;

            /// <summary>
            /// Used for when an audio source has to stop when something isn't moving.
            /// </summary>
            if (audioSource) {
                audioSource.Stop();
            }
        }
    }
}
