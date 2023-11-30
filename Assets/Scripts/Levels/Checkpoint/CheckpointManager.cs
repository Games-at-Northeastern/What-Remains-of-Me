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

    private void Start()
    {
        if (LevelManager.Instance.holdingCheckpoint()) {
            Vector2 teleportPoint = LevelManager.Instance.extractRecentCheckpoint();
            GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().position = teleportPoint;
        }

        if (mostRecentPoint == null)
        {
            mostRecentPoint = levelStartPoint;
        }

        if (levelStartPoint == null)
        {
            Debug.LogWarning("No level start respawn point set - player will not be able to respawn at the beginning of the level");
        }
        else
        {
            StartCoroutine(RespawnOnStart());
        }
    }

    private IEnumerator RespawnOnStart()
    {
        yield return new WaitForEndOfFrame();
        // If there is a 'starting' chamber at the beginning of the scene, have the player
        // come out of that respawn chamber.
        LevelManager.Instance.PlayerReset();
        yield return null;
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
    public void RespawnAtRecent(Transform respawnable)
    {
        respawnable.transform.position = mostRecentPoint.getRespawnPosition();
        mostRecentPoint.RespawnStart();
    }

    /// <summary>
    /// Respawns the given transform at the appropriate level beginning checkpoint.
    /// </summary>
    /// <param name="respawnable"></param>
    public void RespawnAtBeginning(Transform respawnable)
    {
        respawnable.transform.position = levelStartPoint.getRespawnPosition();
        levelStartPoint.RespawnStart();
        SetRecentPoint(levelStartPoint);
    }

    /// <summary>
    /// Store the given checkpoint as the most recently passed checkpoint (i.e. the one that the player will now respawn at)
    /// </summary>
    public void SetRecentPoint(Checkpoint checkpoint)
    {
        // if the player is activating a different checkpoint
        if (mostRecentPoint != checkpoint)
        {
            // deactivate the old checkpoint as long as it's not the level start (i.e. the level start should always remain activated)
            if (mostRecentPoint != levelStartPoint)
            {
                mostRecentPoint.OnDeactivation();
            }

            // set the new checkpoint
            mostRecentPoint = checkpoint;

            if (levelStartPoint == null)
            {
                levelStartPoint = mostRecentPoint;
            }
        }
    }

    /// <summary>
    /// Provides access to the most recent checkpoint that this CheckpointManager registered. Note that this getter is necessary
    /// because, in the case of saving player progress but resetting level state, we need the information of most recent checkpoint
    /// to persist longer than the lifespan of this CheckpointManager object.
    /// </summary>
    public Checkpoint getMostRecentPoint() {
        return mostRecentPoint;
    }
}
