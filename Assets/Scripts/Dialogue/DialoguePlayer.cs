using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Animates dialogue on the given text mesh pro component.
/// </summary>
public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float timeToAdvanceCharacter;
    public UnityEvent onDialogueEnd;
    ControlSchemes cs;

    private void Awake()
    {
        cs = new ControlSchemes();
        cs.Enable();
    }

    public void PlayDialogue(Dialogue d)
    {
        StartCoroutine(DialogueSequence(d));
    }

    IEnumerator DialogueSequence(Dialogue d)
    {
        text.text = "";
        
        IDialogueTree dialogueTree = d.GetDialogueTree();
        print (dialogueTree);
        bool inputForNextSegment;
        cs.Player.Dialogue.performed += _ => inputForNextSegment = true;
        while (dialogueTree.HasNextSegment())
        {   
            dialogueTree.NextSegment();
            // Filling in a segment, char by char
            while (!dialogueTree.SegmentIsComplete())
            {
                dialogueTree.Advance();
                yield return new WaitForSecondsRealtime(timeToAdvanceCharacter);
                text.text = dialogueTree.GetText();
            }
            
            
            // Wait for input for next segment
            inputForNextSegment = false;
            yield return new WaitUntil(() => inputForNextSegment);
            
        }
        dialogueTree.Reset();
        onDialogueEnd.Invoke();
    }
}
