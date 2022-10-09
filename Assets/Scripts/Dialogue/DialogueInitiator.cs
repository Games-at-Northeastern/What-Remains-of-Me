using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles initiation and ending of dialogue sequences.
/// Dialogue sequences can be initiated either when requested by the
/// player's PlayerDialogueInteractor, or when this script's
/// StartDialogueSequence method is called by some other script.
/// Additionally, manages the presence of the dialogue screen.
/// </summary>
public class DialogueInitiator : MonoBehaviour
{
    [SerializeField] PlayerDialogueInteractor pdi;
    [SerializeField] DialoguePlayer dp;
    [SerializeField] GameObject dialogueScreen;
    bool inDialogueSequence;


    /// <summary> 
    /// Adds listeners to start the dialogue sequence and end the dialogue sequence 
    /// when the DialogueInitiator is initialized.
    /// <summary>
    void Start()
    {
        pdi.OnDialogueRequested.AddListener((d) => StartDialogueSequence(d));
        dp.onDialogueEnd.AddListener(() => EndDialogueSequence());
    }

    /// <summary>
    /// Initiates a dialogue sequence from the given Dialogue.
    /// </summary>
    /// <param name="d">The dialogue to be played</param>
    public void StartDialogueSequence(Dialogue d)
    {
        if (!inDialogueSequence)
        {
            inDialogueSequence = true;
            dialogueScreen.SetActive(true);
            dp.PlayDialogue(d);
            GamePauseHandler.setPausedForDialogue(true);
        }
    }

    /// <summary>
    /// Ends the current dialogue sequence if one has been initiated.
    /// </summary>
    public void EndDialogueSequence()
    {
        if (inDialogueSequence)
        {
            inDialogueSequence = false;
            dialogueScreen.SetActive(false);
            GamePauseHandler.setPausedForDialogue(false);
        }
    }
}
