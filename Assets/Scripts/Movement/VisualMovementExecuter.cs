using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manipulates the player visually based on the current move being executed.
/// NOTE: Currently, all this script does is flip the player, but it can easily
/// be expanded to also support animations, particles, etc.
/// </summary>
[RequireComponent(typeof(MovementExecuter))]
public class VisualMovementExecuter : MonoBehaviour
{
    MovementExecuter me;
    [SerializeField] SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        me = GetComponent<MovementExecuter>();
    }

    private void Update()
    {
        HandleFlipping();
    }

    /// <summary>
    /// Flips the player depending on whether, according to the move, the player
    /// is flipped (facing left) or not (facing right).
    /// </summary>
    void HandleFlipping()
    {
        bool flipped = me.GetCurrentMove().Flipped();
        foreach(SpriteRenderer sr in spriteRenderers)
        {
            sr.flipX = flipped;
        }
    }
}
