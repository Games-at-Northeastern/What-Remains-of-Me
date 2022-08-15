using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The "main" movement script. Moves the player based on the information provided
/// in the appropriate IMove script, switching when appropriate.
/// </summary>
[RequireComponent(typeof(MovementSettings))]
[RequireComponent(typeof(MovementInfo))]
[RequireComponent(typeof(WireThrower))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovementExecuter : MonoBehaviour
{
    Rigidbody2D rb; // To move the player
    MovementInfo mi; // Gives other info to move scripts
    MovementSettings ms; // Gives constants to move scripts
    WireThrower wt; // Gives information about the wire
    PlayerHealth ph; // Gives info about player health / damage
    IMove currentMove; // The move taking place this frame
    Vector3 respawnPosition;

    // Initialization
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mi = GetComponent<MovementInfo>();
        ms = GetComponent<MovementSettings>();
        wt = GetComponent<WireThrower>();
        ph = GetComponent<PlayerHealth>();
        ControlSchemes cs = new ControlSchemes();
        cs.Enable();
        cs.Debug.Restart.performed += _ => Restart();
        currentMove = new StarterMove(mi, ms, cs, wt, ph);
        respawnPosition = transform.position;
    }

    /// <summary>
    /// For every frame, a single frame of time should be passed in the current move,
    /// the player should be moved as requested in the move script, and the current
    /// move should be changed to something new if appropriate.
    /// </summary>
    private void Update()
    {
        currentMove.AdvanceTime();
        rb.velocity = new Vector2(currentMove.XSpeed(), currentMove.YSpeed());
        currentMove = currentMove.GetNextMove();
    }

    /// <summary>
    /// Resets the player to their original position. For debugging only.
    /// </summary>
    void Restart()
    {
        transform.position = respawnPosition;
        currentMove = new Fall();
    }

    /// <summary>
    /// Gives an immutable version of the current move. This allows certain information
    /// about the move to be accessed.
    /// </summary>
    public IMoveImmutable GetCurrentMove()
    {
        return currentMove;
    }
}
