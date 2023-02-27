using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InkDialogueTrigger : MonoBehaviour
{
    [Header("VisualCue")]
    [SerializeField] private GameObject visualCue;

    [Header("inkJSON")]
    [SerializeField] private TextAsset inkJSON;

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
            if (_cs.Player.Dialogue.WasReleasedThisFrame())
            {
                InkDialogueManager.GetInstance().EnterDialogueMode(inkJSON);
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
