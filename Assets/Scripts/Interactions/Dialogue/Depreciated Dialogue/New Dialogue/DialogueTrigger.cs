using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    [SerializeField] private DialogueSO _dialogue;
    [SerializeField] private DialogueManager _dialogueManager;

    private ControlSchemes _cs;

    private bool _playerInRange;

    private void Start()
    {
        _cs = new ControlSchemes();
        _cs.Enable();
        _cs.Player.Dialogue.performed += _ => this.TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if (this._playerInRange)
        {
            this._dialogueManager.StartDialogue(this._dialogue);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this._playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this._playerInRange = false;
        }
    }
}
