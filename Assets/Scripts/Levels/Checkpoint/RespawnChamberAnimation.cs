using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class RespawnChamberAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Checkpoint checkpoint;

    private void Start()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (checkpoint == null)
        {
            checkpoint = GetComponentInParent<Checkpoint>();
            if (checkpoint == null)
            {
                Debug.LogWarning("No checkpoint script found in parent - please manually add reference.");
            }
        }

        // This sets up the event listening to begin the respawn animation once the checkpoint parent object
        // has been triggered to start the respawn.
        checkpoint.OnRespawn.AddListener(RespawnStarted);
    }

    /// <summary>
    /// Begins the door animation for the respawn chamber, pausing the player controls while it plays.
    /// </summary>
    private void RespawnStarted()
    {
        LevelManager.Instance.PlayerPause();

        // Set the layer to show above the player
        spriteRenderer.sortingLayerName = "Foreground";

        // Begin the door open animation
        anim.enabled = true;
        anim.SetBool("isOpen", true);
    }

    private void RespawnCompleted() {
        // Set the layer to show below the player
        spriteRenderer.sortingLayerName = "Interactables";

        LevelManager.Instance.PlayerUnpause();
        anim.SetBool("isOpen", false);
    }

}
