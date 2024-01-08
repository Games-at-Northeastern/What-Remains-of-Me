using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;

/// <summary>
/// Script to be held by the player. When the player's dialogue detector
/// collides with some object with dialogue, makes the prompt show up.
/// Additionally, while a prompt is active, allows the player to initiate
/// a dialogue sequence.
/// </summary>
public class PlayerDialogueInteractor : MonoBehaviour
{
    private Dialogue _activeDialogue; // Either the dialogue being prompted or the dialogue being read
    [SerializeField] private PlayerController2D player;

    // There is only one prompt sprite in the game, and it is either
    // inactive or over a specific dialogue holding gameObject.
    [SerializeField] private GameObject promptSprite;

    public DialogueEvent OnDialogueRequested = new();
    private ControlSchemes _cs;

    private void Start()
    {
        _cs = new ControlSchemes();
        _cs.Enable();
        _cs.Player.Dialogue.performed += _ => PlayDialogue();
        player = GetComponentInParent<PlayerController2D>();
    }

    /// <summary>
    /// Has a dialogue take place, if there is an active dialogue prompt.
    /// </summary>
    private void PlayDialogue()
    {
        if (_activeDialogue != null)
        {
            promptSprite.SetActive(false);
            OnDialogueRequested.Invoke(_activeDialogue);
            print("Dialogue Requested");
        }
    }

    /// <summary>
    /// Update whether a dialogue prompt is active or not,
    /// and update the placement of the prompt sprite.
    /// </summary>'
    public Vector2 dialogueDetectorBounds;
    public Vector2 dialogueDetectorOffset;
    private void Update()
    {
        // d should have dialogue component if able to be collided with by dialogueCollisionDetector
        int facing = player.LeftOrRight == CharacterController.Facing.left ? -1 : 1;
        Collider2D[] overlaps = Physics2D.OverlapBoxAll((Vector2)this.transform.position + facing * dialogueDetectorOffset, dialogueDetectorBounds, 0);
        Dialogue dialogue = null;
        foreach (Collider2D col in overlaps)
        {
            Dialogue obj = col.gameObject.GetComponent<Dialogue>();
            if (obj != null)
            {
                dialogue = obj;
                break;
            }
        }
        if (dialogue != null)
        {
            if (dialogue == _activeDialogue)
            {
                return;
            }
            promptSprite.transform.position = dialogue.promptAppearTransform.position;
            promptSprite.SetActive(true);
            _activeDialogue = dialogue;
        }
        else
        {
            _activeDialogue = null;
            promptSprite.SetActive(false);
        }
    }
    [Header("Gizmos")]
    public bool ShowDialogueCollisionBox;
    private void OnDrawGizmos()
    {
        if (ShowDialogueCollisionBox)
        {
            int facing = player.LeftOrRight == CharacterController.Facing.left ? -1 : 1;
            Gizmos.DrawWireCube((Vector2)this.transform.position + facing * dialogueDetectorOffset, dialogueDetectorBounds);
        }
    }
}
