using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public string sceneToLoad;
    public int zoneToLoadAt;
    public float transitionTime = 1f;

    private LevelManager lm;
    private Animator anim;

    /// <summary>
    /// Finds the Animator that should be a component 
    /// in a game object that is a child of this LevelLoader.
    /// </summary>
    private void Start()
    {
        anim = animator;
    }


    /// <summary>
    /// When the player collides with the LevelLoader,
    /// begins loading the scene to load. 
    /// <param name="collision">the other collider involved in this collision</param>
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(LoadLevel(sceneToLoad));
        }
    }

    /// <summary> 
    /// Loads the given level.
    /// <param name="name">the string name of the scene to load</param>
    /// </summary>
    IEnumerator LoadLevel(string name)
    {
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(name);
    }
}
