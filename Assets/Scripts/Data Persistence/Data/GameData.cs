using System.Collections;
using System.Collections.Generic;
using SmartScriptableObjects.ReactiveProperties;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
/// <summary>
///Formats the game data into a readable format for the save and load system.
/// </summary>
public class GameData
{

    public float batteryPercentage;     //The percent of clean energy the player was at during the save.
    public float virusPercentage;       //The percent of virus energy the player was at during the save.
    public Vector3 playerPosition;      //The saved position of the player.

    public SerializableDictionary<string, float> outletCleanEnergy; //Saves all the outlets GUID's and clean energy values as a percent 0-1.0f
    public SerializableDictionary<string, float> outletVirusEnergy; //Saves all the outlets GUID's and virus energy values as a percent 0-1.0f
    public SerializableDictionary<string, float> outletMaxEnergy;   //Saves all the outlets GUID's and max energy values as a percent 0-1.0f

    public int scenePlayerSavedIn;   //Saves the scene the player is currently in as an int.

    /**
     * When a new game is created, all values in the constructor should be read into the game
     * This will give the player the below starting values.
     */
    public GameData()
    {
        scenePlayerSavedIn = 0;
        batteryPercentage = 1f;
        virusPercentage = 0f;
        playerPosition = Vector3.zero;
        float[] temp = new float[3];
        outletCleanEnergy = new SerializableDictionary<string, float>();
        outletVirusEnergy = new SerializableDictionary<string, float>();
        outletMaxEnergy = new SerializableDictionary<string, float>();

    }
}
