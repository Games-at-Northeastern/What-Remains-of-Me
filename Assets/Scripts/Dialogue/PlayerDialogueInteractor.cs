using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to be held by the player. When the player's dialogue detector
/// collides with some object with dialogue, makes the prompt show up.
/// Additionally, while a prompt is active, allows the player to initiate
/// a dialogue sequence.
/// </summary>
public class PlayerDialogueInteractor : MonoBehaviour
{
    Dialogue activeDialogue; // Either the dialogue being prompted or the dialogue being read
    [SerializeField] CollisionDetector dialogueCollisionDetector;
    [SerializeField] GameObject promptSprite; // There is only one prompt sprite in the game, and it is either inactive or over a specific dialogue holding gameObject.
    public DialogueEvent OnDialogueRequested = new DialogueEvent();
    ControlSchemes cs;

    private void Start()
    {
        cs = new ControlSchemes();
        cs.Enable();
        cs.Player.Dialogue.performed += _ => PlayDialogue();
    }

    /// <summary>
    /// Has a dialogue take place, if there is an active dialogue prompt.
    /// </summary>
    void PlayDialogue()
    {
        if (activeDialogue != null)
        {
            promptSprite.SetActive(false);
            OnDialogueRequested.Invoke(activeDialogue);
            print("Dialogue Requested");
        }
    }

    /// <summary>
    /// Update whether a dialogue prompt is active or not,
    /// and update the placement of the prompt sprite.
    /// </summary>
    private void Update()
    {
        GameObject dGameObject = dialogueCollisionDetector.CollidingWith(); // d should have dialogue component if able to be collided with by dialogueCollisionDetector
        if (dGameObject != null)
        {
            Dialogue dialogue = dGameObject.GetComponent<Dialogue>();
            if (dialogue != activeDialogue)
            {
                promptSprite.transform.position = dialogue.GetPromptTransform().position;
                promptSprite.SetActive(true);
                activeDialogue = dialogue;
            }
        }
        else
        {
            activeDialogue = null;
            promptSprite.SetActive(false);
        }
    }
}
