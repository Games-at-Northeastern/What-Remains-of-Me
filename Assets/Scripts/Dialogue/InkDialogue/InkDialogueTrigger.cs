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
    [SerializeField] private bool goNextPiece = false;
    [SerializeField] private int waitForPageTurn = 2;

    private bool playerInRange;

    private ControlSchemes _cs;

    private bool _playerInRange;

    private void Start()
    {
        _cs = new ControlSchemes();
        _cs.Enable();
    }

    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !InkDialogueManager.GetInstance().dialogueIsPlaying)
        {
            visualCue.SetActive(true);
            if (_cs.Player.Dialogue.WasReleasedThisFrame() || forceDialogue)
            {
                forceDialogue = false;
                var i = InkDialogueManager.GetInstance();
                i.EnterDialogueMode(inkJSON);
                i.stopMovement = this.stopMovement;
                i.goNextPiece = this.goNextPiece;
                i.waitBeforePageTurn = this.waitForPageTurn;
            }


        }
        else
        {
            visualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
}
