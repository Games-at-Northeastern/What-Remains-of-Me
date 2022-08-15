using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private string warpDestination;
    private int toWarpID;
    private bool loadedNewScene = false;
    private GameObject playerRef;

    // Singleton
    public static LevelManager Instance { get; private set; }

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

    private void Start()
    {
        if (playerRef == null) { playerRef = GameObject.FindGameObjectWithTag("Player"); }
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (playerRef == null) { playerRef = GameObject.FindGameObjectWithTag("Player"); }
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void SetWarpID(string warpDestination, int id)
    {
        this.warpDestination = warpDestination;
        this.toWarpID = id;
    }

    public void WarpToScene()
    {
        SceneManager.LoadScene(warpDestination);
        playerRef = null; // Comment out if player is DontDestroyOnLoad
        loadedNewScene = true;
    }
}
