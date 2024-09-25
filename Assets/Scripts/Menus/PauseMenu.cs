using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary> 
/// This class provides the functionalities needed for the pause menu.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pausePanel;
    public GameObject confirmQuitMenu;

    /// <summary>
    /// Pauses the game if the "P" key is pressed.
    /// </summary>
    private void OnValidate()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("pause menu won't work without an Event System and.or UI input module");
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameIsPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }
    private void Start()
    {
        pausePanel.SetActive(false);
    }

    /// <summary>
    /// Pauses the game by opening the pause menu and stopping the game time. 
    /// </summary>
    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    /// <summary> 
    /// Resumes the game by closing the pause menu and starting the game time. 
    /// </summary>
    public void Resume()
    {
        pausePanel.SetActive(false);
        confirmQuitMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    /// <summary>
    /// Closes the pause menu and goes to the main menu screen.
    /// </summary>
    public void MainMenu()
    {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Opens the quit confirmation menu
    /// </summary>
    public void ConfirmQuit()
    {
        confirmQuitMenu.SetActive(true);
    }
    /// <summary>
    /// restarts the level
    /// </summary>
    public void RestartLevel()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// Opens up the settings menu where a variety of setting could be adjusted.
    /// </summary>
    public void SettingsMenu()
    {
        Resume();
        SceneManager.LoadScene("SettingsMenu");
        
        /// previous code
        /// throw new System.NotImplementedException("Settings menu not implemented yet");
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
