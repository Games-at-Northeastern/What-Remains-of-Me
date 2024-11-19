using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;
using System.Linq;
using UnityEngine.Playables;
using UnityEditor;
using UnityEngine.UIElements;

/// <summary>
///A singleton class(only place one per scene) that keeps track of save and load data.
/// </summary>

//If a refrence or deeper explination is needed this was based on https://www.youtube.com/watch?v=aUi9aijvpgs&ab_channel=ShapedbyRainStudios.
public class DataPersistenceManager : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Where the save data will be saved to")]
    private string fileName;

    private GameData gameData; //Keeps track of the save/load game data. Things like virus, energy and other important things to load.
     
    private List<IDataPersistence> dataPersistenceObjects; //A list of objects that have the IDataPersistence. Anything with IDataPersistence has data that needs to be saved or loaded.

    private FileDataHandler fileDataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects(); //When the player opens the game, find all the DataPersistenceObjects so we can begin a save/load
        if (instance != null)
        {
            Debug.LogError("Save/Load: Found More than one DataPersistenceManager in the current scene. This may cause errors with saving and loading data.");
        }

        instance = this;
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    /// <summary>
    ///Creates a new game by initializing gameData with its default constructor values.
    /// </summary>
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        //Retrieve data persistence objects
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();


        //Load save data from file using the data handler 
        this.gameData = fileDataHandler.Load();

        //if no data can be loaded, init the new game
        
        if(this.gameData == null)
        {
            Debug.Log("Save/Load: No data found on game load, starting new game.");
            gameData = new GameData();
            NewGame();
        }

        //Push data onto scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        //if no data can be loaded, init the new game

        if (this.gameData == null)
        {
            Debug.Log("Save/Load: No data found on game save, starting new game.");
            NewGame();
        }

        //Pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }


        //save that file using the data handler
        fileDataHandler.Save(gameData);

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
