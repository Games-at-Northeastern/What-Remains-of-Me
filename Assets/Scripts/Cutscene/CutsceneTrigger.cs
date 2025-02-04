using PlayerController;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector cutscene;

    [SerializeField]
    private UnityEvent onCutsceneStart;

    [SerializeField]
    private UnityEvent onCutsceneEnd;

    private PlayerController2D characterController;


    private void Start()
    {
        if (cutscene == null)
        {
            Debug.LogWarning("No cutscene found! Assign the director in the inspector.");
        }

        cutscene.playOnAwake = false;

        characterController = FindObjectOfType<PlayerController2D>();
        cutscene.stopped += EndCutscene;
    }


    //Give the player control again and delete the trigger once the cutscene is done
    private void EndCutscene(PlayableDirector director)
    {
        onCutsceneEnd.Invoke();
        characterController.UnlockInputs();
        Destroy(gameObject);
    }


    //If the player enters the trigger we take away control and start the cutscene
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            onCutsceneStart.Invoke();
            characterController.LockInputs();
            cutscene.Play();
        }
    }
}
