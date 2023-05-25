using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a saveable location that the player can pass and be respawned at.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    private CheckpointManager checkpointManager;

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
    }
}