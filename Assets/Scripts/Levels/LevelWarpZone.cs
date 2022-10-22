using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// This class provides the functionalities for a LevelWarpZone, 
/// which warps the player to the next scene.
/// </summary>
public class LevelWarpZone : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    public int id;
    public string warpDestinationScene;
    public int warpDestinationID;
    public Transform loadInPosition;

    private LevelManager lm;
    private float transitionTime;
    private Animator anim;
    
    /// <summary> 
    /// Finds the LevelManager and the Animator component in this scene. 
    /// </summary>
    private void Start()
    {
        lm = levelManager;

    }

    /// <summary>
    /// If the player game object collides with the LevelWarpZone, 
    /// warps the player to the next level.
    /// <param name="collision">the other collider involved in this collision</param>
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(LoadLevel());
        }
    }

    /// <summary>
    /// Warps the player to the set warp destination scene.
    /// </summary>
    IEnumerator LoadLevel()
    {
        anim.SetTrigger("Start");
        lm.SetWarpID(warpDestinationScene, warpDestinationID);
        yield return new WaitForSeconds(transitionTime);
        lm.WarpToScene();
    }
}
