using UnityEngine;

/// <summary>
/// Stores a triple of data required for dialogue: the asset and the two ranges. Also contextual information for the dialogue.
/// </summary>
[System.Serializable]
public class DialogueEntry : ARangeVirusEntry<TextAsset>
{
    public bool autoPageTurn;
    public bool stopMovement;
    public float pageTurnWait;
}
