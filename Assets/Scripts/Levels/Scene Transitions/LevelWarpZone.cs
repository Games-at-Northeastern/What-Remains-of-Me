using System.Collections;
using UnityEngine;

/// <summary> 
/// This class provides the functionalities for a LevelWarpZone, 
/// which warps the player to the next scene.
/// </summary>
public class LevelWarpZone : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Animator anim;

    [SerializeField] private int id;
    [SerializeField] private string warpDestinationScene;
    [SerializeField] private int warpDestinationID;
    [SerializeField] private Transform loadInPosition;

    private readonly float transitionTime;

    // accessors
    public int Id { get => id; set => id = value; }
    public string WarpDestinationScene { get => warpDestinationScene; set => warpDestinationScene = value; }
    public int WarpDestinationID { get => warpDestinationID; set => warpDestinationID = value; }
    public Transform LoadInPosition { get => loadInPosition; set => loadInPosition = value; }

    /// <summary>
    /// If the player game object collides with the LevelWarpZone, 
    /// warps the player to the next level.
    /// <param name="collision">the other collider involved in this collision</param>
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(LoadLevel());
        }
    }

    /// <summary>
    /// Warps the player to the set warp destination scene.
    /// </summary>
    private IEnumerator LoadLevel()
    {
        //anim.SetTrigger("Start");
        levelManager.SetWarpID(WarpDestinationScene, WarpDestinationID);
        yield return new WaitForSeconds(transitionTime);
        levelManager.WarpToScene();
    }
}
