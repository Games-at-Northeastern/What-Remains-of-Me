using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionToNextScene : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // loads the next scene by getting the index of the current scene and adding 1
            // loads the scene that is the scene after the current scene in the build index.
            // this will cause an error when you get to the last scene in the build index,
            // unless the last scene in the build index never calls this script
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}


