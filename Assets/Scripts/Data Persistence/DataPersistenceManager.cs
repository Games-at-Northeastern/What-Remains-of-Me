using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///A singleton class(only place one per scene) that keeps track of save and load data.
/// </summary>

//If a refrence or deeper explination is needed this was based on https://www.youtube.com/watch?v=aUi9aijvpgs&ab_channel=ShapedbyRainStudios.
public class DataPersistenceManager : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Where the player save data will be saved to")]
    private string fileName;

    [SerializeField]
    [Tooltip("Wether the level should use save data on start to setup the level")]
    private bool loadSaveDataOnStart = true;

    private PlayerData playerData;  //Keeps track of the player data from save/load. Things like player virus and player energy are stored in this format.
    private LevelData levelData;    //Keeps track of the level data from save/load. Things like outlet power and outlet in this format.

    private List<IDataPersistence> dataPersistenceObjects;      //A list of objects that have the IDataPersistence. Anything with IDataPersistence has data that needs to be saved or loaded.

    private FileDataHandler<PlayerData> playerFileDataHandler;  //A file handler that will load and save player data to and from a Json format.
    private FileDataHandler<LevelData> levelFileDataHandler;    //A file handler that will load and save level data to and from a Json format.

    public static bool playerClickedLoad = false;

    public static DataPersistenceManager instance { get; private set; } //A static variable to ensure theres only one DataPersistenceManager in the scene.

    public static int numberOfDataManagers;

    private void Awake()
    {
        numberOfDataManagers = GameObject.FindObjectsOfType(typeof(DataPersistenceManager)).Length;
        this.dataPersistenceObjects = FindAllDataPersistenceObjects(); //When the player opens the game, find all the DataPersistenceObjects so we can begin a save/load
        if (numberOfDataManagers > 2)
        {
            Debug.LogError("Save/Load: Found More than two DataPersistenceManagers in the current scene. Other than within the prefab. This may cause errors with saving and loading data."); 
        }

        instance = this;
        playerData = new PlayerData();
        levelData = new LevelData();
        playerFileDataHandler = new FileDataHandler<PlayerData>(Application.persistentDataPath, fileName);
        levelFileDataHandler = new FileDataHandler<LevelData>(Application.persistentDataPath, "Level_" + SceneManager.GetActiveScene().name);
    }
    private void Start()
    {
        if (loadSaveDataOnStart)
        {
            LoadSceneData();
        }

    }

    /// <summary>
    ///Creates a new game by initializing playerData with its default constructor values.
    /// </summary>
    public void CheckForDataObjects()
    {
        if (playerData == null)
        {
            Debug.Log("Save/Load: No player data found on game load, creating new player data.");
            playerData = new PlayerData();
        }
        if (levelData == null)
        {
            Debug.Log("Save/Load: No level data found on game load, creating new level data.");
            levelData = new LevelData();
        }
        else if (levelData.sceneName != SceneManager.GetActiveScene().name)
        {
            levelData = new LevelData();
            levelData.sceneName = SceneManager.GetActiveScene().name;
        }

    }

    private void LoadSceneData()
    {
        //CheckForDataObjects();
        //Load scene data from file using the data handler 
        this.playerData = playerFileDataHandler.Load();
        this.levelData = levelFileDataHandler.Load();
        bool loadPlayerData = (playerData.scenePlayerSavedIn == SceneManager.GetActiveScene().name && playerClickedLoad);

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadLevelData(levelData);
            if (loadPlayerData)
            {
                dataPersistenceObj.LoadPlayerData(playerData);
                print("loading player data");
            }
        }
        playerClickedLoad = false;

        StaticData.Load();
    }

    public void LoadGame()
    {
        Debug.Log("Loading Game");
        playerClickedLoad = true;
        //CheckForDataObjects();
        //Retrieve data persistence objects
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();


        //Load save data from file using the data handler 
        this.playerData = playerFileDataHandler.Load();
        this.levelData = levelFileDataHandler.Load();
        StaticData.Load();

        //If we arent in the scene the player saved in during a load, load the scene the player saved in
        if (playerData.scenePlayerSavedIn != SceneManager.GetActiveScene().name)
        {

            SceneManager.LoadScene(playerData.scenePlayerSavedIn);
        }

        //if no data can be loaded, init the new game

        if (this.playerData == null)
        {
            Debug.Log("Save/Load: No data found on game load, starting new game.");
            playerData = new PlayerData();
            CheckForDataObjects();
        }
        else
        {
            //Push data onto scripts that need it
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadPlayerData(playerData);
                dataPersistenceObj.LoadLevelData(levelData);
            }
        }
    }

    public void SaveGame()
    {
        CheckForDataObjects();
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();


        //Pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref playerData, ref levelData);
        }
        playerData.scenePlayerSavedIn = SceneManager.GetActiveScene().name;

        //save that file using the data handler
        playerFileDataHandler.Save(playerData);
        levelFileDataHandler.Save(levelData);
        StaticData.Save();
    }

    /// <summary>
    /// Finds all Monobehavior script objects that have the IDataPersistence Interface.
    /// Keep in mind this only finds Monobehavior inheriting scripts.
    /// </summary>
    /// <returns>List of all data Persistence Objects</returns>
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {

        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
