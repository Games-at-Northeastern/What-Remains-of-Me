using System.Collections;
using System.Collections.Generic;
using SmartScriptableObjects.ReactiveProperties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
/// <summary>
///Formats the game data into a readable format for the save and load system.
/// </summary>
public class PlayerData
{
    public string scenePlayerSavedIn = "";   //Saves the scene the player is currently in as an int.

    public float batteryPercentage = 1f;     //The percent of clean energy the player was at during the save.
    public float virusPercentage = 0f;       //The percent of virus energy the player was at during the save.
    public Vector3 playerPosition = Vector3.zero;      //The saved position of the player.

    /**
     * When a new game is created, all values in the constructor should be read into the game
     * This will give the player the below starting values.
     */
    public PlayerData()
    {
        scenePlayerSavedIn = "";
        batteryPercentage = 1f;
        virusPercentage = 0f;
        playerPosition = Vector3.zero;

    }
}
