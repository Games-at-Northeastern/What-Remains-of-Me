using UnityEngine;
using UnityEngine.SceneManagement;
public class GDCLevelManager : MonoBehaviour
{
    public void LoadGameScene(int sceneID)
    {
        Debug.Log("Loading scene " + sceneID);
        SceneManager.LoadScene(sceneID);
    }
}
