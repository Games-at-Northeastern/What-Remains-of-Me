using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Function to load a scene by name (input from Unity Inspector or script)
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
