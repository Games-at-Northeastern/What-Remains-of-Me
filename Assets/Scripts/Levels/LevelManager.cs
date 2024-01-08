using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;



/// <summary>
/// This class provides the functionalities for the LevelManager singleton,
/// which is used to manage the scene and warp the player between scenes.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private string warpDestination;
    private int toWarpID;
    private bool loadedNewScene = false;
    private static Vector2 recentCheckpointHolder;
    private static bool checkpointHeld;
    public static GameObject PlayerRef { get; private set; }

    // Level events
    [Header("Event to trigger player respawn after obstacle collision.")]
    public static UnityEvent OnPlayerReset = new UnityEvent();
    [Header("Event to trigger player death events.")]
    public static UnityEvent OnPlayerDeath = new UnityEvent();
    public static CheckpointManager checkpointManager;

    // Event start functions that are accessible for other objects to trigger events
    /// <summary>
    /// Event to trigger player respawn after obstacle collision. This should respawn the player at the nearest checkpoint.
    /// </summary>
    public static void PlayerReset() => OnPlayerReset?.Invoke();
    /// <summary>
    /// Event to trigger full player death event. This should respawn the player at the beginning of the level.
    /// </summary>
    public static void PlayerDeath() => OnPlayerDeath?.Invoke(); // TODO: We may want to take in a type of death for this function

    private void Awake()
    {
        OnPlayerReset.RemoveAllListeners();
        OnPlayerDeath.RemoveAllListeners();

    }
    private void Start()
    {
        PlayerRef = FindObjectOfType<PlayerController.PlayerController2D>().gameObject;
        checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    /// <summary>
    /// If there is no player reference, sets the player reference to the player game object.
    /// If the player is supposed to load into a new scene, loads the player game object
    /// into the scene at the scene's set load in position.
    /// If the player presses the "R" key, resets the player at the tutorial scene.
    /// </summary>
    private void Update()
    {
        if (loadedNewScene)
        {
            GameObject[] warpZones = GameObject.FindGameObjectsWithTag("WarpZone");
            foreach (GameObject warpZone in warpZones)
            {
                LevelWarpZone wz = warpZone.GetComponent<LevelWarpZone>();
                if (wz.id == toWarpID)
                {
                    PlayerRef.transform.position = wz.loadInPosition.position;
                    loadedNewScene = false;
                    return;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket)) {
            CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
            recentCheckpointHolder = checkpointManager.getMostRecentPoint().getRespawnPosition();
            checkpointHeld = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /* Commented out cause prevents game from pausing
         * if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }*/
    }

    /// <summary>
    /// Sets the warp destination.
    /// <param name="warpDestination">the string name of the scene to warp to</param>
    /// <param name="id">an integer id for the scene to warp to</param>
    /// </summary>
    public void SetWarpID(string warpDestination, int id)
    {
        this.warpDestination = warpDestination;
        this.toWarpID = id;
    }

    /// <summary>
    /// Warps to the set warp destination.
    /// </summary>
    public void WarpToScene()
    {
        SceneManager.LoadScene(warpDestination);
        loadedNewScene = true;
    }

    /// <summary>
    /// Returns the checkpoint being held by this LevelManager and stops holding it. In theory, since the LevelManager is persistent, this method should only
    /// need to be called when we have reloaded a level mid-progress and wish to preserve the player's physical progress through the level.
    /// </summary>
    public static Vector2 extractRecentCheckpoint() {
        checkpointHeld = false;
        return recentCheckpointHolder;
    }

    public static bool holdingCheckpoint() {
        return checkpointHeld;
    }
}
