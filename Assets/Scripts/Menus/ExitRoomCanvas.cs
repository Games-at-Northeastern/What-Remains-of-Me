using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitRoomCanvas : MonoBehaviour
{
    public Canvas nextChaptCanvas;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "ExitRoom")
        {
            Debug.Log("Hit exit room door");
            nextChaptCanvas.enabled = true;
        }
    }

    public void MainMenu()
    {
        nextChaptCanvas.enabled = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
