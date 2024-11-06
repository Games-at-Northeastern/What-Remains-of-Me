using System.Collections;
using System.Collections.Generic;
using SmartScriptableObjects.ReactiveProperties;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
/// <summary>
///Formats the game data into a readable format for the save and load system.
/// </summary>
public class GameData
{
    
    public float batteryPercentage;     //The percent of clean energy the player was at during the save.
    public float virusPercentage;       //The percent of virus energy the player was at during the save.
    public Vector3 playerPosition;      //The saved position of the player.


    /**
     * When a new game is created, all values in the constructor should be read into the game
     * This will give the player the below starting values.
     */
    public GameData()
    {
        this.batteryPercentage = 1f;
        this.virusPercentage = 0f;
        playerPosition = Vector3.zero;
    }
}
