using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerControllerRefresh;

/// <summary>
/// Script to be held by the player. When the player's dialogue detector
/// collides with some object with dialogue, makes the prompt show up.
/// Additionally, while a prompt is active, allows the player to initiate
/// a dialogue sequence.
/// </summary>
public class PlayerDialogueInteractor : MonoBehaviour
{
  private Dialogue _activeDialogue; // Either the dialogue being prompted or the dialogue being read
  [SerializeField] private PlayerController player;

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
        player = GetComponentInParent<PlayerController>();
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
  /// </summary>
  private void Update()
  {
        // d should have dialogue component if able to be collided with by dialogueCollisionDetector
        GameObject dGameObject = Physics2D.BoxCast(player.position, player.col.size, 0, Vector2.right, 2).collider.gameObject; 
    if (dGameObject != null)
    {
      var dialogue = dGameObject.GetComponent<Dialogue>();
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
}
