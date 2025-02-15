using PlayerController;
using Unity.VisualScripting;
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


    [SerializeField]
    private bool autoTurnPage;
    [SerializeField]
    private TextAsset[] cutsceneDialogue;
    private int dialogueIndex = 0;


    private bool isWaitingForDialogue = false;

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

    private void Update()
    {
        if (isWaitingForDialogue)
        {
            if (!InkDialogueManager.GetInstance().dialogueEnded)
            {
                cutscene.Pause();
            }
            else
            {
                cutscene.Resume();
                isWaitingForDialogue = false;
            }
        }
    }


    //Give the player control again and delete the trigger once the cutscene is done
    private void EndCutscene(PlayableDirector director)
    {
        onCutsceneEnd.Invoke();
        characterController.UnlockInputs();
        Destroy(gameObject);
    }


    //Shows the next piece of dialogue from the provided list
    public void ShowNextDialogue()
    {
        if (dialogueIndex >= cutsceneDialogue.Length)
        {
            Debug.LogWarning("Out of dialgoue! Too many dialogue signals in timeline");
            return;
        }

        var dialogueManager = InkDialogueManager.GetInstance();

        //Seems counter intuitive, but we want the cutscene to have control over the player's movement, not the dialogue system
        dialogueManager.stopMovement = false;
        dialogueManager.autoTurnPage = autoTurnPage;
        dialogueManager.waitBeforePageTurn = 0;

        InkDialogueManager.GetInstance().EnterDialogueMode(cutsceneDialogue[dialogueIndex]);
        dialogueIndex++;
    }


    public void WaitForDialogue() => isWaitingForDialogue = true;


    //If the player enters the trigger we take away control and start the cutscene
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCutscene();
        }
    }

    private void StartCutscene()
    {
        onCutsceneStart.Invoke();
        characterController.LockInputs();
        cutscene.Play();
    }
}
