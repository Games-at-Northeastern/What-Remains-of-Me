using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for a dialogue tree. Allows dialogue to be animated, skipped,
/// and responded to.
/// </summary>
public abstract class ADialogueTree : MonoBehaviour, IDialogueTree
{
    public abstract void Advance();

    public abstract void AdvanceToEnd();

    public abstract void ChooseResponse(int index);

    public abstract string[] GetResponses();

    public abstract string GetText();

    public abstract bool HasNextSegment();

    public abstract void NextSegment();

    public abstract void Reset();

    public abstract bool SegmentIsComplete();
}
