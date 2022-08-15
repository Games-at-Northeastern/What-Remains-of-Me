using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The end of some branch of a dialogue tree. Has no dialogue of its own.
/// </summary>
public class DialogueLeaf : ADialogueTree
{
    public override void Advance()
    {
        // Nothing
    }

    public override void AdvanceToEnd()
    {
        // Nothing
    }

    public override void ChooseResponse(int index)
    {
        // Nothing
    }

    public override string[] GetResponses()
    {
        return null;
    }

    public override string GetText()
    {
        Debug.LogError("GetText() called on a dialogue leaf. This is probably a mistake.");
        return "";
    }

    public override bool HasNextSegment()
    {
        return false;
    }

    public override void NextSegment()
    {
        // Nothing
    }

    public override void Reset()
    {
        // Nothing
    }

    public override bool SegmentIsComplete()
    {
        return true;
    }
}
