using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The end of some branch of a dialogue tree. Has no dialogue of its own.
/// </summary>
public class DialogueLeaf : ADialogueTree
{
    /// <summary>
    /// Does nothing because this is the end of a branch of a dialog tree.
    /// </summary>
    public override void Advance()
    {
        // Nothing
    }

    /// <summary>
    /// Does nothing because this is the end of a branch of a dialog tree.
    /// </summary>
    public override void AdvanceToEnd()
    {
        // Nothing
    }

    /// <summary>
    /// Does nothing because this is the end of a branch of a dialog tree.
    /// </summary>
    public override void ChooseResponse(int index)
    {
        // Nothing
    }

    /// <summary>
    /// Returns null because this is the end of a branch of a dialog tree.
    /// </summary>
    /// <returns>this DialogTree's responses</returns>
    public override string[] GetResponses()
    {
        return null;
    }

    /// <summary>
    /// Sends an error message to the console. This should not be called on a DialogueLeaf.
    /// </summary>
    /// <returns>an empty string</returns>
    public override string GetText()
    {
        Debug.LogError("GetText() called on a dialogue leaf. This is probably a mistake.");
        return "";
    }


    /// <summary>
    /// Returns false because a this is the end of a branch of a dialgoue tree.
    /// </summary>
    public override bool HasNextSegment()
    {
        return false;
    }

    /// <summary>
    /// Does nothing because this is the end of a branch of a dialog tree.
    /// </summary>
    public override void NextSegment()
    {
        // Nothing
    }

    /// <summary>
    /// Does nothing because this is the end of a branch of a dialog tree.
    /// </summary>
    public override void Reset()
    {
        // Nothing
    }

    /// <summary>
    /// Returns true because a DialogueLeaf indicates the end of a branch of dialog.
    /// </summary>
    /// <returns>whether or not this dialogue segment is complete</returns>
    public override bool SegmentIsComplete()
    {
        return true;
    }
}
