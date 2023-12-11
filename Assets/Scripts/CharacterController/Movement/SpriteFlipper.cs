using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;

/// <summary>
/// Manipulates the player visually based on the current move being executed.
/// NOTE: Currently, all this script does is flip the player, but it can easily
/// be expanded to also support animations, particles, etc.
/// </summary>
public class SpriteFlipper : MonoBehaviour
{
    [SerializeField] private CharacterController2D cc;
    //[SerializeField] SpriteRenderer[] spriteRenderers;
    private void Start()
    {
        cc = GetComponentInParent<CharacterController2D>();
    }
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
        switch (cc.LeftOrRight)
        {
            case Facing.left:
                playerTransform.rotation = new Quaternion(0f, 180f, 0f, 0f);
                break;
            case Facing.right:
                playerTransform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                break;
        }
    }
}
