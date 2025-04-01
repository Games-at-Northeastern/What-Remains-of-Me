using PlayerController;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class provides the functionalities needed for the pause menu.
/// </summary>

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject confirmQuitMenu;

    [SerializeField] private TMP_Text mouseToggleText;
    private bool isMouseEnabled = true;

    private PlayerController2D player;

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
    
    private void Start()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmQuitMenu.SetActive(false);
        player = FindAnyObjectByType<PlayerController2D>();
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
                if (settingsPanel.activeInHierarchy)
                {
                    ExitSettingsMenu();
                }
                else
                {
                    Resume();
                }
            }
        }
    }

    /// <summary>
    /// Pauses the game by opening the pause menu and stopping the game time.
    /// </summary>
    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        if (player != null)
            player.LockInputs();
    }

    /// <summary>
    /// Resumes the game by closing the pause menu and starting the game time.
    /// </summary>
    public void Resume()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmQuitMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        if (player != null)
            player.UnlockInputs();

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
    /// Opens up the settings menu where a variety of settings could be adjusted.
    /// </summary>
    public void SettingsMenu()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Closes the settings panel, reopens the pause panel.
    /// </summary>
    public void ExitSettingsMenu()
    {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Quits the game and logs the action.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    /// <summary>
    /// Switch the states of toggle mouse(on/off).
    /// </summary>
    public void ToggleMouse()
    {
        isMouseEnabled = !isMouseEnabled;
        Cursor.visible = isMouseEnabled;
        Cursor.lockState = isMouseEnabled ? CursorLockMode.None : CursorLockMode.Locked;
        mouseToggleText.text = "Toggle Mouse Enable: " + (isMouseEnabled ? "On" : "Off");
    }
}
