using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be given to a gameObject which can be interacted with by the player
/// for a specific dialogue sequence.
/// </summary>
public class Dialogue : MonoBehaviour
{
  // Where should the prompt (Press 'this button' to start dialogue) show up?
  public Transform promptAppearTransform;

  public ADialogueTree dialogueTree; // Actual content of the dialogue
}