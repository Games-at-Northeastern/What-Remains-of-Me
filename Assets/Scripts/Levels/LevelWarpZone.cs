using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWarpZone : MonoBehaviour
{
    public int id;
    public string warpDestinationScene;
    public int warpDestinationID;
    public Transform loadInPosition;

    private LevelManager lm;
    private float transitionTime;
    private Animator anim;

    private void Start()
    {
        if (lm == null) { lm = FindObjectOfType<LevelManager>(); }
        anim = GameObject.FindGameObjectWithTag("LevelTransition").GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(LoadLevel());
        }
    }

    IEnumerator LoadLevel()
    {
        anim.SetTrigger("Start");
        lm.SetWarpID(warpDestinationScene, warpDestinationID);
        yield return new WaitForSeconds(transitionTime);
        lm.WarpToScene();
    }
}
