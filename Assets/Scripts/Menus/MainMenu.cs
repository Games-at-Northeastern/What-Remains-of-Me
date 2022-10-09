using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> 
/// This class provides the functionalities needed for the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
   
   /// <summary>
   /// Loads Chapter 1 Room 1 (the tutorial scene).
   /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("Chapt1Room1StartingTutorial");
    }

    /// <summary>
    /// Quits the game and logs the action.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();  
    }

}
