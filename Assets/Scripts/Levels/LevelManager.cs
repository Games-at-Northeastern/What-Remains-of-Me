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
    private GameObject playerRef;

    // Level events
    [Header("Event to trigger player respawn after obstacle collision. Requires a respawn position.")]
    public UnityEvent OnPlayerReset;

    // Event start functions that are accessible for other objects to trigger events
    public void PlayerReset() => OnPlayerReset?.Invoke();

    // Singleton
    public static LevelManager Instance { get; private set; }

    /// <summary>
    /// Since the LevelManager is a singleton, makes sure to keep the same instance
    /// of this game object
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// If there is no player reference, sets the player reference to the player game object.
    /// Do not destroy this singleton object when a new scene is loaded.
    /// </summary>
    private void Start()
    {
        playerRef = player;

        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// If there is no player reference, sets the player reference to the player game object.
    /// If the player is supposed to load into a new scene, loads the player game object
    /// into the scene at the scene's set load in position.
    /// If the player presses the "R" key, resets the player at the tutorial scene.
    /// </summary>
    private void Update()
    {
        if (playerRef == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerRef = GameObject.FindGameObjectWithTag("Player");
        }
        if (loadedNewScene)
        {
            GameObject[] warpZones = GameObject.FindGameObjectsWithTag("WarpZone");
            foreach (GameObject warpZone in warpZones)
            {
                LevelWarpZone wz = warpZone.GetComponent<LevelWarpZone>();
                if (wz.id == toWarpID)
                {
                    playerRef.transform.position = wz.loadInPosition.position;
                    loadedNewScene = false;
                    return;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
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
        playerRef = null; // Comment out if player is DontDestroyOnLoad
        loadedNewScene = true;
    }
}
