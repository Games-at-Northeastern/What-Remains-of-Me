using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The "main" movement script. Moves the player based on the information provided
/// in the appropriate IMove script, switching when appropriate.
/// </summary>
public class MovementExecuter : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb; // To move the player
    [SerializeField] private MovementInfo mi; // Gives other info to move scripts
    [SerializeField] private MovementSettings ms; // Gives constants to move scripts
    [SerializeField] private WireThrower wt; // Gives information about the wire
    [SerializeField] private PlayerHealth ph; // Gives info about player health / damage

    private LevelManager levelManager;
    private IMove currentMove; // The move taking place this frame
    private Vector3 respawnPosition;
    private ControlSchemes cs;

    private bool isPaused;

    public bool isOnAPlatform;
    public Rigidbody2D platformRb;

    private CheckpointManager checkpointManager;

    // Initialization
    private void Awake()
    {
        cs = new ControlSchemes();
        cs.Enable();
        cs.Debug.Restart.performed += _ => Restart();

        currentMove = new StarterMove(mi, ms, cs, wt, ph);
        respawnPosition = transform.position;

        isPaused = false;
    }

    private void Start()
    {
        // Set up the level manager, and the control scheme enable/disable for when the player movement should be paused
        levelManager = LevelManager.Instance;

        RegisterEvents();
        checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    /// <summary>
    /// Register this executor as a listener to any necessary events.
    /// </summary>
    private void RegisterEvents()
    {
        levelManager.OnPlayerReset.AddListener(Respawn);
        levelManager.OnPlayerDeath.AddListener(Restart);
        levelManager.OnPlayerPause.AddListener(Pause);
        levelManager.OnPlayerUnpause.AddListener(Unpause);

        levelManager.OnPlayerPause.AddListener(cs.Disable);
        levelManager.OnPlayerUnpause.AddListener(cs.Enable);
    }

    /// <summary>
    /// For every frame, a single frame of time should be passed in the current move,
    /// the player should be moved as requested in the move script, and the current
    /// move should be changed to something new if appropriate.
    /// </summary>
    private void Update()
    {
        if (isPaused)
        {
            rb.velocity = new Vector2(0, 0);
            currentMove = new Idle();
            return;
        }
        if (isOnAPlatform)
        {
            currentMove.AdvanceTime();
            rb.velocity = new Vector2(currentMove.XSpeed() + platformRb.velocity.x, currentMove.YSpeed() + platformRb.velocity.y);
            currentMove = currentMove.GetNextMove();
        }
        else
        {
            DoMove();
        }
    }


    // executes the movement this frame
    private void DoMove()
    {
        currentMove.AdvanceTime();
        rb.velocity = new Vector2(currentMove.XSpeed(), currentMove.YSpeed());
        currentMove = currentMove.GetNextMove();
    }

    /// <summary>
    /// Resets the player to their original position. For debugging only.
    /// </summary>
    private void Restart()
    {
        if (checkpointManager != null)
        {
            checkpointManager.RespawnAtBeginning(rb.transform);
        } else
        {
            rb.position = respawnPosition;
        }
        currentMove = new Fall();
    }
    
    /// <summary>
    /// Respawns the player at a given location.
    /// </summary>
    private void Respawn()
    {
        if (checkpointManager != null)
        {
            checkpointManager.RespawnAtRecent(rb.transform);
        }

        rb.velocity = Vector2.zero;
        currentMove = new Idle();
    }

    private void Pause() => isPaused = true;
    private void Unpause() => isPaused = false;

    /// <summary>
    /// Gives an immutable version of the current move. This allows certain information
    /// about the move to be accessed.
    /// </summary>
    public IMoveImmutable GetCurrentMove() => currentMove;
}
