using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: https://vintay.medium.com/creating-a-respawn-checkpoint-system-in-unity-13e51faf44f3

public class CheckpointManager : MonoBehaviour
{
    // Checkpoints
    /// <summary>
    /// Represents where the player should be spawned/respawned at the beginning of the level
    /// </summary>
    [SerializeField] private Checkpoint levelStartPoint;
    /// <summary>
    /// Represents the most recent checkpoint that the player has passed through.
    /// </summary>
    [SerializeField] private Checkpoint mostRecentPoint;

    /// <summary>
    /// Since the CheckpointManager is a singleton, makes sure to keep the same instance
    /// of this game object
    /// </summary>
    private void Awake()
    {

    }

    private void Start()
    {
        if (levelStartPoint == null)
        {
            Debug.LogWarning("No level start respawn point set - player will not be able to respawn at the beginning of the level");
        }

        if (mostRecentPoint == null)
        {
            mostRecentPoint = levelStartPoint;
        }
    }

    /// <summary>
    /// Performs any necessary reset functionality
    /// </summary>
    private void ResetCheckpoints()
    {
        mostRecentPoint = levelStartPoint;
    }

    /// <summary>
    /// Respawns the given transform at the appropriate recent checkpoint.
    /// </summary>
    /// <param name="respawnable"></param>
    public void RespawnAtRecent(Transform respawnable) => respawnable.transform.position = mostRecentPoint.transform.position;

    /// <summary>
    /// Respawns the given transform at the appropriate level beginning checkpoint.
    /// </summary>
    /// <param name="respawnable"></param>
    public void RespawnAtBeginning(Transform respawnable) => respawnable.transform.position = levelStartPoint.transform.position;

    public void SetRecentPoint(Checkpoint checkpoint) => mostRecentPoint = checkpoint;
}
