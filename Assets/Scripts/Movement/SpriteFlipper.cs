using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manipulates the player visually based on the current move being executed.
/// NOTE: Currently, all this script does is flip the player, but it can easily
/// be expanded to also support animations, particles, etc.
/// </summary>
public class SpriteFlipper : MonoBehaviour
{
    [SerializeField] private MovementExecuter me;
    //[SerializeField] SpriteRenderer[] spriteRenderers;

    [SerializeField] private Transform playerTransform;

    private void Update()
    {
        if (InkDialogueManager.GetInstance() != null)
        {
            if (InkDialogueManager.GetInstance().dialogueIsPlaying && InkDialogueManager.GetInstance().stopMovement)
            {
                return;
            }
            else
            {
                HandleFlipping();
            }
        }
        HandleFlipping();
    }

    /// <summary>
    /// Flips the player depending on whether, according to the move, the player
    /// is flipped (facing left) or not (facing right).
    /// </summary>
    void HandleFlipping()
    {
        bool flipped = me.GetCurrentMove().Flipped();
        if (flipped)
        {
            playerTransform.rotation = new Quaternion(0f, 180f, 0f, 0f);
        }
        else
        {
            playerTransform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        }
        /*        foreach(SpriteRenderer sr in spriteRenderers)
                {
                    sr.flipX = flipped;
                }*/
    }
}
