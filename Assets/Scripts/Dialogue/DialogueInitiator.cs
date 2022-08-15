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

    void Start()
    {
        pdi.OnDialogueRequested.AddListener((d) => StartDialogueSequence(d));
        dp.onDialogueEnd.AddListener(() => EndDialogueSequence());
    }

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
