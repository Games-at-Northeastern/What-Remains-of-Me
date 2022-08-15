using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be given to a gameObject which can be interacted with by the player
/// for a specific dialogue sequence.
/// </summary>
public class Dialogue : MonoBehaviour
{
    [SerializeField] Transform promptAppearTransform; // Where should the prompt (Press 'this button' to start dialogue) show up?
    [SerializeField] ADialogueTree dialogueTree; // Actual content of the dialogue

    /// <summary>
    /// Gets the actual content of the dialogue from this dialogue-holding
    /// object.
    /// </summary>
    public IDialogueTree GetDialogueTree()
    {
        return dialogueTree;
    }

    /// <summary>
    /// Gives a transform that indicates where the prompt to start the dialogue
    /// should spawn.
    /// </summary>
    public Transform GetPromptTransform()
    {
        return promptAppearTransform;
    }
}
