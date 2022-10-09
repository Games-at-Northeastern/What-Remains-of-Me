using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary> 
/// This class provides the functionalities needed for the pause menu.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseCanvas;

    /// <summary>
    /// Pauses the game if the "P" key is pressed.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!GameIsPaused)
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// Pauses the game by opening the pause menu and stopping the game time. 
    /// </summary>
    void Pause()
    {
        pauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    /// <summary> 
    /// Resumes the game by closing the pause menu and starting the game time. 
    /// </summary>
    public void Resume()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    /// <summary>
    /// Closes the pause menu and goes to the main menu screen.
    /// </summary>
    public void MainMenu()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
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
