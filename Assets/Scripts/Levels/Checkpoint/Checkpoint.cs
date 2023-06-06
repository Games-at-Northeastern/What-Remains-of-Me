using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents a saveable location that the player can pass and be respawned at.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    private CheckpointManager checkpointManager;

    [SerializeField] private Transform respawnPoint;

    // EVENTS
    public UnityEvent OnRespawn;
    public void RespawnStart() => OnRespawn?.Invoke();

    public UnityEvent OnActivate;
    public void Activate() => OnActivate?.Invoke();



    private void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("checkpoint hit");

        // Check to see if the collision is the player or not
        if (!collision.CompareTag("Player"))
        {
            return;
        }


        // Set this checkpoint as the most recent checkpoint if the player has collided with it
        if (checkpointManager != null)
        {
            checkpointManager.SetRecentPoint(this);
        }

        OnActivation();
    }

    private void OnActivation()
    {
        // TODO : fade activation light on here (or in a separate script)
    }

    /// <summary>
    /// Get the position that the player should respawn at for this checkpoint.
    /// </summary>
    /// <returns></returns>
    public Vector2 getRespawnPosition()
    {
        if (respawnPoint != null)
        {
            return respawnPoint.position;
        }

        return transform.position;
    }
}
