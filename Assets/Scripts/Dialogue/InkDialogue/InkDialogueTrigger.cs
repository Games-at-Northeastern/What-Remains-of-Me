using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InkDialogueTrigger : MonoBehaviour
{
    [Header("VisualCue")]
    [SerializeField] private GameObject visualCue;

    [Header("inkJSON")]
    [SerializeField] public TextAsset inkJSON;
    [Header("Force Dialogue")]
    [SerializeField] private bool forceDialogue;
    [SerializeField] private bool stopMovement = true;
    [SerializeField] private bool autoTurnPage = false;
    [SerializeField] private float waitForPageTurn = 2f;
    [SerializeField] private bool dialogueActive = true; 

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
        visualCue.SetActive(true);
    }

    private void Update()
    {
        if (playerInRange && !InkDialogueManager.GetInstance().dialogueIsPlaying)
        {
            
            if (_cs.Player.Dialogue.WasReleasedThisFrame() || forceDialogue)
            {
                _firstInteraction = false;
                forceDialogue = false;
                var i = InkDialogueManager.GetInstance();
                i.EnterDialogueMode(inkJSON);
                i.stopMovement = this.stopMovement;
                i.autoTurnPage = this.autoTurnPage;
                i.waitBeforePageTurn = this.waitForPageTurn;
            }
        }

        if (!_firstInteraction)
        {
            visualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && dialogueActive)
        {
            playerInRange = true;
        }
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
