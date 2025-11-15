using UnityEngine;

public class EnableDialogue : MonoBehaviour
{
    [SerializeField] private SpriteRenderer dialogueIndicator;
    [SerializeField] private BoxCollider2D dialogueTrigger;

    //Toggles whether a dialogue interaction is enabled/disabled in the scene.
    public void ToggleDialogue()
    {
        dialogueIndicator.enabled = !dialogueIndicator.enabled;
        dialogueTrigger.enabled = !dialogueTrigger.enabled;
    }
}
