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
  [SerializeField] private TextMeshProUGUI text;
  [SerializeField] private float timeToAdvanceCharacter;
  public UnityEvent onDialogueEnd;
  private ControlSchemes cs;

  private void Awake()
  {
    cs = new ControlSchemes();
    cs.Enable();
  }

  /// <summary>
  /// Begins the animation for the given Dialogue
  /// </summary>
  /// <param name="d">The dialogue to be played</param>
  public void PlayDialogue(Dialogue d)
  {
    StartCoroutine(DialogueSequence(d));
  }

  /// <summary>
  /// Coroutine to be called in the PlayDialogue method. This is what actually initiates the dialogue sequence.
  /// </summary>
  /// <param name="d">the dialogue to be played</param>
  /// <returns></returns>
  private IEnumerator DialogueSequence(Dialogue d)
  {
    text.text = "";
    IDialogueTree dialogueTree = d.GetDialogueTree();
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