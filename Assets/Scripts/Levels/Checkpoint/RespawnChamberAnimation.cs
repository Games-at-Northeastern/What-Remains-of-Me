using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class RespawnChamberAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

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

        LevelManager.Instance.OnPlayerReset.AddListener(RespawnStarted);
    }

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
