using System;
using UnityEngine;
/// <summary>
///     If this controllable gets enough energy, it tells the given moving elements to activate.
/// </summary>
public class MovingElementControllerScript : AControllable
{
    [SerializeField]
    MovingObjectPathScript movingObjectPath;

    public AudioSource audioSource;

    bool hasMoved;

    /// <summary>
    ///     Updates the platforms by moving them when the battery percentage of the platform is full or empty.
    /// </summary>
    void FixedUpdate()
    {
        if (Math.Abs(GetPercentFull() - 1) < 0.01f && !hasMoved) {
            movingObjectPath.Activate();
            hasMoved = true;

            /// <summary>
            /// Used for when an audio source has to play when something is moving.
            /// </summary>
            if (audioSource) {
                audioSource.Play();
            }
        } else if (GetPercentFull() < 0.99f && hasMoved) {
            movingObjectPath.Deactivate();
            hasMoved = false;

            /// <summary>
            /// Used for when an audio source has to stop when something isn't moving.
            /// </summary>
            if (audioSource) {
                audioSource.Stop();
            }
        }
    }

    //public void ApplyToAll(Action<MovingElement> meFunc) => Array.ForEach(_movingElements, movingElement => meFunc(movingElement));
}
