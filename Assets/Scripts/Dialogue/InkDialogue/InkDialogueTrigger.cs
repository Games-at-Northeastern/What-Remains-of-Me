using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InkDialogueTrigger : MonoBehaviour
{
    [Header("VisualCue")]
    [SerializeField] private GameObject visualCue;

    [Header("inkJSON")]
    public TextAsset inkJSON;
    [Header("Force Dialogue")]
    [SerializeField] private bool forceDialogue;
    [SerializeField] private bool stopMovement = true;
    [SerializeField] private bool autoTurnPage = false;
    [SerializeField] private float waitForPageTurn = 2f;
    [SerializeField] private bool dialogueActive = true;
    [SerializeField] private float waitForInteractAvailable = 3f;

    [SerializeField] private bool isTutorialDialogue = false;

    private bool playerInRange;

    private ControlSchemes _cs;

    private bool _playerInRange;
    private bool _firstInteraction;

    private void Start()
    {
        _cs = new ControlSchemes();
        _cs.Enable();
    }

    private void Awake()
    {
        playerInRange = false;
        _firstInteraction = true;
    }

    private void OnEnable() => visualCue.SetActive(_firstInteraction);

    private void OnDisable() => visualCue.SetActive(false);

    private void Update()
    {
        if (playerInRange && !InkDialogueManager.GetInstance().dialogueIsPlaying && dialogueActive)
        {

            // disables dialogue so that the player can't enter the dialogue mode until waitForInteractAvailable seconds
            if (InkDialogueManager.GetInstance().dialogueEnded)
            {
                StartCoroutine(CanInteractAgain());
            }
            else if (_cs.Player.Dialogue.WasReleasedThisFrame() || forceDialogue)
            {
                _firstInteraction = false;
                forceDialogue = false;
                var i = InkDialogueManager.GetInstance();
                i.stopMovement = this.stopMovement;
                i.autoTurnPage = this.autoTurnPage;
                i.waitBeforePageTurn = this.waitForPageTurn;
                i.EnterDialogueMode(inkJSON);
            }
        }

        // disables visual cue so handler doesn't have animated text bubble upon next interaction
        if (!_firstInteraction)
        {
            visualCue.SetActive(false);
        }
    }

    private IEnumerator CanInteractAgain()
    {
        setDialogueActive(false);
        yield return new WaitForSeconds(waitForInteractAvailable);
        setDialogueActive(true);
        InkDialogueManager.GetInstance().dialogueEnded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && dialogueActive)
        {
            playerInRange = true;
        }
        InkDialogueManager.GetInstance().isTutorialDialogue = isTutorialDialogue;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void setDialogueActive(bool status) {
        dialogueActive = status;
    }
}
