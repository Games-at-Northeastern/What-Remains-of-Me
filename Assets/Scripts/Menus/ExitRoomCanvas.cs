using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> 
/// This class provides the functionalities needed for the exit room menu.
/// </summary>
public class ExitRoomCanvas : MonoBehaviour
{
    public Canvas nextChaptCanvas;

    /// <summary>
    /// Checks if the player has hit the game object that exits the room. 
    /// If the player has collided into said game object, logs the action 
    /// and opens the menu for proceeding to the next chapter. 
    /// <param name="collision">the other collider involved in this collision</param>
    /// </summary>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "ExitRoom")
        {
            Debug.Log("Hit exit room door");
            nextChaptCanvas.enabled = true;
        }
    }

    /// <summary>
    /// Closes the next chapter menu and goes to the main menu screen.
    /// </summary>
    public void MainMenu()
    {
        nextChaptCanvas.enabled = false;
        SceneManager.LoadScene("MainMenu");
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
